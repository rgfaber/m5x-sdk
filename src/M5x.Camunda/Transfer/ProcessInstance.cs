namespace M5x.Camunda.Transfer
{
    public class ProcessInstance
    {
        public string Id { get; set; }
        public string BusinessKey { get; set; }

        public override string ToString()
        {
            return $"ProcessInstance [Id={Id}, BusinessKey={BusinessKey}]";
        }
    }
}