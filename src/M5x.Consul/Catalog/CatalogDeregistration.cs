namespace M5x.Consul.Catalog
{
    public class CatalogDeregistration
    {
        public string Node { get; set; }
        public string Address { get; set; }
        public string Datacenter { get; set; }
        public string ServiceId { get; set; }
        public string CheckId { get; set; }
    }
}