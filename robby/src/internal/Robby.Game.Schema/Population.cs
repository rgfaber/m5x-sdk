using System.Collections.Generic;
using System.Linq;
using M5x.DEC.Schema;

namespace Robby.Game.Schema
{
    public record Population: IPayload
    {
        public Population(IEnumerable<Competitor> competitors)
        {
            Competitors = competitors;
        }

        public Population()
        {
        }

        public IEnumerable<Competitor> Competitors { get; set; }

        public static Population New(int count, M5x.DEC.Schema.Common.Vector maxDimensions)
        {
            var lst = new List<Competitor>();
            while (lst.Count < count)
            {
                var robot = Competitor.New( maxDimensions);
                
                if (lst.All(x => x.Description.Name != robot.Description.Name))
                    lst.Add(robot);
            }
            return new Population(lst);
        }
    }
}