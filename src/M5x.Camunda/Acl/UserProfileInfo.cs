namespace M5x.Camunda.Acl;

public class UserProfileInfo
{
    /// <summary>
    ///     The email of the user.
    /// </summary>
    public string Email;

    /// <summary>
    ///     The firstname of the user.
    /// </summary>
    public string FirstName;

    /// <summary>
    ///     The id of the user.
    /// </summary>
    public string Id;

    /// <summary>
    ///     The lastname of the user.
    /// </summary>
    public string LastName;

    public override string ToString()
    {
        return Id;
    }
}