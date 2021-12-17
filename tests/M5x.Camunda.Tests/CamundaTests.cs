using System.Threading.Tasks;
using M5x.Camunda.Interfaces;
using M5x.Testing;
using Xunit;
using Xunit.Abstractions;

namespace M5x.Camunda.Tests;

public class CamundaTests : CamundaTestsBase
{
    private IBpmClient _clt;

    public CamundaTests(ITestOutputHelper output, IoCTestContainer container)
        : base(output, container)
    {
    }


    [Fact]
    public async Task Must_ExecuteExternalTask()
    {
        var res = await _clt.ExternalTasks.Query().List();
        Assert.NotNull(res);
        foreach (var externalTaskInfo in res) Assert.NotNull(externalTaskInfo);
    }

    [Fact]
    public void Try_MustContainBPMClient()
    {
        var clt = Container.GetRequiredService<IBpmClient>();
        Assert.NotNull(clt);
    }

    [Fact]
    public async Task Try_MustGetCaseDefinitions()
    {
        var clt = Container.GetRequiredService<IBpmClient>();
        var res = await clt.CaseDefinitions.Query().List();
        Assert.NotNull(res);
        foreach (var caseDefinitionInfo in res) Assert.NotNull(caseDefinitionInfo);
    }


    [Fact]
    public async Task Try_MustGetCaseExecutions()
    {
        var res = await _clt.CaseExecutions.Query().List();
        Assert.NotNull(res);
        foreach (var executionInfo in res) Assert.NotNull(executionInfo);
    }


    [Fact]
    public async Task Try_MustGetDecisionDefinitions()
    {
        var res = await _clt.DecisionDefinitions.Query().List();
        Assert.NotNull(res);
        foreach (var decisionDefinitionInfo in res) Assert.NotNull(decisionDefinitionInfo);
    }

    [Fact]
    public async Task Try_MustGetDeployments()
    {
        var res = await _clt.Deployments.Query().List();
        Assert.NotNull(res);
        foreach (var deploymentInfo in res) Assert.NotNull(deploymentInfo);
    }

    [Fact]
    public async Task Try_MustGetExecutions()
    {
        var res = await _clt.Executions.Query().List();
        Assert.NotNull(res);
        foreach (var executionInfo in res) Assert.NotNull(executionInfo);
    }

    [Fact]
    public async Task Try_MustGetExternalTasks()
    {
        var res = await _clt.ExternalTasks.Query().List();
        Assert.NotNull(res);
        foreach (var externalTaskInfo in res) Assert.NotNull(externalTaskInfo);
    }


    [Fact]
    public async Task Try_MustGetGroup()
    {
        var res = await _clt.Group.Query().List();
        Assert.NotNull(res);
        foreach (var groupInfo in res) Assert.NotNull(groupInfo);
    }

    [Fact]
    public async Task Try_MustGetIncidents()
    {
        var res = await _clt.Incidents.Query().List();
        Assert.NotNull(res);
        foreach (var incidentInfo in res) Assert.NotNull(incidentInfo);
    }


    [Fact]
    public async Task Try_MustGetUsers()
    {
        var res = await _clt.Users.Query().List();
        Assert.NotNull(res);
        foreach (var userProfileInfo in res) Assert.NotNull(userProfileInfo);
    }

    [Fact]
    public async Task Try_MustGetUserTasks()
    {
        var res = await _clt.UserTasks.Query().List();
        foreach (var userTaskInfo in res) Assert.NotNull(userTaskInfo);
    }

    protected override void Initialize()
    {
        _clt = Container.GetRequiredService<IBpmClient>();
    }
}