using CouchDB.Driver.Types;
using M5x.DEC.Persistence;
using M5x.Schemas;

namespace M5x.Store
{
    public class CDoc<T> : CouchDocument where T : IReadEntity
    {
        public T Data { get; set; }
    }
}