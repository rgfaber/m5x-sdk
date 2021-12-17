using System;

namespace M5x.Camunda.Acl;

public class DeploymentInfo
{
    /// <summary>
    ///     The time when the deployment was created.
    /// </summary>
    public DateTime DeploymentTime;

    /// <summary>
    ///     The id of the deployment.
    /// </summary>
    public string Id;

    /// <summary>
    ///     The name of the deployment.
    /// </summary>
    public string Name;

    /// <summary>
    ///     The source of the deployment.
    /// </summary>
    public string Source;

    /// <summary>
    ///     The tenant id of the deployment.
    /// </summary>
    public string TenantId;

    public override string ToString()
    {
        return Name ?? Id;
    }
}