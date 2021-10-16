using System;

namespace M5x.Store
{
    public class DocBase
    {
        public DocBase()
        {
            Ts = DateTime.UtcNow;
        }

        public string Id { get; set; }
        public string Rev { get; set; }
        public DateTime Ts { get; set; }
    }
}