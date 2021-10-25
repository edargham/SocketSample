using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Common
{
    public interface INetworkChannel
    {
        Guid ID { get; }
        DateTime LastSent { get; }
        DateTime LastReceived { get; }
 
        event EventHandler Closed;

        void Attach(Socket socket);
        void Close();
        void Dispose();
        Task SendAsync<T>(T message);
    }
}