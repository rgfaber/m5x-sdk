using System;
using M5x.DEC.Schema.Extensions;
using M5x.DEC.Schema.ValueObjects;

namespace M5x.DEC.Commands
{
    public record SourceID : SingleValueObject<string>, ISourceID
    {
        public SourceID(string value)
            : base(value)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException(nameof(value));
        }

        public static ISourceID New => new SourceID(GuidFactories.NewCleanGuid);
    }
}