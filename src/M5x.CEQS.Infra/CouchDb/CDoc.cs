using System;
using CouchDB.Driver.Types;
using M5x.CEQS.Schema;


namespace M5x.CEQS.Infra.CouchDb
{
    public class CDoc<T> : CouchDocument where T : IReadEntity
    {
        public T Data { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}