using System;
using System.Globalization;
using AutoBogus;
using Bogus;
using M5x.DEC.Schema;

namespace M5x.DEC.TestKit.Tests.SuT
{
    public static class MyBogus
    {
        public static class Contract
        {
            public static Faker<MyHope> Hope => new AutoFaker<MyHope>()
                .RuleFor(h => h.AggregateId, () => MyTestSchema.TestID.Value)
                .RuleFor(h => h.Payload, (() => MyTestSchema.Payload))
                .RuleFor(h => h.CorrelationId, (() => MyTestSchema.TEST_CORRELATION_ID));

            public static Faker<MyFeedback> Feedback => new AutoFaker<MyFeedback>()
                .RuleFor(f => f.Meta, () => MyTestSchema.Meta);


            public static Faker<MyPagedQry> PagedQuery => new AutoFaker<MyPagedQry>()
                .RuleFor(x => x.CorrelationId, () => MyTestSchema.TEST_CORRELATION_ID)
                .RuleFor(x => x.PageNumber, () => 1)
                .RuleFor(x => x.PageSize, () => 5);

            public static Faker<MySingletonQuery> SingletonQuery => new AutoFaker<MySingletonQuery>()
                .RuleFor(x => x.Id, () => MyTestSchema.TestID.Value)
                .RuleFor(x => x.CorrelationId, () => MyTestSchema.TEST_CORRELATION_ID);

            public static Faker<MyFact> Fact => new AutoFaker<MyFact>()
                .RuleFor(f => f.Meta, () => MyTestSchema.Meta)
                .RuleFor(f => f.Payload, () => MyTestSchema.Payload)
                .RuleFor(f => f.CorrelationId, (() => MyTestSchema.TEST_CORRELATION_ID));

        }

        public static class Schema
        {
            public static Faker<MyPayload> Payload => new AutoFaker<MyPayload>()
                .RuleFor(x => x.Id, () => MyTestSchema.TestID.Value)
                .RuleFor(x => x.Name, () => "Jane Doe")
                .RuleFor(x => x.Birthday, () => new DateTime(1985, 5, 12));

            public static Faker<MyReadModel> ReadModel => new AutoFaker<MyReadModel>()
                .RuleFor(x => x.Id, () => MyTestSchema.TestID.Value)
                .RuleFor(x => x.Content, c => Payload);

            public static Faker<AggregateInfo> Meta => new AutoFaker<AggregateInfo>()
                .RuleFor(m => m.Id, (() => MyTestSchema.TestID.Value))
                .RuleFor(m => m.Version, (() => -1))
                .RuleFor(m => m.Status, () => 0);

        }


        public static class Domain
        {
            public static Faker<MyCommand> Command => new AutoFaker<MyCommand>()
                .RuleFor(x => x.AggregateId, () => MyTestSchema.TestID)
                .RuleFor(x => x.CorrelationId, () => MyTestSchema.TEST_CORRELATION_ID)
                .RuleFor(x => x.Payload, () => MyTestSchema.Payload);
            public static Faker<MyAggregate> Aggregate => new AutoFaker<MyAggregate>()
                .RuleFor(x => x.Id, () => MyTestSchema.TestID);
        }
        
        
 


    }
}