using System;
using System.Collections.Generic;

namespace M5x.Schemas
{
    public class Errors : List<KeyValuePair<string, Error>>
    {
        public void Add(string key, Error ex)
        {
            var element = new KeyValuePair<string, Error>(key, ex);
            base.Add(element);
        }

        public void Add(string key, Exception e)
        {
            Add(key, e.AsApiError());
        }

        public new void AddRange(IEnumerable<KeyValuePair<string, Error>> range)
        {
            base.AddRange(range);
        }
    }
}