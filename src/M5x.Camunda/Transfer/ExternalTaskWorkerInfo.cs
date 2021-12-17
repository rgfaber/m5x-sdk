using System;
using System.Collections.Generic;
using M5x.Camunda.Worker;

namespace M5x.Camunda.Transfer;

public class ExternalTaskWorkerInfo
{
    public int Retries { get; set; }
    public long RetryTimeout { get; set; }
    public Type Type { get; set; }
    public string TopicName { get; set; }
    public List<string> VariablesToFetch { get; set; }
    public IExternalTaskAdapter TaskAdapter { get; set; }
}