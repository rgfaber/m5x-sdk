using System;
using CouchDB.Driver.Indexes;
using M5x.Schemas;

namespace M5x.DEC.Infra.CouchDb
{
    public class DbIndexInfo<TReadModel> where TReadModel:IReadEntity
    {
        private DbIndexInfo(string name, Action<IIndexBuilder<CDoc<TReadModel>>> builder, IndexOptions options)
        {
            Name = name;
            Builder = builder;
            Options = options;
        }
        public string Name { get; set; }
        
        public string DesignDoc { get; set; }
        public Action<IIndexBuilder<CDoc<TReadModel>>> Builder { get; set; }
        public IndexOptions Options { get; set; }

        public static DbIndexInfo<TReadModel> CreateNew(string indexName, 
            Action<IIndexBuilder<CDoc<TReadModel>>> indexBuilder, 
            IndexOptions options)
        {
            return new(indexName, indexBuilder, options);
        }
    }
}