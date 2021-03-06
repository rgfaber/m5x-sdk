using System;

namespace M5x.CEQS.Schema
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