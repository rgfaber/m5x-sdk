using System.Collections.Generic;

namespace M5x.Consul.Coordinate;

public class SerfCoordinate
{
    public SerfCoordinate()
    {
        Vec = new List<double>();
    }

    public List<double> Vec { get; set; }
    public double Error { get; set; }
    public double Adjustment { get; set; }
    public double Height { get; set; }
}