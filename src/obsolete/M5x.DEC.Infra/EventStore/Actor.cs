using System.Collections.Generic;
using System.Threading.Tasks;
using M5x.DEC.Persistence;
using M5x.DEC.PubSub;
using M5x.Schemas;
using M5x.Schemas.Commands;
using Serilog;

namespace M5x.DEC.Infra.EventStore
{
    
    
    
    public abstract class Actor<TAggregate, TAggregateId, TCommand, TResponse, TEmittedEvent> : IActor<TAggregateId, TCommand, TResponse>
        where TAggregate : IAggregateRoot<TAggregateId>
        where TAggregateId : IAggregateID
        where TCommand: Command<TAggregateId>
        where TResponse : Response
        where TEmittedEvent : IEvent<TAggregateId>
    {
        private readonly IEnumerable<IAggregateEventHandler<TAggregateId, TEmittedEvent>> _handlers;
        protected readonly ILogger Logger;
        private readonly IAggregateSubscriber _subscriber;
        protected readonly IEventRepository<TAggregate, TAggregateId> EventRepo;
    
    
        protected Actor(IEventRepository<TAggregate, TAggregateId> eventRepo,
            IAggregateSubscriber subscriber,
            IEnumerable<IAggregateEventHandler<TAggregateId, TEmittedEvent>> handlers,
            ILogger logger)
        {
            EventRepo = eventRepo;
            _subscriber = subscriber;
            _handlers = handlers;
            Logger = logger;
        }
    
        public async Task Replay(TAggregateId id)
        {
            var events = await EventRepo.LoadAsync(id);
            foreach (var @event in events)
            {
                var ev = (TEmittedEvent) @event.Event;
                await HandleAsync(_handlers, ev);
            }
        }
    
        public async Task<TResponse> Handle(TCommand cmd)
        {
            _subscriber.Subscribe<TEmittedEvent>(async @event => await HandleAsync(_handlers, @event));
            return await Act(cmd);
        }
    
    
        protected abstract Task<TResponse> Act(TCommand cmd); 
    
        private async Task HandleAsync(IEnumerable<IAggregateEventHandler<TAggregateId, TEmittedEvent>> handlers,
            TEmittedEvent @event)
        {
            foreach (var handler in handlers) await handler.HandleAsync(@event);
        }
    }
    
    
    
    
    // public interface IActor<in TAggregateId, TRequest, TResponse>
    //     where TAggregateId : IAggregateID
    //     where TRequest:IRequest
    //     where TResponse : Response
    // {
    //     Task<TResponse> Handle(TRequest req);
    //     Task Replay(TAggregateId id);
    // }

    public interface IActor<in TAggregateId, TCommand, TResponse>
        where TAggregateId : IAggregateID
        where TCommand : ICommand
        where TResponse : Response
    {
        Task<TResponse> Handle(TCommand req);
        Task Replay(TAggregateId id);
    }

    
    
    
    
    
    // public abstract class Actor<TAggregate, TAggregateId, TRequest, TCommand, TResponse, TEmittedEvent> : IActor<TAggregateId, TRequest, TResponse>
    //     where TAggregate : IAggregateRoot<TAggregateId>
    //     where TAggregateId : IAggregateID
    //     where TCommand: Command<TAggregateId>
    //     where  TRequest : IRequest
    //     where TResponse : Response
    //     where TEmittedEvent : IEvent<TAggregateId>
    // {
    //     private readonly IEnumerable<IAggregateEventHandler<TAggregateId, TEmittedEvent>> _handlers;
    //     protected readonly ILogger Logger;
    //     private readonly IAggregateSubscriber _subscriber;
    //     protected readonly IEventRepository<TAggregate, TAggregateId> EventRepo;
    //
    //
    //     protected Actor(IEventRepository<TAggregate, TAggregateId> eventRepo,
    //         IAggregateSubscriber subscriber,
    //         IEnumerable<IAggregateEventHandler<TAggregateId, TEmittedEvent>> handlers,
    //         ILogger logger)
    //     {
    //         EventRepo = eventRepo;
    //         _subscriber = subscriber;
    //         _handlers = handlers;
    //         Logger = logger;
    //     }
    //
    //     public async Task Replay(TAggregateId id)
    //     {
    //         var events = await EventRepo.LoadAsync(id);
    //         foreach (var @event in events)
    //         {
    //             var ev = (TEmittedEvent) @event.Event;
    //             await HandleAsync(_handlers, ev);
    //         }
    //     }
    //
    //     public async Task<TResponse> Handle(TRequest req)
    //     {
    //         _subscriber.Subscribe<TEmittedEvent>(async @event => await HandleAsync(_handlers, @event));
    //         return await Act(ToCommand(req));
    //     }
    //
    //     protected abstract TCommand ToCommand(TRequest req);
    //
    //     protected abstract Task<TResponse> Act(TCommand cmd); 
    //
    //     private async Task HandleAsync(IEnumerable<IAggregateEventHandler<TAggregateId, TEmittedEvent>> handlers,
    //         TEmittedEvent @event)
    //     {
    //         foreach (var handler in handlers) await handler.HandleAsync(@event);
    //     }
    // }
}