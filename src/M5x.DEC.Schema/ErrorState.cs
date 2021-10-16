using System;
using System.Linq;
using System.Text.Json;

namespace M5x.DEC.Schema
{
    [Serializable]
    public sealed record ErrorState
    {
        public ErrorState()
        {
            Errors = new Errors();
        }

        public bool IsSuccessful => !Errors.Any();
        public Errors Errors { get; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}