namespace M5x.Schemas.Commands
{
    
    public interface ICommand : IVersionedType, ICorrelated
    {
        ISourceID GetSourceId();
    }
    
    public interface ICommand<out TIdentity, out TSourceIdentity> : ICommand
        where TIdentity : IIdentity
        where TSourceIdentity : ISourceID
    {
        TIdentity AggregateId { get; }
        TSourceIdentity SourceId { get; }
    }

    public interface ICommand<out TIdentity> : ICommand<TIdentity, ISourceID>
        where TIdentity : IIdentity
    {
    }
    
}