namespace M5x.DEC.Schema.Common;

public record Description : IPayload
{
    public Description()
    {
    }

    public Description(string name, string comments)
    {
        Name = name;
        Comments = comments;
    }

    public string Name { get; set; }
    public string Comments { get; set; }

    public static Description New(string name, string comments = "")
    {
        return new Description(name, comments);
    }
}