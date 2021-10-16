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
        : IExecute<Start.Cmd>
        , IApply<Start.Evt>
        {
            public IExecutionResult Execute(Start.Cmd command)
            {
                if (command == null) throw new ArgumentNullException(nameof(command));
                if (command.Payload == null) throw new Start.Excep("Payload Cannot be nil");
                
                if (!Model.Status.HasFlag(Schema.Game.Flags.DimensionsUpdated) ||
                    !Model.Status.HasFlag(Schema.Game.Flags.PopulationUpdated)) 
                    return ExecutionResult.Failed();
                var newStatus = Model.Status | Schema.Game.Flags.Started;
                RaiseEvent(Start.Evt.New(command, newStatus));
                return ExecutionResult.Success();
            }

            public void Apply(Start.Evt evt)
            {
                Model.Status = (Schema.Game.Flags) evt.Meta.Status;
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
                .AddTransient<IGame, Schema.Game>()
                .AddTransient<Aggregate.IRoot, Aggregate.Root>()
                .AddTransient<IActor, Actor>();
        }

        public interface IEmitter : IFactEmitter<Schema.Game.ID, Contract.Features.Start.Fact>
        {}

        public record Cmd : Aggregate.Cmd<StartOrder>
        {
            public Cmd()
            {
            }

            public Cmd(Schema.Game.ID aggregateId, string correlationId, StartOrder payload) 
                : base(aggregateId, correlationId, payload)
            {
            }

            public static Cmd New(Schema.Game.ID id, string correlationId, StartOrder payload)
            {
                return new(id, correlationId, payload);
            }
        }

        public record Evt : Aggregate.Evt<StartOrder>
        {
            public Evt() {}
            public static Evt New(Cmd command, Schema.Game.Flags newStatus)
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

            public Evt(AggregateInfo meta, string correlationId, StartOrder payload) : base(meta, correlationId, payload)
            {
            }

            public Evt(AggregateInfo meta, StartOrder payload) : base(meta, payload)
            {
            }

            public override IEvent<Schema.Game.ID> WithAggregate(AggregateInfo meta)
            {
                return new Evt(meta, Payload);
            }
        }
        public interface IActor : IAsyncActor<Aggregate.Root, Schema.Game.ID, Cmd, Contract.Features.Start.Hope,
            Contract.Features.Start.Feedback>
        { }
        
        
        internal class Actor : AsyncActor<Aggregate.Root, Schema.Game.ID, Cmd,
                Contract.Features.Start.Hope,
                Contract.Features.Start.Feedback, 
                Evt, Contract.Features.Start.Fact>, IActor
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

            protected override Cmd ToCommand(Contract.Features.Start.Hope hope)
            {
                return Cmd.New(Schema.Game.ID.With(hope.AggregateId), hope.CorrelationId, hope.Payload);
            }

            protected override async Task<Contract.Features.Start.Feedback> Act(Cmd cmd)
            {
                var rsp = Contract.Features.Start.Feedback.Empty(cmd.CorrelationId);
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
                    Logger?.Error(e.InnerAndOuter());
                }
                return rsp;
            }

            private static class Errors
            {
                public const string ActorError = "StartSimulation.ActorError";
            }

            protected override Contract.Features.Start.Fact ToFact(Evt @event)
            {
                return Contract.Features.Start.Fact.New(@event.Meta, @event.CorrelationId, @event.Payload);
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