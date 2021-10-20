using System;

namespace Common
{
    /// <summary>
    /// Specifies which path the target wishes to communicate to.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public abstract class RouteAttribute : Attribute
    {
        public string Path { get; }
        public RouteAttribute(string path)
        {
            Path = path;
        }
    }
}
