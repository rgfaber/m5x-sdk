namespace M5x.CEQS.TestKit.Integration
{
    public record ContainerInfo : IContainerInfo
    {
        public string ContainerName { get; set; }
        public string ImageName { get; set; }
        public string ImageTag { get; set; }
        public string ExternalPort { get; set; }
        public string InternalPort { get; set; }
    }

    public interface IContainerInfo
    {
        string ContainerName { get; set; }
        string ImageName { get; set; } 
        string ImageTag { get; set; }
        string ExternalPort { get; set; }
        string InternalPort { get; set; }

    }
}