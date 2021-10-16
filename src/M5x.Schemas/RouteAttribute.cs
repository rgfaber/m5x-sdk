using System;

namespace M5x.Schemas
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