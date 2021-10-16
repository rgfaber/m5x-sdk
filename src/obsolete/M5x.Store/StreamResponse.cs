using M5x.Schemas;

namespace M5x.Store
{
    public record StreamResponse : Response
    {
        public StreamResponse()
        {
        }

        public StreamResponse(string correlationId)
            : base(correlationId)
        {
        }

        public string Id { get; set; }
        public string Rev { get; set; }
        public string Name { get; set; }
        public bool IsEmpty { get; set; }
        public byte[] Content { get; set; }
    }
}