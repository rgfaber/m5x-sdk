using System.ComponentModel.DataAnnotations;
using M5x.DEC.Schema;
using M5x.DEC.Schema.Common;

namespace Robby.Game.Schema
{
    public record InitializationOrder : IPayload
    {
        private string _name;
        private int _numberOfRobots;

        public InitializationOrder()
        {
            FieldDimensions = new FieldDimensions();
        }

        private InitializationOrder(string name, int numberOfRobots, M5x.DEC.Schema.Common.Vector fieldDimensions, string aggregateId)
        {
            Name = name;
            NumberOfRobots = numberOfRobots;
            FieldDimensions = fieldDimensions;
            AggregateId = aggregateId;
        }

        public string Name
        {
            get => _name;
            set => _name = string.IsNullOrWhiteSpace(value) ? "Undefined" : value;
        }

        public int NumberOfRobots
        {
            get => _numberOfRobots;
            set => _numberOfRobots = value <= 0 ? 20 : value;
        }

        public Vector FieldDimensions { get; set; }
        [Required(AllowEmptyStrings = false)]public string AggregateId { get; set; }

        public static InitializationOrder New(string name, int x, int y, int z, int nbrOfRobots)
        {
            return new(name, nbrOfRobots, new M5x.DEC.Schema.Common.Vector(x, y, z), GameModel.ID.New.Value);
        }
    }
}