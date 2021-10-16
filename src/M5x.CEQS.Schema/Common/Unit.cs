using System.Text;

namespace M5x.CEQS.Schema.Common
{
    public record Unit
    {
        public Unit()
        {
            BaseUnit = null;
        }


        public Unit(string code, string symbol, Unit? baseUnit = null, int? power = 0)
        {
            Code = code;
            Symbol = symbol;
            BaseUnit = baseUnit;
            Power = power;
        }

        public string Code { get; set; }
        public string Symbol { get; set; }
        public Unit? BaseUnit { get; set; }
        public int? Power { get; set; }

        public static Unit CreateNew(string code, string symbol, Unit? baseUnit = null, int? power = 0)
        {
            return new(code, symbol, baseUnit, power);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class Unit {\n");
            sb.Append("  Code: ").Append(Code).Append("\n");
            sb.Append("  Symbol: ").Append(Symbol).Append("\n");
            sb.Append("  Base: ").Append(BaseUnit).Append("\n");
            sb.Append("  Power: ").Append(Power).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
    }
}