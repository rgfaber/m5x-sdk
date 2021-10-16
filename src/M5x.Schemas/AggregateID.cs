namespace M5x.Schemas
{
    public abstract record AggregateId<TId> : Identity<TId>, IAggregateID where TId : Identity<TId>
    {
        private static readonly string Prefix = GetPrefix();

        protected AggregateId(string value) : base(value)
        {
        }

        public string Id => Value;

        public override int GetHashCode()
        {
            return Id != null ? Id.GetHashCode() : 0;
        }

        public override string ToString()
        {
            return Value;
        }

        public static TId NewId()
        {
            return NewComb();
        }
    }


    public interface IAggregateID : IIdentity
    {
//        string Id { get; }
    }
}