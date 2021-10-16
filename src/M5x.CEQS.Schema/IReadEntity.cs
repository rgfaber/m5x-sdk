namespace M5x.CEQS.Schema
{
    public interface IReadEntity : IBoundedContext
    {
        string Id { get; set; }
        string Prev { get; set; }
    }
}