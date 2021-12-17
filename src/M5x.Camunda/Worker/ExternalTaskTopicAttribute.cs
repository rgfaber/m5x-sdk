using System;

namespace M5x.Camunda.Worker;

[AttributeUsage(AttributeTargets.Class |
                AttributeTargets.Struct)
]
public sealed class ExternalTaskTopicAttribute : Attribute
{
    public ExternalTaskTopicAttribute(string topicName)
    {
        TopicName = topicName;
    }

    public ExternalTaskTopicAttribute(string topicName, int retries, long retryTimeout)
    {
        TopicName = topicName;
        Retries = retries;
        RetryTimeout = retryTimeout;
    }

    public string TopicName { get; }
    public int Retries { get; } = 5; // default: 5 times
    public long RetryTimeout { get; } = 10 * 1000; // default: 10 seconds
}