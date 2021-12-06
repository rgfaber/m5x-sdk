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
                Model.Status |= Schema.GameModel.Flags.PopulationUpdated;
                Model.Population = evt.Payload;
            }

            public IExecutionResult Execute(UpdatePopulation.Cmd command)
            {
                try
                {
                    if (command == null) throw new ArgumentNullException(nameof(command));
                    if (command.Payload == null) throw new UpdatePopulation.Excep("Command Payload cannot be nil");
                    if (!Model.Status.HasFlag(Schema.GameModel.Flags.Initialized)) throw new UpdatePopulation.Excep("Context is not initialized");
                    var newStatus = Model.Status | Schema.GameModel.Flags.PopulationUpdated;
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
                .AddTransient<IGameModel, Schema.GameModel>()
                .AddTransient<Aggregate.IRoot, Aggregate.Root>()
                .AddTransient<IActor, Actor>();
        }



        public interface IActor : IAsyncActor<
            Schema.GameModel.ID,
            Cmd,
            Contract.Features.UpdatePopulation.Fbk>
        {
        }

        internal class Actor : AsyncActor<
            Aggregate.Root,
            Schema.GameModel.ID,
            Cmd,
            Contract.Features.UpdatePopulation.Fbk>, IActor
        {


            protected override async Task<Contract.Features.UpdatePopulation.Fbk> Act(Cmd cmd)
            {
                var feedback =
                    Contract.Features.UpdatePopulation.Fbk.Empty(cmd.CorrelationId);
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
                }
                return feedback;
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


        public interface IEmitter : IFactEmitter<
            Schema.GameModel.ID, 
            Domain.UpdatePopulation.Evt,
            Contract.Features.UpdatePopulation.Fact>
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

            public Cmd(Schema.GameModel.ID aggregateId, string correlationId, Population payload) : base(aggregateId, correlationId, payload)
            {
            }

            public static Cmd New(Domain.Initialize.Evt @event)
            {
                var gameId = Schema.GameModel.ID.With(@event.Meta.Id);
                return new Cmd(gameId, 
                    @event.CorrelationId, 
                    Population.New(@event.Payload.NumberOfRobots, @event.Payload.FieldDimensions));
            }

            public static Cmd New(Schema.GameModel.ID id, string correlationId, Population payload)
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

            public static Evt New(Cmd command, Schema.GameModel.Flags newStatus)
            {
                return new Evt(
                    AggregateInfo.New(command.AggregateId.Value, -1, (int)newStatus),
                    command.CorrelationId,
                    command.Payload);
            }

            public override IEvent<GameModel.ID> WithAggregate(AggregateInfo meta, string correlationId)
            {
                return new Evt(meta, correlationId, Payload);
            }
        }
    }
}