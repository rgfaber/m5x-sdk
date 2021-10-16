using System;

namespace M5x.DEC.Schema
{
    [AttributeUsage(AttributeTargets.Class)]
    public class EndpointAttribute : Attribute
    {
        public EndpointAttribute(string endpoint)
        {
            Endpoint = endpoint;
        }

        public string Endpoint { get; set; }
    }
}