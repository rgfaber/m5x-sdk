using System.Text;

namespace M5x.DEC.Schema.Common;

public record GeoPosition
{
    public Longitude Longitude { get; set; }
    public Latitude Latitude { get; set; }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append("class GeoPosition {\n");
        sb.Append("  Longitude: ").Append(Longitude).Append("\n");
        sb.Append("  Latitude: ").Append(Latitude).Append("\n");
        sb.Append("}\n");
        return sb.ToString();
    }
}