using System.Threading.Tasks;
using Ardalis.GuardClauses;
using M5x.DEC.Events;
using M5x.DEC.Persistence;
using M5x.DEC.Schema;


namespace M5x.DEC.Infra
{
    public abstract class EtlEventWriter<TID, TEvent, TModel> 
        : IEtlWriter<TID,TEvent, TModel>
        where TModel : IReadEntity
        where TEvent : IEvent<TID>
        where TID : IIdentity
    {
        private readonly IInterpreter<TModel, TEvent> _interpreter;

        protected TModel Model;
        protected EtlEventWriter(
            IInterpreter<TModel,TEvent> interpreter)
        {
            _interpreter = interpreter;
        }

        protected abstract Task<TModel> ExtractAsync(TEvent @event);
        protected abstract Task<TModel> LoadAsync();

        private void Transform(TEvent @event)
        {
            Model = _interpreter.Interpret(@event, Model);
        }
        
        public async Task HandleAsync(TEvent @event)
        {
            Guard.Against.BadEvent(@event);
            Model = await ExtractAsync(@event);
            Transform(@event);
            await LoadAsync();
        }
        public abstract Task<TModel> DeleteAsync(string id);
    }
}