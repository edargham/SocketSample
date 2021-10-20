using System;

namespace Common.XML
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class XPathRouteAttribute : RouteAttribute
    {
        public XPathRouteAttribute(string path) : base(path) { } 
    }
}
