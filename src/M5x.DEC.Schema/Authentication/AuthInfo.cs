namespace M5x.DEC.Schema.Authentication;

public record AuthInfo : IPayload
{
    public AuthInfo(string aggregateId, string token, string username, string displayName, string userId)
    {
        AggregateId = aggregateId;
        Token = token;
        Username = username;
        DisplayName = displayName;
        UserId = userId;
    }

    public string AggregateId { get; set; }
    public string Token { get; set; }
    public string Username { get; set; }
    public string DisplayName { get; set; }
    public string UserId { get; set; }
    public static AuthInfo Default => null;
}