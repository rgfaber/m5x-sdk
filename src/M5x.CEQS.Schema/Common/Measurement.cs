using System.Text;

namespace M5x.CEQS.Schema.Common
{
    public record Measurement
    {
        public Measurement(decimal? value, int? power, Unit unit)
        {
            Value = value;
            Power = power;
            Unit = unit;
        }

        public decimal? Value { get; set; }
        public int? Power { get; set; }
        public Unit Unit { get; set; }


        public static Measurement CreateNew(decimal? value, int? power, Unit unit)
        {
            return new(value, power, unit);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class Measurement {\n");
            sb.Append("  Value: ").Append(Value).Append("\n");
            sb.Append("  Power: ").Append(Power).Append("\n");
            sb.Append("  Unit: ").Append(Unit).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
    }
}