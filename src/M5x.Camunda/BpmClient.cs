using System;
using System.Net.Http;
using Camunda.Api.Client;
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
using M5x.Camunda.Interfaces;

namespace M5x.Camunda;

public class BpmClient : IBpmClient
{
    private readonly CamundaClient _camunda;

    public BpmClient()
    {
        var http = new HttpClient();
        http.DefaultRequestHeaders.Add("Authorization", $"Basic {CamundaConfig.Default.BasicAuthorizationKey}");
        http.BaseAddress = new Uri(CamundaConfig.EngineUrl);
        _camunda = CamundaClient.Create(http);
    }

    public ExternalTaskService ExternalTasks => _camunda.ExternalTasks;


    public CaseDefinitionService CaseDefinitions => _camunda.CaseDefinitions;
    public CaseExecutionService CaseExecutions => _camunda.CaseExecutions;
    public DecisionDefinitionService DecisionDefinitions => _camunda.DecisionDefinitions;
    public DeploymentService Deployments => _camunda.Deployments;
    public ExecutionService Executions => _camunda.Executions;
    public GroupService Group => _camunda.Group;
    public HistoryService History => _camunda.History;
    public IncidentService Incidents => _camunda.Incidents;
    public JobDefinitionService JobDefinitions => _camunda.JobDefinitions;
    public JobService Jobs => _camunda.Jobs;
    public MessageService Messages => _camunda.Messages;
    public ProcessDefinitionService ProcessDefinitions => _camunda.ProcessDefinitions;
    public ProcessInstanceService ProcessInstances => _camunda.ProcessInstances;
    public SignalService Signals => _camunda.Signals;
    public TenantService Tenants => _camunda.Tenants;
    public UserService Users => _camunda.Users;
    public UserTaskService UserTasks => _camunda.UserTasks;
    public VariableInstanceService VariableInstances => _camunda.VariableInstances;
}