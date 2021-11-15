using System.Threading.Tasks;
using Ardalis.GuardClauses;
using M5x.DEC.Events;
using M5x.DEC.Schema;

namespace M5x.DEC.Infra.CouchDb
{
    public abstract class CouchEtlWriter<TID, TEvent, TModel>
        : EtlEventWriter<TID, TEvent, TModel>
        where TModel : IReadEntity
        where TEvent : IEvent<TID>
        where TID : IIdentity

    {
        protected ICouchStore<TModel> CouchDb;

        protected CouchEtlWriter(IInterpreter<TModel, TEvent> interpreter,
            ICouchStore<TModel> couchDb) : base(interpreter)
        {
            CouchDb = couchDb;
        }

        protected override Task<TModel> LoadAsync()
        {
            return CouchDb.AddOrUpdateAsync(Model);
        }

        protected override async Task<TModel> ExtractAsync(TEvent @event)
        {
            Guard.Against.BadEvent(@event);
            return await CouchDb.GetByIdAsync(@event.Meta.Id);
        }

        public override Task<TModel> DeleteAsync(string id)
        {
            Guard.Against.NullOrEmpty(id, nameof(id));
            return CouchDb.DeleteAsync(id);
        }
    }
}