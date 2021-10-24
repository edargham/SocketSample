using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Text;

namespace Common
{
    /// <summary>
    /// The channel manager class keeps track of all active connections, 
    /// throttle the number of connections allowed 
    /// and 'groom' channels; i.e. close inactive ones.
    /// </summary>
    public class ChannelManager
    {
        private ConcurrentDictionary<Guid, INetworkChannel> _networkChannels = new ConcurrentDictionary<Guid, INetworkChannel>();
        private readonly Func<INetworkChannel> _networkChannelFactory;
        
        public ChannelManager(Func<INetworkChannel> channelFactory)
        {
            _networkChannelFactory = channelFactory;
        }

        public void Accept(Socket socket)
        {
            INetworkChannel networkChannel = _networkChannelFactory();
            _ = _networkChannels.TryAdd(networkChannel.ID, networkChannel);
            networkChannel.Attach(socket);
        }
    }
}
