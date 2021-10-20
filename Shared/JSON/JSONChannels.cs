using Common.JSON;
using Newtonsoft.Json.Linq;

namespace Common.JSONChannels
{
    public class JSONChannel : NetworkChannel<JSONProtocol, JObject> { }
    public class JSONClientChannel : ClientChannel<JSONProtocol, JObject> { }
}
