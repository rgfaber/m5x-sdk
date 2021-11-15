using System;
using M5x.DEC.Schema;

namespace M5x.DEC.TestKit.Tests.SuT
{
    public record MyPayload : IPayload
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime Birthday { get; set; }
    }
}