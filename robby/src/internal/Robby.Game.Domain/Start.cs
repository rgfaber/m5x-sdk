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
        : IExecute<Start.Cmd>
        , IApply<Start.Evt>
        {
            public IExecutionResult Execute(Start.Cmd command)
            {
                if (command == null) throw new ArgumentNullException(nameof(command));
                if (command.Payload == null) throw new Start.Excep("Payload Cannot be nil");
                
                if (!Model.Status.HasFlag(Schema.GameModel.Flags.DimensionsUpdated) ||
                    !Model.Status.HasFlag(Schema.GameModel.Flags.PopulationUpdated)) 
                    return ExecutionResult.Failed();
                var newStatus = Model.Status | Schema.GameModel.Flags.Started;
                RaiseEvent(Start.Evt.New(command, newStatus));
                return ExecutionResult.Success();
            }

            public void Apply(Start.Evt evt)
            {
                Model.Status = (Schema.GameModel.Flags) evt.Meta.Status;
                Model.Meta = evt.Meta;
                Model.StartOrder = evt.Payload;
            }
        }
    }
    
    
    
    
    public static class Start
    {

        public static IServiceCollection AddStartActor(this IServiceCollection services)
        {
            return services?
                .AddDECBus()
                .AddTransient<IGameModel, Schema.GameModel>()
                .AddTransient<Aggregate.IRoot, Aggregate.Root>()
                .AddTransient<IActor, Actor>();
        }

        public interface IEmitter : IFactEmitter<Schema.GameModel.ID,
            Domain.Initialize.Evt,
            Contract.Features.Start.Fact>
        {}

        public record Cmd : Aggregate.Cmd<StartOrder>
        {
            public Cmd()
            {
            }

            public Cmd(Schema.GameModel.ID aggregateId, string correlationId, StartOrder payload) 
                : base(aggregateId, correlationId, payload)
            {
            }

            public static Cmd New(Schema.GameModel.ID id, string correlationId, StartOrder payload)
            {
                return new(id, correlationId, payload);
            }
        }

        public record Evt : Aggregate.Evt<StartOrder>
        {
            public Evt() {}
            public static Evt New(Cmd command, Schema.GameModel.Flags newStatus)
            {
                return new Evt(AggregateInfo.New(
                        command.AggregateId.Value,
                        -1,
                        (int)newStatus),
                    command.CorrelationId,
                    command.Payload);
            }

            public Evt(StartOrder payload) : base(payload)
            {
            }

            private Evt(AggregateInfo meta,
                string correlationId,
                StartOrder payload) : base(meta,
                correlationId,
                payload)
            {
            }

            public Evt(AggregateInfo meta, StartOrder payload) : base(meta, payload)
            {
            }

            public override IEvent<GameModel.ID> WithAggregate(AggregateInfo meta, string correlationId)
            {
                return new Evt(meta, correlationId, Payload);
            }
        }
        public interface IActor : IAsyncActor<
            Schema.GameModel.ID, 
            Cmd,
            Contract.Features.Start.Fbk>
        { }
        
        
        internal class Actor : AsyncActor<
            Game.Domain.Aggregate.Root, 
            Schema.GameModel.ID, 
            Cmd, Contract.Features.Start.Fbk>, IActor
        {

            protected override async Task<Contract.Features.Start.Fbk> Act(Cmd cmd)
            {
                var rsp = Contract.Features.Start.Fbk.Empty(cmd.CorrelationId);
                try
                {
                    var root = await Aggregates.GetByIdAsync(cmd.AggregateId);
                    if (root == null) throw new KeyNotFoundException($"Aggregate [{cmd.AggregateId.Value}] not found");
                    var res = root.Execute(cmd);
                    if (!res.IsSuccess) throw new Excep(cmd);
                    rsp.Meta = root.Model.Meta;
                    Aggregates.SaveAsync(root).Wait();
                }
                catch (Exception e)
                {
                    rsp.ErrorState.Errors.Add(Errors.ActorError, e.AsApiError() );
                }
                return rsp;
            }

            private static class Errors
            {
                public const string ActorError = "StartSimulation.ActorError";
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

        public class Excep: Exception
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

            public Excep(Cmd cmd) : base($"Command [{cmd.GetType().PrettyPrint()}] failed to execute")
            { }

        }
        
    }
}