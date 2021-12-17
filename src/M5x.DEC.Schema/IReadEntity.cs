namespace M5x.DEC.Schema;

public interface IReadEntity : IBoundedContext, IPayload
{
    string Id { get; set; }
    string Prev { get; set; }
}