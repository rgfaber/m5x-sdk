namespace M5x.Docker
{
    public class Port
    {
        public string IP { get; set; }
        public ushort PrivatePort { get; set; }
        public ushort PublicPort { get; set; }
        public string Type { get; set; }
    }
}