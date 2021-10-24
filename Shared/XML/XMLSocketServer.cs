using System.Xml.Linq;
using Common.XMLChannels;

namespace Common.XML.Server
{
    public class XMLSocketServer : SocketServer<XMLChannel, XMLProtocol, XDocument, XMLDispatcher> { }
}
