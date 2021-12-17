using System.Collections.Generic;
using M5x.Camunda.Transfer;

namespace M5x.Camunda.Worker;

public interface IExternalTaskAdapter
{
    void Execute(ExternalTask externalTask, ref Dictionary<string, object> resultVariables);
}