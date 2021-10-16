using System;
using AutoBogus;
using Bogus;
using M5x.DEC.Schema.Utils;

namespace M5x.Testing
{
    public static class Fakers
    {
        public static Faker<Payload> Payload => new AutoFaker<Payload>()
            .RuleFor(p => p.Id, () => GuidUtils.NewCleanGuid)
            .RuleFor(p => p.Material, () =>
            {
                var materials = new[] { "Ore", "Coal", "Wood", "Steel" };
                return materials[new Random().Next(0, materials.Length - 1)];
            })
            .RuleFor(p => p.Arrival, () => DateTime.Now + TimeSpan.FromDays(new Random().Next(3, 65)))
            .RuleFor(p => p.Weight, () => new Random().Next(30, 7855));
    }
}