namespace M5x.Camunda.Acl;

public class ExecutionInfo
{
    /// <summary>
    ///     A flag indicating whether the execution has ended or not.
    /// </summary>
    public bool Ended;

    /// <summary>
    ///     The id of the execution.
    /// </summary>
    public string Id;

    /// <summary>
    ///     The id of the process instance that this execution instance belongs to.
    /// </summary>
    public string ProcessInstanceId;

    /// <summary>
    ///     The tenant id of the execution.
    /// </summary>
    public string TenantId;

    public override string ToString()
    {
        return Id;
    }
}