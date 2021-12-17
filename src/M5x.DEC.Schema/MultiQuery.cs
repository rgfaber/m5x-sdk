namespace M5x.DEC.Schema;

public abstract record MultiQuery : Query
{
    protected MultiQuery()
    {
    }

    protected MultiQuery(string correlationId) : base(correlationId)
    {
    }
}