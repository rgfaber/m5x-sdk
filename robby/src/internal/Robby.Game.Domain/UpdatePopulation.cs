using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using M5x.DEC;
using M5x.DEC.Events;
using M5x.DEC.ExecutionResults;
using M5x.DEC.PubSub;
using M5x.DEC.Schema;
using M5x.DEC.Schema.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Robby.Game.Schema;
using Serilog;

namespace Robby.Game.Domain
{
    public static partial class Aggregate
    {
        public partial class Root
            : IExecute<UpdatePopulation.Cmd>
                , IApply<UpdatePopulation.Evt>
        {
            public void Apply(UpdatePopulation.Evt evt)
            {
                Model.Status |= Schema.Game.Flags.PopulationUpdated;
                Model.Population = evt.Payload;
            }

            public IExecutionResult Execute(UpdatePopulation.Cmd command)
            {
                try
                {
                    if (command == null) throw new ArgumentNullException(nameof(command));
                    if (command.Payload == null) throw new UpdatePopulation.Excep("Command Payload cannot be nil");
                    if (!Model.Status.HasFlag(Schema.Game.Flags.Initialized)) throw new UpdatePopulation.Excep("Context is not initialized");
                    var newStatus = Model.Status | Schema.Game.Flags.PopulationUpdated;
                    // TODO Factor this out
                    Model.Status = newStatus;
                    RaiseEvent(UpdatePopulation.Evt.New(command, newStatus));
                    return ExecutionResult.Success();
                }
                catch (Exception e)
                {
                    return ExecutionResult.Failed(e.InnerAndOuter());
                }
            }
        }
    }

    public static class UpdatePopulation
    {
        public static IServiceCollection AddUpdatePopulationActor(this IServiceCollection services)
        {
            return services
                .AddDECBus()
                .AddTransient<IGame, Schema.Game>()
                .AddTransient<Aggregate.IRoot, Aggregate.Root>()
                .AddTransient<IEventHandler<Schema.Game.ID, Domain.Initialize.Evt>, Initialize.Handler>()
                .AddTransient<IActor, Actor>();
        }


        private static class Initialize
        {
            internal class Handler : IEventHandler<Schema.Game.ID, Domain.Initialize.Evt>
            {
                private readonly IActor _actor;

                public Handler(IActor actor)
                {
                    _actor = actor;
                }

                public async Task HandleAsync(Domain.Initialize.Evt @event)
                {
                    var hope = Contract.Features.UpdatePopulation.Hope.New(@event.Meta.Id, @event.CorrelationId,
                        @event.Payload.NumberOfRobots);
                    var feedback = await _actor.HandleAsync(hope);
                    if (!feedback.IsSuccess) 
                        Log.Debug($"{GetType()} failed: {feedback}");
                }
            }
        }


        public interface IActor : IAsyncActor<
            Aggregate.Root, 
            Schema.Game.ID,
            Cmd,
            Contract.Features.UpdatePopulation.Hope,
            Contract.Features.UpdatePopulation.Feedback>
        {
        }

        internal class Actor : AsyncActor<
            Aggregate.Root,
            Schema.Game.ID,
            Cmd,
            Contract.Features.UpdatePopulation.Hope,
            Contract.Features.UpdatePopulation.Feedback,
            Evt,
            Contract.Features.UpdatePopulation.Fact>, IActor
        {
            public Actor(IGameStream aggregates,
                IDECBus subscriber,
                IEnumerable<IEventHandler<Schema.Game.ID, Evt>> handlers,
                IEmitter emitter,
                ILogger logger) : base(aggregates,
                subscriber,
                handlers,
                emitter,
                logger)
            {
            }

            protected override Cmd ToCommand(Contract.Features.UpdatePopulation.Hope hope)
            {
                return Cmd.New(Schema.Game.ID.With(hope.AggregateId), hope.CorrelationId, hope.Payload);
            }

            protected override async Task<Contract.Features.UpdatePopulation.Feedback> Act(Cmd cmd)
            {
                var feedback =
                    Contract.Features.UpdatePopulation.Feedback.Empty(cmd.CorrelationId);
                try
                {
                    var root = await Aggregates.GetByIdAsync(cmd.AggregateId);
                    if (root == null)
                        throw new KeyNotFoundException($"Could not find stream [{cmd.AggregateId.Value}]");
                    var res = root.Execute(cmd);
                    if (!res.IsSuccess)
                        throw new Excep($"Execution of Command {cmd.GetType()} - [{cmd.CorrelationId}] was not successful");
                    Aggregates.SaveAsync(root).Wait();
                    feedback.Meta = root.Model.Meta;
                }
                catch (Exception e)
                {
                    feedback.ErrorState.Errors.Add(Constants.GameErrors.DomainError, e.AsApiError());
                    Logger?.Error(e.InnerAndOuter());
                }
                return feedback;
            }

            protected override Contract.Features.UpdatePopulation.Fact ToFact(Evt @event)
            {
                return Contract.Features.UpdatePopulation.Fact.New(@event.Meta, @event.CorrelationId,
                    @event.Payload);
            }
        }


        public interface IEmitter : IFactEmitter<Schema.Game.ID, Contract.Features.UpdatePopulation.Fact>
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

            public Excep(string? message) : base(message)
            {
            }

            public Excep(string? message, Exception? innerException) : base(message, innerException)
            {
            }
        }


        public record Cmd : Aggregate.Cmd<Population>
        {
            public Cmd()
            {
            }

            public Cmd(Schema.Game.ID aggregateId, string correlationId, Population payload) : base(aggregateId, correlationId, payload)
            {
            }

            public static Cmd New(Domain.Initialize.Evt @event)
            {
                var gameId = Schema.Game.ID.With(@event.Meta.Id);
                return new Cmd(gameId, 
                    @event.CorrelationId, 
                    Population.New(@event.Payload.NumberOfRobots, @event.Payload.FieldDimensions));
            }

            public static Cmd New(Schema.Game.ID id, string correlationId, Population payload)
            {
                return new Cmd(id, correlationId, payload);
            }
        }

        public record Evt : Aggregate.Evt<Population>
        {
            private Evt(AggregateInfo meta, string correlationId, Population payload) : base(meta, correlationId, payload)
            {
            }

            public Evt()
            {
            }

            private Evt(AggregateInfo meta, Population payload) : base(meta, payload)
            {
            }

            public static Evt New(Cmd command, Schema.Game.Flags newStatus)
            {
                return new(AggregateInfo.New(command.AggregateId.Value, -1, (int)newStatus),
                    command.CorrelationId,
                    command.Payload);
            }

            public override IEvent<Schema.Game.ID> WithAggregate(AggregateInfo meta)
            {
                return new Evt(meta, Payload);
            }
        }
    }
}