#nullable enable
using System.Reflection;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Common.XML
{
    /// <summary>
    /// The XML Message Dispatcher will handle registering the target methods to valid routes and invoke them accordingly.
    /// </summary>
    public class XMLDispatcher : Dispatcher<XDocument>
    {
        protected override TParam Deserialize<TParam>(XDocument payload)
        {
            return XMLSerializer.Deserialize<TParam>(payload);
        }

        protected override XDocument? Serialize<TResult>(TResult result)
        {
            return XMLSerializer.Serialize(result);
        }

        protected override RouteAttribute? GetAttribute(MethodInfo methodInfo)
        {
            return methodInfo.GetCustomAttribute<RouteAttribute>();
        }

        protected override bool IsValidMatch(RouteAttribute route, XDocument message)
        {
            return (message.XPathEvaluate($"boolean({route.Path})") as bool?) ?? false;
        }
    }
}