using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Common
{
    public abstract class SocketServer<TChannelType, TProtocol, TPayloadType, TDispatcher>
        where TChannelType : NetworkChannel<TProtocol, TPayloadType>, new()
        where TProtocol : Protocol<TPayloadType>, new()
        where TPayloadType: class, new()
        where TDispatcher : Dispatcher<TPayloadType>, new()
    {
        private readonly ChannelManager _networkChannelManager;
        private readonly TDispatcher _dispatcher = new TDispatcher();

        public SocketServer()
        {
            _networkChannelManager = new ChannelManager(() =>
            {
                NetworkChannel<TProtocol, TPayloadType> networkChannel = CreateChannel();
                _dispatcher.Bind(networkChannel);
                return networkChannel;
            });
        }

        protected virtual NetworkChannel<TProtocol, TPayloadType> CreateChannel()
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
        public void Start(int port = 42369)
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, port);

            Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(endPoint);
            socket.Listen(128);

            _ = Task.Run(() => RunAsync(socket));
        }

        private async Task RunAsync(Socket socket)
        {
            do
            {
                // The initial socket must create the actual client socket that will serve the client endpoint data.
                // A task factory will be dispatched to configure this socket.
                Socket clientSocket = await Task.Factory.FromAsync(new Func<AsyncCallback, object, IAsyncResult>(socket.BeginAccept), //
                                                                   new Func<IAsyncResult, Socket>(socket.EndAccept), //
                                                                   null).ConfigureAwait(false);

                Console.WriteLine("SOCKET SERVER :: ESTABLISHING CONNECTION WITH NEW CLIENT");

                _networkChannelManager.Accept(clientSocket);

                Console.WriteLine("SOCKET SERVER :: NEW CONNECTION ESTABLISHED");
            }
            while (true);
        }
    }
}

