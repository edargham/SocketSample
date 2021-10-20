using System;

namespace Common.JSON
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class JPathRouteAttribute : RouteAttribute
    {
        public string Value { get; }
        public JPathRouteAttribute(string path, string value) : base(path)
        {
            Value = value;
        }
    }
}
