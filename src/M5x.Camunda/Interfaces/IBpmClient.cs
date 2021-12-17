using Camunda.Api.Client.CaseDefinition;
using Camunda.Api.Client.CaseExecution;
using Camunda.Api.Client.DecisionDefinition;
using Camunda.Api.Client.Deployment;
using Camunda.Api.Client.Execution;
using Camunda.Api.Client.ExternalTask;
using Camunda.Api.Client.Group;
using Camunda.Api.Client.History;
using Camunda.Api.Client.Incident;
using Camunda.Api.Client.Job;
using Camunda.Api.Client.JobDefinition;
using Camunda.Api.Client.Message;
using Camunda.Api.Client.ProcessDefinition;
using Camunda.Api.Client.ProcessInstance;
using Camunda.Api.Client.Signal;
using Camunda.Api.Client.Tenant;
using Camunda.Api.Client.User;
using Camunda.Api.Client.UserTask;
using Camunda.Api.Client.VariableInstance;

namespace M5x.Camunda.Interfaces;

public interface IBpmClient
{
    CaseDefinitionService CaseDefinitions { get; }
    CaseExecutionService CaseExecutions { get; }
    DecisionDefinitionService DecisionDefinitions { get; }
    DeploymentService Deployments { get; }
    ExecutionService Executions { get; }
    ExternalTaskService ExternalTasks { get; }
    GroupService Group { get; }
    HistoryService History { get; }
    IncidentService Incidents { get; }
    JobDefinitionService JobDefinitions { get; }
    JobService Jobs { get; }
    MessageService Messages { get; }
    ProcessDefinitionService ProcessDefinitions { get; }
    ProcessInstanceService ProcessInstances { get; }
    SignalService Signals { get; }
    TenantService Tenants { get; }
    UserService Users { get; }
    UserTaskService UserTasks { get; }
    VariableInstanceService VariableInstances { get; }
}