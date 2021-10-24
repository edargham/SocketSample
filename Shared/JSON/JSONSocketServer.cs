using Common.JSONChannels;
using Newtonsoft.Json.Linq;

namespace Common.JSON.Server
{
    public class JSONSocketServer : SocketServer<JSONChannel, JSONProtocol, JObject, JSONDispatcher> { }
}
