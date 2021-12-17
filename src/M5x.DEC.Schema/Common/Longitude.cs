using System.Runtime.Serialization;
using System.Text;

namespace M5x.DEC.Schema.Common;

public record Longitude
{
    public enum EwEnum
    {
        /// <summary>
        ///     Enum EEnum for E
        /// </summary>
        [EnumMember(Value = "E")] E = 0,

        /// <summary>
        ///     Enum WEnum for W
        /// </summary>
        [EnumMember(Value = "W")] W = 1
    }

    public int? Degrees { get; set; }
    public int? Minutes { get; set; }
    public decimal? Seconds { get; set; }

    public EwEnum? Ew { get; set; }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append("class Longitude {\n");
        sb.Append("  Degrees: ").Append(Degrees).Append("\n");
        sb.Append("  Minutes: ").Append(Minutes).Append("\n");
        sb.Append("  Seconds: ").Append(Seconds).Append("\n");
        sb.Append("  Ew: ").Append(Ew).Append("\n");
        sb.Append("}\n");
        return sb.ToString();
    }
}