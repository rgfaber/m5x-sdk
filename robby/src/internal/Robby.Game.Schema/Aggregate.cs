using M5x.DEC.Schema;

namespace Robby.Game.Schema
{
    public static class Aggregate
    {


        [IDPrefix(Constants.RobotAttributes.IDPrefix)]
        public record RobotId : Identity<RobotId>
        {
            public RobotId(string value) : base(value)
            {
            }
            
            public RobotId() : base(Identity<RobotId>.New.Value) {}

            public static RobotId New(string id)
            {
                return With(id);
            }
            
            
        }
    }
}