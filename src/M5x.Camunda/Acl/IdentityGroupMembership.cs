using System.Collections.Generic;
using Newtonsoft.Json;

namespace M5x.Camunda.Acl;

public class IdentityGroupMembership
{
    public IdentityGroupMembership()
    {
        Groups = new List<IdentityGroup>();
        GroupUsers = new List<IdentityUser>();
    }

    /// <summary>
    ///     List of groups the user is a member of
    /// </summary>
    [JsonProperty("groups")]
    public List<IdentityGroup> Groups { get; set; }

    /// <summary>
    ///     List of users who are members of any of the groups
    /// </summary>
    [JsonProperty("groupUsers")]
    public List<IdentityUser> GroupUsers { get; set; }
}

public class IdentityGroup
{
    public string Id;
    public string Name;

    public override string ToString()
    {
        return Id;
    }
}

public class IdentityUser
{
    public string DisplayName;
    public string FirstName;
    public string Id;
    public string LastName;

    public override string ToString()
    {
        return Id;
    }
}