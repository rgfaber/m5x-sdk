using System;
using System.Linq;

namespace M5x.Store
{
    public class ViewResponse<TKey, TValue>
    {
        public bool HasRows => Rows?.Any() ?? false;
        public Exception Exception { get; set; }
        public ViewResponseRow<TKey, TValue>[] Rows { get; set; }
    }
}