using System.Linq;

namespace M5x.Schemas
{
    public record ErrorState
    {
        public ErrorState()
        {
            Errors = new Errors();
        }

        public bool IsSuccessful => !Errors.Any();
        public Errors Errors { get; }
    }
}