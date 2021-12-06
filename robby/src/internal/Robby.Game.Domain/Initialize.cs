using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using FluentValidation;
using M5x.DEC;
using M5x.DEC.Events;
using M5x.DEC.ExecutionResults;
using M5x.DEC.Persistence;
using M5x.DEC.PubSub;
using M5x.DEC.Schema;
using M5x.DEC.Schema.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Robby.Game.Schema;

namespace Robby.Game.Domain
{
    public static partial class Aggregate
    {
        public partial class Root :
            IExecute<Initialize.Cmd>,
            IApply<Initialize.Evt>
        {
            public void Apply(Initialize.Evt evt)
            {
                Model.Status = (Schema.GameModel.Flags) evt.Meta.Status;
                Model.Meta = evt.Meta;
                Model.Id = evt.Meta.Id;
                Model.Description.Name = evt.Payload.Name;
            }

            private void Validate(Initialize.Cmd command)
            {
                command.Validate();
                Guard.Against.Null(command, nameof(command));
                Guard.Against.Null(command.Payload, nameof(command.Payload));
                Guard.Against.Null(command.Payload.FieldDimensions, nameof(command.Payload.FieldDimensions));
                Guard.Against.NullOrWhiteSpace(command.Payload.Name, nameof(command.Payload.Name));
                Guard.Against.AlreadyInitialized(Model);
            }

            public IExecutionResult Execute(Initialize.Cmd command)
            {
                try
                {
                    Validate(command);
                    RaiseEvents(command);
                    return ExecutionResult.Success();
                }
                catch (Exception e)
                {
                    return ExecutionResult.Failed(e.InnerAndOuter());
                }
            }

            private void RaiseEvents(Initialize.Cmd cmd)
            {
            }
        }
    }

    public static class Initialize
    {

        public static void AlreadyInitialized(this IGuardClause guardClause, Schema.GameModel model)
        {
            if (model.Status != Schema.GameModel.Flags.Unknown)
                throw new Initialize.Excep($"Game [{model.Id}] is already initialized.");
        }

        public static IServiceCollection AddInitializeActor(this IServiceCollection services)
        {
            return services
                .AddDECBus()
                .AddTransient<IGameModel, Schema.GameModel>()
                .AddTransient<Aggregate.IRoot, Aggregate.Root>()
                .AddTransient<IActor, Actor>();
        }

        public interface IEmitter : IFactEmitter<Schema.GameModel.ID, Evt, Contract.Features.Initialize.Fact>
        {
        }

        public class Excep : Exception
        {
            public Excep()
            {
            }

            protected Excep(SerializationInfo info, StreamingContext context) : base(info, context)
            {
            }

            public Excep(string message) : base(message)
            {
            }

            public Excep(string message, Exception innerException) : base(message, innerException)
            {
            }
        }

        public static void Validate(this Cmd command)
        {
            var v = new Cmd.Validator();
            var res = v.Validate(command);
            if (res.IsValid) return;
            var s = string.Join(';', res.Errors);
            throw new Excep(s);
        }

        public record Cmd : Aggregate.Cmd<InitializationOrder>
        {
            public class Validator : AbstractValidator<Cmd>
            {
                public Validator()
                {
                    RuleFor(cmd => cmd).NotNull();
                    RuleFor(cmd => cmd.Payload).NotNull();
                    RuleFor(cmd => cmd.AggregateId).NotNull();
                }
            }

            public Cmd()
            {
            }

            private Cmd(Schema.GameModel.ID aggregateId, string correlationId, InitializationOrder payload) : base(aggregateId, correlationId, payload)
            {
            }

            public static Cmd New(Schema.GameModel.ID gameId, string correlationId, InitializationOrder order)
            {
                return new(gameId, correlationId, order);
            }
        }

        public record Evt : Aggregate.Evt<InitializationOrder>
        {
            public Evt() {}
            private Evt(AggregateInfo meta, InitializationOrder payload) : base(meta, payload)
            {
            }
            private Evt(AggregateInfo meta, string correlationId, InitializationOrder payload) : base(meta, correlationId, payload)
            {
            }
            public override IEvent<GameModel.ID> WithAggregate(AggregateInfo meta, string correlationId)
            {
                return new Evt(meta, correlationId, Payload);
            }
        }

        private static class Errors
        {
            public const string ActorError = "Robby.InitializeContext.ActorError";
        }

        public interface IActor : IAsyncActor<Schema.GameModel.ID, 
            Cmd,
            Contract.Features.Initialize.Fbk>
        {
        }

        internal class Actor : AsyncActor<
            Aggregate.Root,
            GameModel.ID, 
            Cmd,
            Contract.Features.Initialize.Fbk>, IActor
        {


            protected override async Task<Contract.Features.Initialize.Fbk> Act(Cmd cmd)
            {
                var res = Contract.Features.Initialize.Fbk.Empty(cmd.CorrelationId);
                try
                {
                    var root = await Aggregates.GetByIdAsync(cmd.AggregateId);
                    if (root == null) throw new KeyNotFoundException($"Stream [{cmd.AggregateId}] was not found");
                    var result = root.Execute(cmd);
                    if (result.IsSuccess)
                    {
                        Aggregates.SaveAsync(root).Wait();
                        res.Meta = root.Model.Meta;
                    }
                }
                catch (Exception e)
                {
                    res.ErrorState.Errors.Add(Errors.ActorError, e.AsApiError());
                }
                return res;
            }
           

            public Actor(IBroadcaster<GameModel.ID> caster, IAsyncEventStream<Aggregate.Root, GameModel.ID> aggregates, IDECBus bus, IEnumerable<IEventHandler<GameModel.ID, IEvent<GameModel.ID>>> handlers) : base(caster, aggregates, bus, handlers)
            {
            }
        }
    }
}