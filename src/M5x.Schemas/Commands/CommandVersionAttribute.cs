using System;
using M5x.Schemas.VersionedTypes;

namespace M5x.Schemas.Commands
{
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
}