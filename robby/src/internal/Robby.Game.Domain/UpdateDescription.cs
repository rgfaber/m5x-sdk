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
    public static partial class Aggregate
    {
        public partial class Root
            : IApply<UpdateDescription.Evt>
                , IExecute<UpdateDescription.Cmd>
        {
            public void Apply(UpdateDescription.Evt evt)
            {
                Model.Description = evt.Payload;
                Model.Status = (Schema.GameModel.Flags) evt.Meta.Status;
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
                    if (!Model.Status.HasFlag(Schema.GameModel.Flags.Initialized))
                        throw new UpdateDescription.Excep("Model is not Initialized.");
                    var newStatus = Model.Status | Schema.GameModel.Flags.DescriptionUpdated;
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
                .AddTransient<IGameModel, Schema.GameModel>()
                .AddTransient<Aggregate.IRoot, Aggregate.Root>()
                .AddTransient<IActor, Actor>();
        }

        public interface IEmitter : IFactEmitter<
            Schema.GameModel.ID,
            Game.Domain.UpdateDescription.Evt,
            Contract.Features.UpdateDescription.Fact>
        {
        }

        public interface IActor : IAsyncActor<
            Schema.GameModel.ID, 
            Cmd,
            Contract.Features.UpdateDescription.Fbk>
        {
        }

        internal class Actor : AsyncActor<
            Aggregate.Root, 
            Schema.GameModel.ID, 
            Cmd,
            Contract.Features.UpdateDescription.Fbk>, IActor
        {
            protected override async Task<Contract.Features.UpdateDescription.Fbk> Act(Cmd cmd)
            {
                var res = Contract.Features.UpdateDescription.Fbk.Empty(cmd.CorrelationId);
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


            public Actor(IBroadcaster<GameModel.ID> caster,
                IGameStream aggregates,
                IDECBus bus,
                IEnumerable<IEventHandler<GameModel.ID, IEvent<GameModel.ID>>> handlers) : base(caster,
                aggregates,
                bus,
                handlers)
            {
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

            private Cmd(Schema.GameModel.ID aggregateId, string correlationId, Description payload) : base(aggregateId, correlationId, payload)
            {
            }

            public static Cmd New(Contract.Features.Initialize.Fact fact)
            {
                return new(Schema.GameModel.ID.With(fact.Meta.Id), fact.CorrelationId, new Description(
                    fact.Payload.Name, ""));
            }

            public static Cmd New(Domain.Initialize.Evt @event)
            {
                var gameId = Schema.GameModel.ID.With(@event.Meta.Id);
                return new( gameId, @event.CorrelationId, Description.New(@event.Payload.Name));
            }

            public static Cmd New(Schema.GameModel.ID id, string correlationId, Description payload)
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

            public static Evt New(Cmd command, Schema.GameModel.Flags newGameStatus)
            {
                return new(
                    AggregateInfo.New(command.AggregateId.Value, -1, (int)newGameStatus), 
                    command.CorrelationId, 
                    command.Payload);
            }

            private Evt(AggregateInfo meta, string correlationId, Description payload) : base(meta, correlationId, payload)
            {
            }

            public override IEvent<GameModel.ID> WithAggregate(AggregateInfo meta, string correlationId)
            {
                return new Evt(meta, correlationId, Payload);
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
    }
}