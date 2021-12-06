using M5x.DEC.Schema.Common;

namespace Robby.Game.Schema
{
    public record Competitor
    {
        public Competitor()
        {
            Description = Description.New("new robot");
            Position = Vector.New(0, 0, 0);
        }

        private Competitor(string name, Vector start) : this()
        {
            Description.Name = name;
            Position = start;
        }

        public string Id { get; set; }
        public Description Description { get; set; }
        
        public Vector Position { get; set; }

        public static Competitor New(string name, Vector maxDimensions)
        {
            var start = Vector.Random(maxDimensions.X, maxDimensions.Y, maxDimensions.Z);
            return new Competitor(name, start);
        }
    }
}