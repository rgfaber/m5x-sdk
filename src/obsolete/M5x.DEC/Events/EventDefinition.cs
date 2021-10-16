using System;
using M5x.DEC.Core.VersionedTypes;
using M5x.Schemas.VersionedTypes;

namespace M5x.DEC.Events
{
    public record EventDefinition : VersionedTypeDefinition
    {
        public EventDefinition(
            int version,
            Type type,
            string name)
            : base(version, type, name)
        {
        }
    }
}