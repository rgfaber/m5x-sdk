namespace M5x.Camunda.Transfer;

public class ProcessDefinition
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Key { get; set; }
    public string Version { get; set; }
    public string StartFormKey { get; set; }

    public override string ToString()
    {
        return $"ProcessDefinition [Id={Id}, Key={Key}, Name={Name}]";
    }
}