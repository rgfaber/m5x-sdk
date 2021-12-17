namespace M5x.DEC.Sagas;

public interface ISaga
{
}

public interface ISaga<TSagaId> : ISaga
    where TSagaId : ISagaId
{
}