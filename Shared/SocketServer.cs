using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Common
{
    public abstract class SocketServer<TChannelType, TProtocol, TPayloadType, TDispatcher>
        where TChannelType : NetworkChannel<TProtocol, TPayloadType>, new()
        where TProtocol : Protocol<TPayloadType>, new()
        where TPayloadType : class, new()
        where TDispatcher : Dispatcher<TPayloadType>, new()
    {
        private readonly ChannelManager _networkChannelManager;
        private readonly TDispatcher _dispatcher = new TDispatcher();

        private readonly SemaphoreSlim _connectionSemaphore;

        private Func<Socket> _serverSocketFactory;

        public SocketServer(int maxSupportedClients)
        {
            _connectionSemaphore = new SemaphoreSlim(maxSupportedClients, maxSupportedClients);

            _networkChannelManager = new ChannelManager(() =>
            {
                NetworkChannel<TProtocol, TPayloadType> networkChannel = CreateChannel();
                _dispatcher.Bind(networkChannel);
                return networkChannel;
            });

            _networkChannelManager.ChannelClosed += OnChannelClose;
        }

        private void OnChannelClose(object sender, EventArgs e)
        {
            _connectionSemaphore.Release();
        }

        protected virtual TChannelType CreateChannel()
        {
            return new TChannelType();
        }

        public void Bind<TController>()
        {
            _dispatcher.Bind<TController>();
        }

        /// <summary>
        /// Starts the echo server.
        /// To start the echo server, the endpoint will need to use the machine's IP and reserve a free port to listen on.
        /// A TCP socket stream must be instantiated and bound to the endpoint.
        /// The number of allowed connections to listen must be set. by default this is set to 128.
        /// Specify a task for the server to do.
        /// </summary>
        /// <param name="port">The port number reserved for the server.</param>
        public Task StartAsync(int port, CancellationToken cancellationToken)
        {
            _serverSocketFactory = () =>
            {
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, port);
                Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                socket.Bind(endPoint);
                socket.Listen(128);
                return socket;
            };

            //_ = Task.Run(() => RunAsync(socket));

            return Task.Factory.StartNew(() => RunAsync(cancellationToken), cancellationToken);
        }

        private async Task AcceptConnection(Socket serverSocket)
        {
            // The initial socket must create the actual client socket that will serve the client endpoint data.
            // A task factory will be dispatched to configure this socket.
            Socket clientSocket = await Task.Factory.FromAsync(new Func<AsyncCallback, object, IAsyncResult>(serverSocket.BeginAccept), //
                                                               new Func<IAsyncResult, Socket>(serverSocket.EndAccept), //
                                                               null).ConfigureAwait(false);

            Console.WriteLine("SOCKET SERVER :: ESTABLISHING CONNECTION WITH NEW CLIENT");

            _networkChannelManager.Accept(clientSocket);

            Console.WriteLine($"SOCKET SERVER :: NEW CONNECTION ESTABLISHED (REMAINING CONNECTIONS AVAILABLE: {_connectionSemaphore.CurrentCount})");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            try
            {
                Socket serverSocket = null;

                do
                {
                    bool isConnectionAvailable = await _connectionSemaphore.WaitAsync(1000, cancellationToken);

                    if (!isConnectionAvailable)
                    {
                        Console.WriteLine($"SOCKET SERVER :: MAX CONNECTIONS REACHED, DECLINING REQUEST");
                        try
                        {
                            if (serverSocket != null)
                            {
                                serverSocket.Close();
                                serverSocket.Dispose();
                            }
                            serverSocket = null;
                        }
                        catch { }

                        await _connectionSemaphore.WaitAsync(cancellationToken);
                    }

                    if (!cancellationToken.IsCancellationRequested)
                    {
                        if (serverSocket == null)
                        {
                            serverSocket = _serverSocketFactory();
                        }
                        await AcceptConnection(serverSocket);
                    }
                } while (!cancellationToken.IsCancellationRequested);
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                Console.WriteLine($"SOCKET SERVER :: EXCEPTION CAUGHT WHILE CONNECTING TO THE SOCKET.\nEXCEPTION MESSAGE:\n{ex}");
            }
        }
    }
}

