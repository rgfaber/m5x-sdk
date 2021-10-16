using System;
using System.Collections.Generic;
using M5x.Common;

namespace M5x.Extensions
{
    public static class ExceptionExtensions
    {
        public static Xeption ToXeption(this Exception e)
        {
            var res = new Xeption
            {
                Subject = "Unexpected Error",
                ErrorMessage = e.Message,
                LastModified = DateTime.UtcNow,
                Source = e.Source,
                Content = e.Message,
                Stack = e.StackTrace
            };
            if (e.InnerException != null)
                res.InnerXeption = e.InnerException.ToXeption();
            return res;
        }


        public static IDictionary<string, Xeption> AddRange(this IDictionary<string, Xeption> target,
            IDictionary<string, Xeption> source)
        {
            if (source == null) return target;
            foreach (var it in source) target.Add(it);
            return target;
        }
    }
}