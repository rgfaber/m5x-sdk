namespace M5x.Store
{
    public class XBulkResponse : XResponse
    {
        public bool IsEmpty { get; set; }
        public Row[] Rows { get; set; }


        public class Row
        {
            public string Id { get; set; }
            public string Rev { get; set; }
            public string Error { get; set; }
            public string Reason { get; set; }
            public bool Succeeded => !string.IsNullOrWhiteSpace(Rev);
        }
    }
}