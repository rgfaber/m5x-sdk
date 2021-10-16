namespace M5x.DEC.Schema.Common
{
    public record Dummy : IPayload
    {
        public Dummy() {}
        
        public static Dummy Empty = new();
    }
}