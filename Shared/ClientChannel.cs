using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Common
{
    public class ClientChannel<TProtocol, TMessageType> : NetworkChannel<TProtocol, TMessageType> where TProtocol : Protocol<TMessageType>, new()
    {
        /// <summary>
        /// Creates a TCP socket stream using the specified IP address's family, connects to it and attaches it to the network channel.
        /// </summary>
        /// <param name="endPoint"></param>
        public async Task ConnectAsync(IPEndPoint endPoint)
        {
            Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            await socket.ConnectAsync(endPoint).ConfigureAwait(false);
            Attach(socket);
        }
    }
}
