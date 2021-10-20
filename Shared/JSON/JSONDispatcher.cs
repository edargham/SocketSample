using Newtonsoft.Json.Linq;
using System.Reflection;

namespace Common.JSON
{
    public class JSONDispatcher : Dispatcher<JObject>
    {
        protected override TParam Deserialize<TParam>(JObject payload)
        {
            return JSONSerializer.Deserialize<TParam>(payload);
        }

        protected override JObject Serialize<TResult>(TResult result)
        {
            return JSONSerializer.Serialize(result);
        }

        protected override RouteAttribute GetAttribute(MethodInfo methodInfo)
        {
           return methodInfo.GetCustomAttribute<JPathRouteAttribute>();
        }

        protected override bool IsValidMatch(RouteAttribute route, JObject message)
        {
            return message.SelectToken(route.Path).ToString() == (route as JPathRouteAttribute).Value;
        }
    }
}
