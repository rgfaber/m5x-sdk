using System;

namespace M5x.DEC.Infra.Tests
{
    public record TestPayload
    {
        public TestPayload(string name, DateTime birthDay, int weight)
        {
            Name = name;
            BirthDay = birthDay;
            Weight = weight;
        }

        public string Name { get; set; }
        public DateTime BirthDay { get; set; }
        public int Weight { get; set; }
    }
}