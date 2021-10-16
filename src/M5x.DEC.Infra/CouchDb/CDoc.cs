using System;
using CouchDB.Driver.Types;
using M5x.DEC.Schema;

namespace M5x.DEC.Infra.CouchDb
{
    public class CDoc<T> : CouchDocument where T : IReadEntity
    {
        public T Data { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}