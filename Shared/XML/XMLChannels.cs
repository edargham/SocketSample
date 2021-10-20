using System.Xml.Linq;

namespace Common.XMLChannels
{
    public class XMLChannel : NetworkChannel<XMLProtocol, XDocument> { }
    public class XMLClientChannel : ClientChannel<XMLProtocol, XDocument> { }
}
