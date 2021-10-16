using System;
using System.Runtime.Serialization;

namespace M5x.Publisher.Contract
{
    public enum LocationEnum
    {
        [EnumMember(Value = "Unknown")] Unknown = 0,
        [EnumMember(Value="London")] London = 1,
        [EnumMember(Value="Berlin")] Berlin = 2,
        [EnumMember(Value="Paris")] Paris = 3,
        [EnumMember(Value="Madrid")] Madrid = 4,
        [EnumMember(Value="Amsterdam")] Amsterdam = 5,
        [EnumMember(Value="Brussels")] Brussels = 6,
        [EnumMember(Value="Warsaw")] Warsaw = 7
    }

    public enum NameEnum
    {
        [EnumMember(Value="Unknown")] Unknown=0,
        [EnumMember(Value="John")] John=1,
        [EnumMember(Value="Paul")] Paul=2,
        [EnumMember(Value="George")] George=3,
        [EnumMember(Value="Ringo")] Ringo=4,        
    }
    
    public class TestPayload
    {
        public NameEnum Name { get; set; }
        public DateTime ScheduledAt { get; set; }
        public LocationEnum Location { get; set; }

        public static TestPayload CreateRandom()
        {
            var nameRand = new Random().Next(0, 4);
            var locRand = new Random().Next(0, 7);
            return new TestPayload()
            {
                Location = (LocationEnum) locRand,
                Name = (NameEnum) nameRand,
                ScheduledAt = DateTime.UtcNow
            };
        }
    }
    
    
}