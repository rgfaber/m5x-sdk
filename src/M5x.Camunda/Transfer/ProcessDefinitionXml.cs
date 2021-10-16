namespace M5x.Camunda.Transfer
{
    public class ProcessDefinitionXml
    {
        public string Id { get; set; }
        public string Bpmn20Xml { get; set; }

        public override string ToString()
        {
            return $"ProcessDefinitionXml [Id={Id}]";
        }
    }
}