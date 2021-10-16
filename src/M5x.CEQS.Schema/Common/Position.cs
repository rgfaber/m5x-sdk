using System.Text;

namespace M5x.CEQS.Schema.Common
{
    public record Position
    {
        public int? X { get; set; }
        public int? Y { get; set; }
        public int? Z { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class Vector {\n");
            sb.Append("  X: ").Append(X).Append("\n");
            sb.Append("  Y: ").Append(Y).Append("\n");
            sb.Append("  Z: ").Append(Z).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
    }
}