using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using M5x.Camunda.Interfaces;
using M5x.Camunda.Service;
using M5x.Camunda.Transfer;
using M5x.Camunda.Worker;
using Serilog;

namespace M5x.Camunda;

public class EngineClient : IEngineClient
{
    public static string DEFAULT_URL = "http://localhost:8080/engine-rest/engine/default/";
    public static string COCKPIT_URL = "http://localhost:8080/camunda/app/cockpit/default/";
    private readonly CamundaClientHelper _camundaClientHelper;

    private readonly IList<ExternalTaskWorker> _workers = new List<ExternalTaskWorker>();

    public EngineClient() : this(new Uri(DEFAULT_URL), null, null)
    {
    }

    public EngineClient(Uri restUrl, string userName, string password)
    {
        _camundaClientHelper = new CamundaClientHelper(restUrl, userName, password);
    }

    public BpmnWorkflowService BpmnWorkflowService => new(_camundaClientHelper);

    public HumanTaskService HumanTaskService => new(_camundaClientHelper);

    public RepositoryService RepositoryService => new(_camundaClientHelper);

    public ExternalTaskService2 ExternalTaskService2 => new(_camundaClientHelper);

    public void Startup()
    {
        StartWorkers();
        RepositoryService.AutoDeploy();
    }

    public void Shutdown()
    {
        StopWorkers();
    }

    public void StartWorkers()
    {
        var assembly = Assembly.GetEntryAssembly();
        var externalTaskWorkers = RetrieveExternalTaskWorkerInfo(assembly);
        foreach (var taskWorkerInfo in externalTaskWorkers)
        {
            Log.Information($"Register Task Worker for Topic '{taskWorkerInfo.TopicName}'");
            var worker = new ExternalTaskWorker(ExternalTaskService2, taskWorkerInfo);
            _workers.Add(worker);
            worker.StartWork();
        }
    }

    public void StopWorkers()
    {
        foreach (var worker in _workers) worker.StopWork();
    }

    private static IEnumerable<ExternalTaskWorkerInfo> RetrieveExternalTaskWorkerInfo(Assembly assembly)
    {
        // find all classes with CustomAttribute [ExternalTask("name")]
        var externalTaskWorkers =
            from t in assembly.GetTypes()
            let externalTaskTopicAttribute =
                t.GetCustomAttributes(typeof(ExternalTaskTopicAttribute), true).FirstOrDefault() as
                    ExternalTaskTopicAttribute
            let externalTaskVariableRequirements =
                t.GetCustomAttributes(typeof(ExternalTaskVariableRequirementsAttribute), true).FirstOrDefault() as
                    ExternalTaskVariableRequirementsAttribute
            where externalTaskTopicAttribute != null
            select new ExternalTaskWorkerInfo
            {
                Type = t,
                TopicName = externalTaskTopicAttribute.TopicName,
                Retries = externalTaskTopicAttribute.Retries,
                RetryTimeout = externalTaskTopicAttribute.RetryTimeout,
                VariablesToFetch = externalTaskVariableRequirements?.VariablesToFetch,
                TaskAdapter = t.GetConstructor(Type.EmptyTypes)?.Invoke(null) as IExternalTaskAdapter
            };
        return externalTaskWorkers;
    }

    // HELPER METHODS
}