namespace M5x.Consul.Catalog;

public class CatalogService
{
    public string Node { get; set; }
    public string Address { get; set; }
    public string ServiceId { get; set; }
    public string ServiceName { get; set; }
    public string ServiceAddress { get; set; }
    public string[] ServiceTags { get; set; }
    public int ServicePort { get; set; }
    public bool ServiceEnableTagOverride { get; set; }
}