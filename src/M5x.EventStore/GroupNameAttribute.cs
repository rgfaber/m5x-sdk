using System;

namespace M5x.EventStore;

[AttributeUsage(AttributeTargets.Class)]
public class GroupNameAttribute : Attribute
{
    public GroupNameAttribute(string groupName)
    {
        GroupName = groupName;
    }

    public string GroupName { get; }
}