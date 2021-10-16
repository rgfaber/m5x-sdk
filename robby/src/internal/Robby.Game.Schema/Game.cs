using System;
using M5x.DEC.Schema;
using M5x.DEC.Schema.Common;
using M5x.DEC.Schema.Utils;

namespace Robby.Game.Schema
{
    public interface IGame : IStateEntity<Game.ID>
    {
    }

    [Serializable]
    [DbName(Attributes.DbName)]
    public record Game : IGame, IPayload
    {
        
        
        [IDPrefix(Attributes.IDPrefix)]
        public record ID : Identity<ID>
        {
            public ID(string value) : base(value)
            {
            }

            public ID() : base(Identity<ID>.New.Value)
            {
            }

            public static ID New(string id=null)
            {
                if(string.IsNullOrWhiteSpace(id)) 
                    id = GuidUtils.LowerCaseGuid;
                return NewComb(id);
            }




        }

        [Flags]
        public enum Flags
        {
            Unknown = 0,
            Initialized = 1,
            DimensionsUpdated = 2,
            PopulationUpdated = 4,
            DescriptionUpdated = 8,
            Started = 16,
        }

        
        
        public static class Attributes
        {
            public const string IDPrefix = "robbygame";
            public const string DbName = "robby-game-db";
        }

       
        public Game() {}

        public Game(Population population, Description description, Flags flags, string id,
            string prev, string aggregateId, Vector dimensions)
        {
            Population = population;
            Description = description;
            Status = flags;
            Id = id;
            Prev = prev;
            Dimensions = dimensions;
            Meta = AggregateInfo.New(aggregateId,-1,(int)Flags.Unknown);
        }

        private Game(string aggregateId)
        {
            Population = new Population();
            Description = new Description("New Simulation", "");
            Status = Flags.Unknown;
            Id = aggregateId;
            Prev = null;
            Dimensions = new Vector(0, 0, 0);
            Meta = AggregateInfo.New(aggregateId,-1,(int)Flags.Unknown);
        }

        public Game(ID gameID, Population population, Description description,
            Flags flags, string id, string prev, AggregateInfo meta, Vector dimensions)
        {
            Population = population;
            Description = description;
            Status = flags;
            Id = id;
            Prev = prev;
            Meta = meta;
            Dimensions = dimensions;
        }

        public Game(Population population, Description description, Flags flags, string id,
            string prev, AggregateInfo meta, Vector dimensions)
        {
            Population = population;
            Description = description;
            Status = flags;
            Id = id;
            Prev = prev;
            Meta = meta;
            Dimensions = dimensions;
        }

        public Population Population { get; set; }
        public Description Description { get; set; }
        public Flags Status { get; set; }
        public Vector Dimensions { get; set; }
        public string Id { get; set; }
        public string Prev { get; set; }
        public AggregateInfo Meta { get; set; }
        public StartOrder StartOrder { get; set; }
        public static Game New(string idValue, InitializationOrder initializationOrder = null)
        {
            return new(idValue);
        }
    }

    public static class FlagExtensions
    {
        public static bool HasFlagFast(this Game.Flags value, Game.Flags flag)
        {
            return (value & flag) != 0;
        }
        
    }
}