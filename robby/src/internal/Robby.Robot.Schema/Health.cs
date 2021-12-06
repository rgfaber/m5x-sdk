namespace Robby.Robot.Schema
{
    public record Health
    {
        public Health()
        {
        }

        public Health(int value)
        {
            Value = value;
        }

        public int Value { get; set; }
    }
}