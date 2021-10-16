using M5x.Couch.Interfaces;

namespace M5x.Couch
{
    public abstract class CouchViewDefinitionBase : ICouchViewDefinitionBase
    {
        protected CouchViewDefinitionBase(string name, CouchDesignDocument doc)
        {
            Doc = doc;
            Name = name;
        }

        public CouchDesignDocument Doc { get; set; }
        public string Name { get; set; }

        public ICouchDatabase Db()
        {
            return Doc.Owner;
        }

        public ICouchRequest Request()
        {
            return Db().Request(Path());
        }

        public virtual string Path()
        {
            if (Doc.Id == "_design/") return Name;
            return Doc.Id + "/_view/" + Name;
        }
    }
}