using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using M5x.DEC;
using M5x.DEC.Events;
using M5x.DEC.ExecutionResults;
using M5x.DEC.PubSub;
using M5x.DEC.Schema;
using M5x.DEC.Schema.Common;
using M5x.DEC.Schema.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Robby.Game.Schema;
using Serilog;

namespace Robby.Game.Domain
{
    /// <summary>
    ///     The Partial Aggregate.Root Class
    ///     We make it partial so we can separate the code into smaller readable chunks
    /// </summary>
    public static partial class Aggregate
    {
        public partial class Root
            : IExecute<UpdateDimensions.Cmd>
                , IApply<UpdateDimensions.Evt>
        {
            public void Apply(UpdateDimensions.Evt evt)
            {
                Model.Dimensions = evt.Payload;
                Model.Status = (Schema.Game.Flags) evt.Meta.Status;
                Model.Meta = evt.Meta;
            }

            public IExecutionResult Execute(UpdateDimensions.Cmd command)
            {
                try
                {
                    if (command == null) throw new ArgumentNullException(nameof(command));
                    if (command.Payload == null) throw new UpdateDimensions.Excep($"Dimensions must not be nil in command {command.GetType()}");
                    if (!Model.Status.HasFlag(Schema.Game.Flags.Initialized))
                        throw new UpdatePopulation.Excep("Context is not initialized");
                    if (Model.Status.HasFlag(Schema.Game.Flags.DimensionsUpdated))
                        throw new UpdateDimensions.Excep("Simultation Dimensions are already set");
                    var newStatus = Model.Status | Schema.Game.Flags.DimensionsUpdated;
                    // TODO Factor this out
                    Model.Status = newStatus;
                    RaiseEvent(UpdateDimensions.Evt.New(command, newStatus));
                    return ExecutionResult.Success();
                }
                catch (Exception e)
                {
                    Log.Error(e.InnerAndOuter());
                    return ExecutionResult.Failed(e.InnerAndOuter());
                }
            }
        }
    }

    /// <summary>
    ///     The static Feature Class
    /// </summary>
    public static class UpdateDimensions
    {
        public static IServiceCollection AddUpdateDimensionsActor(this IServiceCollection services)
        {
            return services
                .AddDECBus()
                .AddTransient<IGame, Schema.Game>()
                .AddTransient<Aggregate.IRoot, Aggregate.Root>()
                .AddTransient<IEventHandler<Schema.Game.ID, Domain.Initialize.Evt>,Initialize.Handler>()
                .AddTransient<IActor, Actor>();
        }

        public interface IActor : IAsyncActor<
            Aggregate.Root, 
            Schema.Game.ID, 
            Cmd,
            Contract.Features.UpdateDimensions.Hope,
            Contract.Features.UpdateDimensions.Feedback>
        {
        }


        internal class Actor : AsyncActor<
            Aggregate.Root, 
            Schema.Game.ID, Cmd,
            Contract.Features.UpdateDimensions.Hope,
            Contract.Features.UpdateDimensions.Feedback, 
            Evt,
            Contract.Features.UpdateDimensions.Fact>, IActor
        {
            public Actor(IGameStream gameStream,
                IDECBus bus,
                IEnumerable<IEventHandler<Schema.Game.ID, Evt>> handlers,
                IEmitter emitter,
                ILogger logger) : base(gameStream,
                bus,
                handlers,
                emitter,
                logger)
            {
            }

            protected override Cmd ToCommand(Contract.Features.UpdateDimensions.Hope hope)
            {
                return Cmd.New(Schema.Game.ID.With(hope.AggregateId), hope.CorrelationId, hope.Payload);
            }

            protected override async Task<Contract.Features.UpdateDimensions.Feedback> Act(Cmd cmd)
            {
                var rsp = Contract.Features.UpdateDimensions.Feedback.Empty(cmd.CorrelationId);
                try
                {
                    var root = await Aggregates.GetByIdAsync(cmd.AggregateId);
                    if (root == null) throw new KeyNotFoundException($"Aggregate [{cmd.AggregateId}] was not found");
                    var res = root.Execute(cmd);
                    if (!res.IsSuccess) throw new Excep($"Command {cmd.GetType()} was not Succesful");
                    rsp.Meta = root.Model.Meta;
                    Aggregates.SaveAsync(root).Wait();
                }
                catch (Exception e)
                {
                    rsp.ErrorState.Errors.Add(Constants.GameErrors.DomainError, e.AsApiError());
                    Logger?.Error(e.InnerAndOuter());
                }
                return rsp;
            }

            protected override Contract.Features.UpdateDimensions.Fact ToFact(Evt @event)
            {
                return Contract.Features.UpdateDimensions.Fact.New(@event.Meta, @event.CorrelationId, @event.Payload);
            }
        }

        public interface IEmitter : IFactEmitter<Schema.Game.ID, Contract.Features.UpdateDimensions.Fact>
        {
        }

        public record Cmd : Aggregate.Cmd<Vector>
        {
            public Cmd()
            {
            }

            public Cmd(Schema.Game.ID aggregateId, string correlationId, Vector payload) : base(aggregateId, correlationId, payload)
            {
            }

            public static Cmd New(Domain.Initialize.Evt @event)
            {
                var gameId = Schema.Game.ID.With(@event.Meta.Id);
                return new(gameId, @event.CorrelationId, @event.Payload.FieldDimensions);
            }

            public static Cmd New(Schema.Game.ID id, string correlationId, Vector payload)
            {
                return new Cmd(id, correlationId, payload);
            }
        }

        public record Evt : Aggregate.Evt<Vector>
        {
            public Evt(AggregateInfo meta, string correlationId, Vector payload) : base(meta, correlationId, payload)
            {
            }

            public Evt()
            {
            }


            public Evt(AggregateInfo meta, Vector payload) : base(meta, payload)
            {
            }

            public static Evt New(Cmd command, Schema.Game.Flags newGameStatus)
            {
                return new(
                    AggregateInfo.New(command.AggregateId.Value,  -1, (int)newGameStatus), 
                    command.CorrelationId, 
                    command.Payload);
            }

            public override IEvent<Schema.Game.ID> WithAggregate(AggregateInfo meta)
            {
                return new Evt(meta, Payload);
            }
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
                    var hope = Contract.Features.UpdateDimensions.Hope.New(@event.Meta.Id, @event.CorrelationId,
                        @event.Payload.FieldDimensions);
                    var feedback = await _actor.HandleAsync(hope);
                    if (!feedback.IsSuccess)
                        Log.Debug($"{GetType()} failed: {feedback}");
                }
            }
        }
    }
}