using M5x.Camunda.Service;

namespace M5x.Camunda.Interfaces
{
    public interface IEngineClient
    {
        BpmnWorkflowService BpmnWorkflowService { get; }
        HumanTaskService HumanTaskService { get; }
        RepositoryService RepositoryService { get; }
        ExternalTaskService2 ExternalTaskService2 { get; }
        void Startup();
        void Shutdown();
        void StartWorkers();
        void StopWorkers();
    }
}