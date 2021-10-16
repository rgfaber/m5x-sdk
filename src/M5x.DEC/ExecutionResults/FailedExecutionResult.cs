using System.Collections.Generic;
using System.Linq;

namespace M5x.DEC.ExecutionResults
{
    public record FailedExecutionResult : ExecutionResult
    {
        public FailedExecutionResult(
            IEnumerable<string> errors)
        {
        }


        public override string ToString()
        {
            return ErrorState.Errors.Any()
                ? $"Failed execution due to: {string.Join(", ", ErrorState.Errors)}"
                : "Failed execution";
        }
    }
}