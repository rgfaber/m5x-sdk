using System;

namespace M5x.EventStore
{
    [AttributeUsage(AttributeTargets.Class)]
    public class StreamNameAttribute : Attribute
    {
        public StreamNameAttribute(string streamName)
        {
            StreamName = streamName;
        }

        public string StreamName { get; }
    }
}