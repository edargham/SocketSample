using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Timers;

namespace Common
{
    /// <summary>
    /// The channel manager class keeps track of all active connections, 
    /// throttle the number of connections allowed 
    /// and 'groom' channels; i.e. close inactive ones.
    /// </summary>
    public class ChannelManager
    {
        const int TIMEOUT_INTERVAL_MINUTES = 1;

        private ConcurrentDictionary<Guid, INetworkChannel> _networkChannels = new ConcurrentDictionary<Guid, INetworkChannel>();
        private readonly Func<INetworkChannel> _networkChannelFactory;

        private  readonly Timer _watchdogTimer = new Timer(TIMEOUT_INTERVAL_MINUTES * 60000);

        public ChannelManager(Func<INetworkChannel> channelFactory)
        {
            _networkChannelFactory = channelFactory;

            _watchdogTimer.Elapsed += OnWatchdogTimerElapsed;
            _watchdogTimer.Start();
        }

        private void OnWatchdogTimerElapsed(object sender, ElapsedEventArgs e)
        {
            _watchdogTimer.Stop();

            Console.WriteLine("WATCHDOG :: MARKING DEAD CHANNELS FOR TERMINATION...");

            int socketsClosed = 0;

            try
            {
                List<Guid> deadChannels = new List<Guid>();

                TimeSpan timeoutTimeSpan = new TimeSpan(0, TIMEOUT_INTERVAL_MINUTES, 0);
                DateTime delta = DateTime.UtcNow.Subtract(timeoutTimeSpan);

                foreach (Guid channelKey in _networkChannels.Keys)
                {
                    INetworkChannel channel = _networkChannels[channelKey];

                    DateTime lastActivityTime = DateTime.Compare(channel.LastReceived, channel.LastSent) > 0 ? channel.LastReceived : channel.LastSent;
                    int lastActivityDuration = DateTime.Compare(delta, lastActivityTime);

                    if (lastActivityDuration < 0)
                    {
                        deadChannels.Add(channelKey);
                    }
                }

                Console.WriteLine($"WATCHDOG :: {deadChannels.Count} DEAD CHANNELS MARKED FOR TERMINATION. PROCESSING...");

                foreach (Guid channelKey in deadChannels)
                {
                    Console.WriteLine($"WATCHDOG :: TERMINATING DEAD CHANNEL {channelKey}...");

                    INetworkChannel deadChannel = _networkChannels[channelKey];
                    deadChannel.Close();

                    socketsClosed++;
                }

            }
            finally
            {
                _watchdogTimer.Start();
            }

            Console.WriteLine($"WATCHDOG :: {socketsClosed} CHANNELS SUCCESSFULLY TERMINATED.");
        }

        public void Accept(Socket socket)
        {
            INetworkChannel networkChannel = _networkChannelFactory();
            _ = _networkChannels.TryAdd(networkChannel.ID, networkChannel);
            networkChannel.Closed += (sender, eventArgs) => _networkChannels.TryRemove(networkChannel.ID, out _);
            networkChannel.Attach(socket);
        }
    }
}
