using System.Text;

namespace M5x.DEC.Schema.Common
{
    public record Volume
    {
        public Volume()
        {
            Length = IsoDimensions.Length();
            Width = IsoDimensions.Length();
            Height = IsoDimensions.Length();
        }

        public Measurement Length { get; set; }
        public Measurement Width { get; set; }
        public Measurement Height { get; set; }

        /// <summary>
        ///     Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("record Dimensions {\n");
            sb.Append("  Length: ").Append(Length).Append("\n");
            sb.Append("  Width: ").Append(Width).Append("\n");
            sb.Append("  Height: ").Append(Height).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
    }
}