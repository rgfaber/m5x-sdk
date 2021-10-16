using M5x.DEC;
using M5x.Publisher.Contract;

namespace M5x.Stan.Subscriber.Cli
{
    public class RafRoot: AggregateRoot<RafId>, IExecute<TestRaf>, IApply<RafTested>
    {
        private RafRoot(RafId id) : base(id)
        {
        }

        public static RafRoot CreateNew(RafId id)
        {
            return new(id);
        }

        public bool Execute(TestRaf command)
        {
            RaiseEvent(RafTested.CreateNew(command.AggregateId, Version));
            return true;
        }

        public void Apply(RafTested aggregateEvent)
        {
            
        }
    }
}