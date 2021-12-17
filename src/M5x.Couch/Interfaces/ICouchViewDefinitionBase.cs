namespace M5x.Couch.Interfaces;

public interface ICouchViewDefinitionBase
{
    CouchDesignDocument Doc { get; set; }
    string Name { get; set; }
    ICouchDatabase Db();
    string Path();
    ICouchRequest Request();
}