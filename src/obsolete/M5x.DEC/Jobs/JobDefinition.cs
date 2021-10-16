using System;
using M5x.DEC.Core.VersionedTypes;
using M5x.Schemas.VersionedTypes;

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