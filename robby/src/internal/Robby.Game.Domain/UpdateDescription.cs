using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Web;
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
    public static partial class Aggregate
    {
        public partial class Root
            : IApply<UpdateDescription.Evt>
                , IExecute<UpdateDescription.Cmd>
        {
            public void Apply(UpdateDescription.Evt evt)
            {
                Model.Description = evt.Payload;
                Model.Status = (Schema.Game.Flags) evt.Meta.Status;
                Model.Meta = evt.Meta;
            }

            public IExecutionResult Execute(UpdateDescription.Cmd command)
            {
                try
                {
                    if (command == null) throw new ArgumentNullException(nameof(command));
                    if (command.Payload == null) throw new UpdateDescription.Excep("Description cannot be nil");
                    if (string.IsNullOrEmpty(command.Payload.Name))
                        throw new UpdateDescription.Excep("Description Name cannot be empty.");
                    if (Model.Description.Name == command.Payload.Name)
                        throw new UpdateDescription.Excep("Description Name should be different.");
                    if (!Model.Status.HasFlag(Schema.Game.Flags.Initialized))
                        throw new UpdateDescription.Excep("Model is not Initialized.");
                    var newStatus = Model.Status | Schema.Game.Flags.DescriptionUpdated;
                    // TODO Factor this out
                    Model.Status = newStatus;
                    RaiseEvent(UpdateDescription.Evt.New(command, newStatus));
                    return ExecutionResult.Success();
                }
                catch (Exception e)
                {
                    return ExecutionResult.Failed(e.InnerAndOuter());
                }
            }
        }
    }


    public static class UpdateDescription
    {
        public static IServiceCollection AddUpdateDescriptionActor(this IServiceCollection services)
        {
            return services
                .AddDECBus()
                .AddTransient<IGame, Schema.Game>()
                .AddTransient<Aggregate.IRoot, Aggregate.Root>()
                .AddTransient<IEventHandler<Schema.Game.ID, Domain.Initialize.Evt>, Initialize.Handler>()
                .AddTransient<IActor, Actor>();
        }

        public interface IEmitter : IFactEmitter<Schema.Game.ID, Contract.Features.UpdateDescription.Fact>
        {
        }

        public interface IActor : IAsyncActor<Aggregate.Root, Schema.Game.ID, 
            Cmd,
            Contract.Features.UpdateDescription.Hope,
            Contract.Features.UpdateDescription.Feedback>
        {
        }

        internal class Actor : AsyncActor<
            Aggregate.Root, 
            Schema.Game.ID, 
            Cmd,
            Contract.Features.UpdateDescription.Hope,
            Contract.Features.UpdateDescription.Feedback, 
            Evt,
            Contract.Features.UpdateDescription.Fact>, IActor
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

            protected override Cmd ToCommand(Contract.Features.UpdateDescription.Hope hope)
            {
                return Cmd.New(Schema.Game.ID.With(hope.AggregateId), hope.CorrelationId, hope.Payload);
            }

            protected override async Task<Contract.Features.UpdateDescription.Feedback> Act(Cmd cmd)
            {
                var res = Contract.Features.UpdateDescription.Feedback.Empty(cmd.CorrelationId);
                try
                {
                    var root = await Aggregates.GetByIdAsync(cmd.AggregateId);
                    var exec = root.Execute(cmd);
                    if (!exec.IsSuccess)
                        throw new Excep($"Command {cmd.GetType()} failed to execute");
                    res.Meta = root.Model.Meta;
                    Aggregates.SaveAsync(root).Wait();
                    
                }
                catch (Exception e)
                {
                    res.ErrorState.Errors.Add(Errors.ActorError, e.AsApiError());
                    Log.Error(e.InnerAndOuter());
                }

                return res;
            }

            protected override Contract.Features.UpdateDescription.Fact ToFact(Evt @event)
            {
                return Contract.Features.UpdateDescription.Fact.New(@event.Meta, @event.CorrelationId, @event.Payload);
            }
        }

        public static class Errors
        {
            public const string ActorError = "UpdateDescription.ActorError";
        }


        public record Cmd : Aggregate.Cmd<Description>
        {
            public Cmd()
            {
            }

            private Cmd(Schema.Game.ID aggregateId, string correlationId, Description payload) : base(aggregateId, correlationId, payload)
            {
            }

            public static Cmd New(Contract.Features.Initialize.Fact fact)
            {
                return new(Schema.Game.ID.With(fact.Meta.Id), fact.CorrelationId, new Description(
                    fact.Payload.Name, ""));
            }

            public static Cmd New(Domain.Initialize.Evt @event)
            {
                var gameId = Schema.Game.ID.With(@event.Meta.Id);
                return new( gameId, @event.CorrelationId, Description.New(@event.Payload.Name));
            }

            public static Cmd New(Schema.Game.ID id, string correlationId, Description payload)
            {
                return new(id, correlationId, payload);
            }
        }

        public record Evt : Aggregate.Evt<Description>
        {
            private Evt(AggregateInfo meta, Description payload) : base(meta, payload)
            {
            }

            public Evt()
            {
            }

            public static Evt New(Cmd command, Schema.Game.Flags newGameStatus)
            {
                return new(
                    AggregateInfo.New(command.AggregateId.Value, -1, (int)newGameStatus), 
                    command.CorrelationId, 
                    command.Payload);
            }

            private Evt(AggregateInfo meta, string correlationId, Description payload) : base(meta, correlationId, payload)
            {
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
                    var hope = Contract.Features.UpdateDescription.Hope.New(
                        @event.Meta.Id, 
                        @event.CorrelationId,
                        @event.Payload.Name); 
                    var feedback = await _actor.HandleAsync(hope);
                    if (!feedback.IsSuccess)
                        Log.Debug($"{GetType()} failed: {feedback}");
                }
            }
        }
    }
}