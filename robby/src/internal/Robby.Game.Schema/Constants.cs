namespace Robby.Game.Schema
{
    public static class Constants
    {

        public static class GameErrors
        {
            public const string ApiError = "Robby.Game.ApiError";
            public const string ServiceError = "Robby.Game.ServiceError";
            public const string DomainError = "Robby.Game.DomainError";
        }

        

        public static class RobotAttributes
        {
            public const string IDPrefix = "robbyrobot";
            public const string DbName = "robby-robot-db";
        }
        

        public static class RobotErrors
        {
            public const string ApiError = "Robby.Robot.ApiError";
            public const string ServiceError = "Robby.Robot.ServiceError";
            public const string DomainError = "Robby.Robot.DomainError";
        }
        

        public static class RobotStatuses
        {
            public const string Unknown = "Robby.Robot.Unknown";
        }
        
        
        
    }
}