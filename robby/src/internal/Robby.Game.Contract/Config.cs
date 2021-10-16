namespace Robby.Game.Contract
{
    public static class Config
    {
        public static class Hopes
        {
            public const string Initialize = "Robby.Game.Initialize";
            public const string UpdateDescription = "Robby.Game.UpdateDescription";
            public const string UpdateDimensions = "Robby.Game.UpdateDimensions";
            public const string UpdatePopulation = "Robby.Game.UpdatePopulation";
            public const string Start = "Robby.Game.Start";
            public const string End = "Robby.Game.End";
        }


        public static class Facts
        {
            public const string Initialized = "Robby.Game.Initialized";
            public const string DescriptionUpdated = "Robby.Game.DescriptionUpdated";
            public const string DimensionsUpdated = "Robby.Game.DimensionsUpdated";
            public const string PopulationUpdated = "Robby.Game.PopulationUpdated";
            public const string Started = "Robby.Game.Started";
            public const string Ended = "Robby.Game.Ended";
        }


        public static class Endpoints
        {
            public const string ById = "/game/by-id";
            public const string First20 = "/game/first-20";
            public const string Initialize = "/game/initialize";
            public const string Start = "/game/start";
            public const string End = "/game/end";
        }
        
        
        
        
    }
}