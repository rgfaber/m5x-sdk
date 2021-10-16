using System;
using M5x.DEC.Schema.VersionedTypes;

namespace M5x.DEC.Jobs
{
    public record JobDefinition : VersionedTypeDefinition
    {
        public JobDefinition(
            int version,
            Type type,
            string name)
            : base(version, type, name)
        {
        }
    }
}