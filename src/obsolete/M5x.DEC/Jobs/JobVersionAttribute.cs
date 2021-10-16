using System;
using M5x.DEC.Core.VersionedTypes;
using M5x.Schemas.VersionedTypes;

namespace M5x.DEC.Jobs
{
    [AttributeUsage(AttributeTargets.Class)]
    public class JobVersionAttribute : VersionedTypeAttribute
    {
        public JobVersionAttribute(
            string name,
            int version)
            : base(name, version)
        {
        }
    }
}