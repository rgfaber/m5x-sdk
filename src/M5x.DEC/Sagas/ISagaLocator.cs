namespace M5x.DEC.Sagas;

public interface ISagaLocator<out TIdentity>
    where TIdentity : ISagaId
{
    TIdentity LocateSaga(IDomainEvent domainEvent);
}