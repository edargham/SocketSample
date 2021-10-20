using Newtonsoft.Json.Linq;
using System.Text;

namespace Common.JSON
{
    /// <summary>
    /// Serializes the message using the JSON standard to send/recieve data to/from the server.
    /// </summary>
    public class JSONProtocol : Protocol<JObject>
    {
        protected override JObject DecodeMessage(byte[] message)
        {
            string serializedMessage = Encoding.UTF8.GetString(message);
            return JSONSerializer.Deserialize(serializedMessage);
        }

        protected override byte[] EncodeBody<T>(T message)
        {
            string serializedMessage = JSONSerializer.Serialize(message).ToString();
            return Encoding.UTF8.GetBytes(serializedMessage);
        }
    }
}
