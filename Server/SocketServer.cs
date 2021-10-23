using Common;
using Common.JSON;
using Common.JSONChannels;
using Common.XML;
using Common.XMLChannels;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Server
{
    public class SocketServer
    {
        //private readonly XMLDispatcher _xmlDispatcher = new XMLDispatcher();
        private readonly JSONDispatcher _jsonDispatcher = new JSONDispatcher();

        public SocketServer()
        {
            //_xmlDispatcher.Register<HeartBeatRequestMessage<PayloadMessage>, HeartBeatResponseMessage<PayloadMessage>>(Handler.HandleResponse);
            //_jsonDispatcher.Register<HeartBeatRequestMessage<PayloadMessage>, HeartBeatResponseMessage<PayloadMessage>>(Handler.HandleResponse);

            //_xmlDispatcher.Bind<Handler>();
            _jsonDispatcher.Bind<Handler>();
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

            _ = Task.Run(() => Echo(socket));
        }

        private async Task Echo(Socket socket)
        {
            do
            {
                // The initial socket must create the actual client socket that will serve the client endpoint data.
                // A task factory will be dispatched to configure this socket.
                Socket clientSocket = await Task.Factory.FromAsync(new Func<AsyncCallback, object, IAsyncResult>(socket.BeginAccept), //
                                                               new Func<IAsyncResult, Socket>(socket.EndAccept), //
                                                               null).ConfigureAwait(false);

                Console.WriteLine("ECHO SERVER :: NEW CONNECTION ESTABLISHED");

                //XMLChannel xmlNetworkChannel = new XMLChannel();
                JSONChannel jsonNetworkChannel = new JSONChannel();

                //_xmlDispatcher.Bind(xmlNetworkChannel);
                _jsonDispatcher.Bind(jsonNetworkChannel);

                //xmlNetworkChannel.Attach(clientSocket);
                jsonNetworkChannel.Attach(clientSocket);

                while (true) { }
            }
            while (true);
        }
    }
}
