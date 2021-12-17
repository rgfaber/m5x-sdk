namespace M5x.Consul.Client;

/// <summary>
///     WriteOptions are used to parameterize a write
/// </summary>
public class WriteOptions
{
    public static readonly WriteOptions Default = new()
    {
        Datacenter = string.Empty,
        Token = string.Empty
    };

    /// <summary>
    ///     Providing a datacenter overwrites the DC provided by the Config
    /// </summary>
    public string Datacenter { get; set; }

    /// <summary>
    ///     Token is used to provide a per-request ACL token which overrides the agent's default token.
    /// </summary>
    public string Token { get; set; }
}