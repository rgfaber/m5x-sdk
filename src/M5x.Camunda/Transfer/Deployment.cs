namespace M5x.Camunda.Transfer
{
    public class Deployment
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Source { get; set; }

        public override string ToString()
        {
            return $"Deployment [Id={Id}, Name={Name}]";
        }
    }
}