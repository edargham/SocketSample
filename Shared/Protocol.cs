using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Common
{
    /// <summary>
    /// Standardizes and serializes the message format used to send and recieve data between the clients and the server.
    /// <br />
    /// Descendents of this class must handle encoding and decoding the message using the appropriate desirable format.
    /// </summary>
    /// <typeparam name="TMessageType">The type of the class the messages will be serialized to.</typeparam>
    public abstract class Protocol<TMessageType>
    {
        private const int NETWORK_HEADER_SIZE = 4;
        // To send a message of any message format we must first encode the message the send it through the stream.
        // After a message of any message of any message format is recieved, the bytes must be decoded to read the message.

        /// <summary>
        /// Serialize the message and create the header.
        /// <br/> 
        /// The header should be 4 bytes in length expressed in network byte order. 
        /// The 4 bytes will represent the size of the body to expect.
        /// <br/>
        /// This header allows the content to be network agnostic 
        /// which helps mitigate the discrepencies between little-endian and big-endian notations on different servers.
        /// </summary>
        /// <returns>A byte array that represents the body of the encoded message.</returns>
        protected abstract byte[] EncodeBody<T>(T message);

        private (byte[] header, byte[] body) EncodeMessage<T>(T message)
        {
            byte[] bodyBytes = EncodeBody(message);
            byte[] headerBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(bodyBytes.Length));

            return (headerBytes, bodyBytes);
        }

        protected abstract TMessageType DecodeMessage(byte[] message);

        private async Task<byte[]> ReadBytesAsync(NetworkStream stream, int bufferSize)
        {
            byte[] buffer = new byte[bufferSize];
            int bytesSuccessfulyRead = 0;

            while (bytesSuccessfulyRead < bufferSize)
            {
                int newBytesReceived = await stream.ReadAsync(buffer, bytesSuccessfulyRead, (bufferSize - bytesSuccessfulyRead)).ConfigureAwait(false);

                if (newBytesReceived == 0)
                {
                    throw new Exception("Connection was closed prematurely.");
                }

                bytesSuccessfulyRead += newBytesReceived;
            }

            return buffer;
        }

        private async Task<int> ReadHeader(NetworkStream stream)
        {
            byte[] headerBytes = await ReadBytesAsync(stream, NETWORK_HEADER_SIZE).ConfigureAwait(false);
            return IPAddress.NetworkToHostOrder(BitConverter.ToInt32(headerBytes));
        }

        private async Task<TMessageType> ReadBody(NetworkStream stream, int bodySize)
        {
            byte[] bodyBytes = await ReadBytesAsync(stream, bodySize);
            return DecodeMessage(bodyBytes);
        }

        protected virtual void AssertValidBodySize(int bodySize)
        {
            if (bodySize < 1)
            {
                throw new ArgumentOutOfRangeException("The recieved body size is out of range of the accepted sizes.");
            }
        }

        /// <summary>
        /// Receive a message from the stream.
        /// </summary>
        /// <param name="stream">The stream through which to await a message.</param>
        /// <returns>The deserialized message received.</returns>
        public async Task<TMessageType> ReceiveAsync(NetworkStream stream)
        {
            int bodySize = await ReadHeader(stream);

            AssertValidBodySize(bodySize);

            return await ReadBody(stream, bodySize);
        }

        /// <summary>
        /// Send a message 
        /// </summary>
        /// <typeparam name="T">Type of the message to be serialized.</typeparam>
        /// <param name="stream">The stream through which the message will be sent.</param>
        /// <param name="message">The message to serialize send.</param>
        public async Task SendAsync<T>(NetworkStream stream, T message)
        {
            (byte[] header, byte[] body) = EncodeMessage(message);

            byte[] packet = new byte[header.Length + body.Length];

            Buffer.BlockCopy(header, 0, packet, 0, header.Length);
            Buffer.BlockCopy(body, 0, packet, header.Length, body.Length);

            await stream.WriteAsync(packet, 0, packet.Length).ConfigureAwait(true);
        }
    }
}
