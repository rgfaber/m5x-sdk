using System.Collections.Generic;
using System.Linq;
using M5x.DEC.Schema;

namespace Robby.Game.Schema
{
    public record Population: IPayload
    {
        public Population(IEnumerable<Robot> robots)
        {
            Robots = robots;
        }

        public Population()
        {
        }

        public IEnumerable<Robot> Robots { get; set; }

        public static Population New(int count, M5x.DEC.Schema.Common.Vector maxDimensions)
        {
            var lst = new List<Robot>();
            while (lst.Count < count)
            {
                var robot = Robot.New(maxDimensions);
                
                if (lst.All(x => x.Description.Name != robot.Description.Name))
                    lst.Add(robot);
            }
            return new Population(lst);
        }
    }
}