using System.Collections.Concurrent;
using System.Collections.Generic;
using M5x.DEC.Schema;

namespace M5x.DEC
{
    public interface IFactCache<TFact, TAckFact>
        where TFact : IFact
        where TAckFact : IFact
    {
        ConcurrentDictionary<string, TFact> Items { get; }
        void RemoveFact(TAckFact ackFact);
        void AddFact(TFact fact);
    }

    public abstract class FactCache<TFact, TAckFact>
        : IFactCache<TFact, TAckFact>
        where TFact : IFact
        where TAckFact : IFact
    {
        protected FactCache()
        {
            Items = new ConcurrentDictionary<string, TFact>();
        }

        public void RemoveFact(TAckFact ackFact)
        {
            if (Items.ContainsKey(ackFact.CorrelationId)) Items.Remove(ackFact.CorrelationId, out _);
        }


        public void AddFact(TFact fact)
        {
            Items.TryAdd(fact.CorrelationId, fact);
        }

        public ConcurrentDictionary<string, TFact> Items { get; }
    }
}