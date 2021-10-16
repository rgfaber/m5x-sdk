using System.Collections.Generic;
using System.Linq;
using M5x.DEC.Schema;

namespace M5x.DEC.ExecutionResults
{
    public abstract record ExecutionResult : IExecutionResult
    {
        private static readonly IExecutionResult SuccessResult = new SuccessExecutionResult();
        private static readonly IExecutionResult FailedResult = new FailedExecutionResult(Enumerable.Empty<string>());

        protected ExecutionResult()
        {
            ErrorState = new ErrorState();
        }

        public ErrorState ErrorState { get; }


        public bool IsSuccess => this is SuccessExecutionResult;

        public static IExecutionResult Success()
        {
            return SuccessResult;
        }

        public static IExecutionResult Failed()
        {
            return FailedResult;
        }

        public static IExecutionResult Failed(IEnumerable<string> errors)
        {
            return new FailedExecutionResult(errors.ToArray());
        }

        public static IExecutionResult Failed(params string[] errors)
        {
            return new FailedExecutionResult(errors);
        }

        public override string ToString()
        {
            return $"ExecutionResult - IsSuccess:{ErrorState.IsSuccessful}";
        }
    }
}