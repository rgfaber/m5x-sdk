using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using M5x.DEC.Schema;
using M5x.DEC.Schema.Common;

namespace Robby.Game.Schema
{
    public record Robot : IStateEntity<Aggregate.RobotId>, IPayload
    {
        private static readonly string[] _names =
        {
            "John", "Paul", "George", "Ringo", "Stella", "Nathalie", "Katie", "Linda", "Robby", "Sarah", "Lena", "Emma",
            "Nicolas", "Anna", "Betty", "Caroline", "Daisy", "Emma", "Rita", "Theo", "Martin", "Melissa", "Oscar",
            "Penelope", "Xavier", "Logan", "Sonja", "Tony", "Homer", "Bart", "Marge", "Lisa", "Wendy", "Willy",
            "Zoe", "Chloe", "Alex", "Fiona","Jean-Luc", "Jim", "Spock","Albert","Niels","Andre","Winston","Jozef","Gerrit"
        };

        private static readonly string[] _adjectives =
        {
            "Cruel", "Sweet", "Dark", "Friendly", "Famous", "Brave", "Big", "Bloody", "Exotic", "Great", "Holy",
            "Slow", "Lazy", "Joking", "White", "Lovely", "Evil", "Good", "Fat", "Weak", "Strong", "Hot", "Cold",
            "Fast", "Loud", "Silent", "Trusty", "Butch", "Red", "Bad","Sneaky", "Green", "Orange","Blue", "Yellow"
        };

        
        [Flags]
        public enum Flags
        {
            Unknown = 0,
            Pending = 1,
            Active = 2,
            Fighting = 4,
            Spawning = 8,
            InActive = 16
        }

        
        
        
        public Robot()
        {
        }

        public Robot(string id,
            string prev,
            Description description,
            Vector position,
            Health health,
            Flags status,
            RobotKind kind,
            AggregateInfo aggregateInfo)
        {
            Id = id;
            Prev = prev;
            Description = description;
            Position = position;
            Health = health;
            Status = status;
            Kind = kind;
            Meta = aggregateInfo;
        }

        [Required] public string Id { get; set; }
        public string Prev { get; set; }
        public Description Description { get; set; }
        public Vector Position { get; set; }
        public Health Health { get; set; }
        public Flags Status { get; set; }
        public RobotKind Kind { get; set; }

        public static Robot New(M5x.DEC.Schema.Common.Vector maxDimensions)
        {
            var rndName = new Random().Next(_names.Length - 1);
            var rndAdjective = new Random().Next(_adjectives.Length - 1);
            var id = $"{_adjectives[rndAdjective]} {_names[rndName]}";
            return new Robot
            {
                Id = id,
                Description = new Description(id, ""),
                Health = new Health(new Random().Next(5)),
                Kind = (RobotKind) new Random().Next(1, 3),
                Position = new M5x.DEC.Schema.Common.Vector
                {
                    X = new Random().Next(1, maxDimensions.X),
                    Y = new Random().Next(1, maxDimensions.Y),
                    Z = new Random().Next(1, maxDimensions.Z)
                },
                Status = Flags.InActive
            };
        }

        public AggregateInfo Meta { get; set; }
    }

    public static class FlagsExtensions
    {
        public static bool HasFlagFast(this Robot.Flags value, Robot.Flags flag)
        {
            return (value & flag) != 0;
        }
    }

    public enum RobotKind
    {
        [EnumMember(Value = "Rock")] Rock = 1,
        [EnumMember(Value = "Paper")] Paper = 2,
        [EnumMember(Value = "Scissors")] Scissors = 3
    }
}