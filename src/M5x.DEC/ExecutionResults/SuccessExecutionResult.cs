namespace M5x.DEC.ExecutionResults
{
    public record SuccessExecutionResult : ExecutionResult
    {
        public override string ToString()
        {
            return "Successful execution";
        }
    }
}