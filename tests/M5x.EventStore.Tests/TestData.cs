using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using AutoBogus;
using Bogus;
using EventStore.Client;
using M5x.Testing;

namespace M5x.EventStore.Tests;

public static class TestData
{
    public const string StreamName = TestConstants.Id;
    public const string GroupName = TestConstants.GroupName;

    public static readonly MyPayload TestPayload = new(TestConstants.Id, TestConstants.GroupName);

    public static string MyFactType => typeof(MyFact).AssemblyQualifiedName;

    public static EventData EventData(Guid eventId)
    {
        return new EventData(
            Uuid.FromGuid(eventId),
            MyFactType,
            JsonSerializer.SerializeToUtf8Bytes(TestPayload));
    }


    public record MyFact
    {
        [JsonConstructor]
        public MyFact(string id, byte[] payload)
        {
            Id = id;
            Payload = payload;
        }


        public string Id { get; }
        public byte[] Payload { get; }
    }

    public record MyPayload
    {
        [JsonConstructor]
        public MyPayload(string id, string name)
        {
            Id = id;
            Name = name;
        }

        public string Id { get; }
        public string Name { get; }
    }


    public static class Fakers
    {
        public static Faker<MyFact> MyFact => new AutoFaker<MyFact>()
            .RuleFor(f => f.Id, () => TestConstants.Id)
            .RuleFor(f => f.Payload, () => JsonSerializer.SerializeToUtf8Bytes(TestPayload));
    }
}