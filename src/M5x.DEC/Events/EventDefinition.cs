using System;
using M5x.DEC.Schema.VersionedTypes;

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