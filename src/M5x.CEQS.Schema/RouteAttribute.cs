using System;

namespace M5x.CEQS.Schema
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RouteAttribute : Attribute
    {
        public RouteAttribute(string route)
        {
            Route = route;
        }

        public string Route { get; set; }
    }
}