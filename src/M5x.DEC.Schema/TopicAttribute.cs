using System;

namespace M5x.DEC.Schema
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TopicAttribute : Attribute
    {
        public TopicAttribute(string id)
        {
            Id = id;
        }

        public string Id { get; set; }
    }
}