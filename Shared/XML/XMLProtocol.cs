using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Common
{
    /// <summary>
    /// Serializes the message using the XML standard to send/recieve data to/from the server.
    /// </summary>
    public class XMLProtocol : Protocol<XDocument>
    {
        protected override XDocument DecodeMessage(byte[] message)
        {
            string serializedXML = Encoding.ASCII.GetString(message);

            StringReader stringReader = new StringReader(serializedXML);

            XmlReaderSettings xmlSettings = new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Ignore
            };

            XmlReader reader = XmlReader.Create(stringReader, xmlSettings);

            return XDocument.Load(reader);
        }

        protected override byte[] EncodeBody<T>(T message)
        {
            if (message is XDocument)
            {
                byte[] bodyBytes = Encoding.ASCII.GetBytes(message.ToString());
                return bodyBytes;
            }
            else
            {
                byte[] bodyBytes = Encoding.ASCII.GetBytes(XMLSerializer.Serialize(message).ToString());
                return bodyBytes;
            }

        }
    }
}
