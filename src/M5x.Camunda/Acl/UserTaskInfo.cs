using System;
using Camunda.Api.Client.UserTask;

namespace M5x.Camunda.Acl;

public class UserTaskInfo : UserTask
{
    /// <summary>
    ///     The id of the case definition the task belongs to.
    /// </summary>
    public string CaseDefinitionId;

    /// <summary>
    ///     The id of the case execution the task belongs to.
    /// </summary>
    public string CaseExecutionId;

    /// <summary>
    ///     The time the task was created.
    /// </summary>
    public DateTime? Created;

    /// <summary>
    ///     The id of the execution the task belongs to.
    /// </summary>
    public string ExecutionId;

    /// <summary>
    ///     If not null, the form key for the task.
    /// </summary>
    public string FormKey;

    /// <summary>
    ///     The id of the task.
    /// </summary>
    public string Id;

    /// <summary>
    ///     The id of the process definition this task belongs to.
    /// </summary>
    public string ProcessDefinitionId;

    /// <summary>
    ///     The id of the process instance this task belongs to.
    /// </summary>
    public string ProcessInstanceId;

    /// <summary>
    ///     The task definition key.
    /// </summary>
    public string TaskDefinitionKey;

    public override string ToString()
    {
        return base.ToString() ?? Id;
    }
}