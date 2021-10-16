using System;
using System.Collections.Generic;

namespace M5x.DEC.Core
{
    public interface IMetadataContainer : IReadOnlyDictionary<string, string>
    {
        string GetMetadataValue(string key);
        T GetMetadataValue<T>(string key, Func<string, T> converter);
    }
}