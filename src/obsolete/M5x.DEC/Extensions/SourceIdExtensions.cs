using M5x.Schemas.Commands;

namespace M5x.DEC.Extensions
{
    public static class SourceIdExtensions
    {
        public static bool IsNone(this ISourceID sourceId)
        {
            return string.IsNullOrEmpty(sourceId?.Value);
        }
    }
}