using Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Server.Controllers
{
    public class POSController
    {
        // The use of weak reference is present to handle the termination of the inactive channel.
        private readonly ConcurrentDictionary<string, WeakReference<INetworkChannel>> _posChannelMap = new ConcurrentDictionary<string, WeakReference<INetworkChannel>>();

        /// <summary>
        /// Update the mapping between the channel and the POS object.
        /// </summary>
        public void ProcessHeartBeat(string posID, INetworkChannel channel)
        {
            WeakReference<INetworkChannel> weakChannelReference = new WeakReference<INetworkChannel>(channel);
            _posChannelMap.AddOrUpdate(posID, weakChannelReference, (pid, weakref) => weakChannelReference);
        }

        public async Task SendTo<T>(T message) where T : Message<POSData>
        {
            if (message.Data != null)
            {
                string paylaodID = message.Data.ID;
                
                if (string.IsNullOrWhiteSpace(paylaodID))
                {
                    throw new Exception("Every payload message must be associated a unique identifier.");
                }

                // Check if the channel has been disposed of.
                if (_posChannelMap.TryGetValue(paylaodID, out WeakReference<INetworkChannel> weakChannelReference) == true)
                {
                    if (weakChannelReference.TryGetTarget(out INetworkChannel networkChannel))
                    {
                        await networkChannel.SendAsync(message).ConfigureAwait(false);
                    }
                    else // Enters when channel is disposed.
                    {
                        _posChannelMap.TryRemove(paylaodID, out _);
                    }
                }
            }
        }
    }
}
