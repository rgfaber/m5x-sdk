using System;
using M5x.DEC.Core.VersionedTypes;
using M5x.Schemas.VersionedTypes;

namespace M5x.DEC.Events
{
    [AttributeUsage(
        AttributeTargets.Class,
        AllowMultiple = true
    )]
    public class EventVersionAttribute : VersionedTypeAttribute
    {
        public EventVersionAttribute(
            string name,
            int version)
            : base(name, version)
        {
        }
    }
}