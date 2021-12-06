using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using M5x.DEC;
using M5x.DEC.Events;
using M5x.DEC.ExecutionResults;
using M5x.DEC.Persistence;
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
                Model.Status = (Schema.GameModel.Flags) evt.Meta.Status;
                Model.Meta = evt.Meta;
            }

            public IExecutionResult Execute(UpdateDimensions.Cmd command)
            {
                try
                {
                    if (command == null) throw new ArgumentNullException(nameof(command));
                    if (command.Payload == null) throw new UpdateDimensions.Excep($"Dimensions must not be nil in command {command.GetType()}");
                    if (!Model.Status.HasFlag(Schema.GameModel.Flags.Initialized))
                        throw new UpdatePopulation.Excep("Context is not initialized");
                    if (Model.Status.HasFlag(Schema.GameModel.Flags.DimensionsUpdated))
                        throw new UpdateDimensions.Excep("Simultation Dimensions are already set");
                    var newStatus = Model.Status | Schema.GameModel.Flags.DimensionsUpdated;
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
                .AddTransient<IGameModel, Schema.GameModel>()
                .AddTransient<Aggregate.IRoot, Aggregate.Root>()
                .AddTransient<IActor, Actor>();
        }

        public interface IActor : IAsyncActor<
            Schema.GameModel.ID, 
            Cmd,
            Contract.Features.UpdateDimensions.Fbk>
        {
        }


        internal class Actor : AsyncActor<Aggregate.Root, 
            Schema.GameModel.ID, 
            Cmd,
            Contract.Features.UpdateDimensions.Fbk>, IActor
        {
            protected override async Task<Contract.Features.UpdateDimensions.Fbk> Act(Cmd cmd)
            {
                var rsp = Contract.Features.UpdateDimensions.Fbk.Empty(cmd.CorrelationId);
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
                }
                return rsp;
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

        public interface IEmitter : IFactEmitter<GameModel.ID, 
            Evt, 
            Contract.Features.UpdateDescription.Fact>
        {
        }

        public record Cmd : Aggregate.Cmd<Vector>
        {
            public Cmd()
            {
            }

            public Cmd(Schema.GameModel.ID aggregateId, string correlationId, Vector payload) : base(aggregateId, correlationId, payload)
            {
            }

            public static Cmd New(Domain.Initialize.Evt @event)
            {
                var gameId = Schema.GameModel.ID.With(@event.Meta.Id);
                return new(gameId, @event.CorrelationId, @event.Payload.FieldDimensions);
            }

            public static Cmd New(Schema.GameModel.ID id, string correlationId, Vector payload)
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

            public static Evt New(Cmd command, Schema.GameModel.Flags newGameStatus)
            {
                return new(
                    AggregateInfo.New(command.AggregateId.Value,  -1, (int)newGameStatus), 
                    command.CorrelationId, 
                    command.Payload);
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