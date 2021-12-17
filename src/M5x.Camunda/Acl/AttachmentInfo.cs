namespace M5x.Camunda.Acl;

public class AttachmentInfo
{
    public string Description;
    public string Id;
    public string Name;
    public string TaskId;
    public string Type;
    public string Url;

    public override string ToString()
    {
        return Id;
    }
}