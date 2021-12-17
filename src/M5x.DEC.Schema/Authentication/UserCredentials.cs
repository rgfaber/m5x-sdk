namespace M5x.DEC.Schema.Authentication;

public record UserCredentials : IPayload
{
    public UserCredentials(string username, string password, string aggregateId)
    {
        Username = username;
        Password = password;
        AggregateId = aggregateId;
    }

    public string Username { get; set; }
    public string Password { get; set; }

    public string AggregateId { get; set; }

    public static UserCredentials New(string username, string password)
    {
        return new UserCredentials(username, password, string.Empty);
    }
}