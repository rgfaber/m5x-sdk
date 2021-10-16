using System.Collections.Generic;

namespace M5x.Store
{
    public class QueryViewDoc<T> where T : class
    {
        public IEnumerable<string> Keys { get; set; }
        public IEnumerable<T> Docs { get; set; }
    }
}