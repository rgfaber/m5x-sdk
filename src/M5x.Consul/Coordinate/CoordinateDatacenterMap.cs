using System.Collections.Generic;

namespace M5x.Consul.Coordinate
{
    public class CoordinateDatacenterMap
    {
        public CoordinateDatacenterMap()
        {
            Coordinates = new List<CoordinateEntry>();
        }

        public string Datacenter { get; set; }
        public List<CoordinateEntry> Coordinates { get; set; }
    }
}