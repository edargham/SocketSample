using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Common
{
    /// <summary>
    /// The NetworkChannel class handles the process of connecting and sending/recieving messages through the socket.
    /// <br />
    /// Objects of this class and its descendents must attach to a socket, send and receive data through the stream, and then
    /// perform a specified task.
    /// </summary>
    /// <typeparam name="TProtocol">The protocol to use to send the message.</typeparam>
    /// <typeparam name="TMessageType">Type of message that will be passed through the channel.</typeparam>
    public abstract class NetworkChannel<TProtocol, TMessageType> : IDisposable, INetworkChannel where TProtocol : Protocol<TMessageType>, new()
    {
        protected bool _isDisposed = false;
        protected bool _isClosed = false;

        private readonly TProtocol _protocol = new TProtocol();
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private Func<TMessageType, Task> _callback;

        private NetworkStream _stream;
        private Task _channelTask;

        public Guid ID { get; } = Guid.NewGuid();
        public DateTime LastSent { get; protected set; }
        public DateTime LastReceived { get; protected set; }

        public event EventHandler Closed;

        /// <summary>
        /// Opens a stream using the socket and dispatches the callback method passed during initialization.
        /// </summary>
        /// <param name="socket">The socket to use to open a connection.</param>
        public void Attach(Socket socket)
        {
            _stream = new NetworkStream(socket, true);
            _channelTask = Task.Run(ChannelTask, _cancellationTokenSource.Token);
        }

        /// <summary>
        /// Assigns a task to run when the connection is opened.
        /// </summary>
        /// <param name="callback">The callback method to invoke when the socket is attached and opened.</param>
        public void SetCallBack(Func<TMessageType, Task> callback)
        {
            _callback = callback;
        }

        public void Close()
        {
            if (!_isClosed)
            {
                _isClosed = true;
                _cancellationTokenSource.Cancel();
                
                if (_stream != null)
                {
                    _stream.Close();
                }

                if (Closed != null)
                {
                    Closed.Invoke(this, EventArgs.Empty);
                }
                
            }
        }

        /// <summary>
        /// Uses the assigned protocol to send a message via the socket.
        /// </summary>
        /// <param name="message">The message to send through the socket.</param>
        public async Task SendAsync<T>(T message)
        {
            await _protocol.SendAsync(_stream, message).ConfigureAwait(false);
            LastSent = DateTime.UtcNow;
        }

        protected virtual async Task ChannelTask()
        {
            try
            {
                while (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    TMessageType message = await _protocol.ReceiveAsync(_stream).ConfigureAwait(false);
                    LastReceived = DateTime.UtcNow;
                    await _callback(message).ConfigureAwait(false);
                }
            }
            catch (IOException)
            {
                Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"NETWORK CHANNEL :: EXCEPTION CAUGHT WHILE RUNNING THE NETWORK CHANNEL TASK LOOP.\nEXCEPTION MESSAGE:\n{ex}\nNETWORK CHANNEL :: TERMINATING SOCKET CONNECTION...\n");
                Close();
            }
        }

        ~NetworkChannel()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected void Dispose(bool isDisposing)
        {
            if (!_isDisposed)
            {
                _isDisposed = true;

                Close();

                if (_stream != null)
                {
                    _stream.Dispose();
                }

                if (isDisposing)
                {
                    GC.SuppressFinalize(this);
                }
            }
        }
    }
}
