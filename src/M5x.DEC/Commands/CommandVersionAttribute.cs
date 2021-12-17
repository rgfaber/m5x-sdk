using System;
using M5x.DEC.Schema.VersionedTypes;

namespace M5x.DEC.Commands;

[AttributeUsage(AttributeTargets.Class)]
public class CommandVersionAttribute : VersionedTypeAttribute
{
    public CommandVersionAttribute(
        string name,
        int version)
        : base(name, version)
    {
    }
}