using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
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
        public partial class Root :
            IExecute<Initialize.Cmd>,
            IApply<Initialize.Evt>
        {
            public void Apply(Initialize.Evt evt)
            {
                Model.Status = (Schema.Game.Flags) evt.Meta.Status;
                Model.Meta = evt.Meta;
                Model.Id = evt.Meta.Id;
                Model.Description.Name = evt.Payload.Name;
            }

            private void Validate(Initialize.Cmd command)
            {
                command.Validate();
                if (command == null) throw new ArgumentNullException("Command should not be nil", ex);
                if (command.Payload == null) throw new Initialize.Excep("Order cannot be nil", ex);
                if (command.Payload.FieldDimensions == null) throw new Initialize.Excep("FieldDimensions cannot be nil", ex);
                if (string.IsNullOrEmpty(command.Payload.Name)) throw new Initialize.Excep("Name cannot be Empty", ex);
                if ((Schema.Game.Flags) Model.Meta.Status != Schema.Game.Flags.Unknown)
                    return ExecutionResult.Failed();
                
                return ex;
            }

            public IExecutionResult Execute(Initialize.Cmd command)
            {
                try
                {
                    Validate(command);
                    RaiseEvents(cmd);
                    return ExecutionResult.Success();
                }
                catch (Exception e)
                {
                    return ExecutionResult.Failed(e.InnerAndOuter());
                }
            }
        }
    }

    public static class Initialize
    {
        
        public static IServiceCollection AddInitializeActor(this IServiceCollection services)
        {
            return services
                .AddDECBus()
                .AddTransient<IGame, Schema.Game>()
                .AddTransient<Aggregate.IRoot, Aggregate.Root>()
                .AddTransient<IActor, Actor>();
        }

        public interface IEmitter : IFactEmitter<Schema.Game.ID, Contract.Features.Initialize.Fact>
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

            private Cmd(Schema.Game.ID aggregateId, string correlationId, InitializationOrder payload) : base(aggregateId, correlationId, payload)
            {
            }

            public static Cmd New(Schema.Game.ID gameId, string correlationId, InitializationOrder order)
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

            public static Evt New(Cmd command, Schema.Game.Flags newGameStatus)
            {
                return new(AggregateInfo.New(
                    command.AggregateId.Value,
                    -1,
                    (int)newGameStatus), command.CorrelationId, command.Payload);
            }


            private Evt(AggregateInfo meta, string correlationId, InitializationOrder payload) : base(meta, correlationId, payload)
            {
            }

            public override IEvent<Schema.Game.ID> WithAggregate(AggregateInfo meta)
            {
                return new Evt(meta, Payload);
            }
        }

        private static class Errors
        {
            public const string ActorError = "Robby.InitializeContext.ActorError";
        }

        public interface IActor : IAsyncActor<Aggregate.Root, Schema.Game.ID, Cmd,
            Contract.Features.Initialize.Hope,
            Contract.Features.Initialize.Feedback>
        {
        }

        internal class Actor : AsyncActor<Aggregate.Root, Schema.Game.ID, Cmd,
            Contract.Features.Initialize.Hope,
            Contract.Features.Initialize.Feedback, 
            Evt, Contract.Features.Initialize.Fact>, IActor
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

            protected override Cmd ToCommand(Contract.Features.Initialize.Hope hope)
            {
                return Cmd.New(Schema.Game.ID.With(hope.AggregateId), hope.CorrelationId, hope.Payload);
            }

            protected override async Task<Contract.Features.Initialize.Feedback> Act(Cmd cmd)
            {
                var res = Contract.Features.Initialize.Feedback.Empty(cmd.CorrelationId);
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
                    Logger.Error(e.InnerAndOuter());
                }
                return res;
            }

            protected override Contract.Features.Initialize.Fact ToFact(Evt @event)
            {
                return Contract.Features.Initialize.Fact.New(@event.Meta, @event.CorrelationId, @event.Payload);
            }
        }
    }
}