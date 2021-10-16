using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using DotLiquid.Tags;
using M5x.DEC;
using M5x.DEC.Infra.CouchDb;
using M5x.DEC.Infra.EventStore;
using M5x.DEC.Infra.STAN;
using M5x.DEC.Persistence;
using M5x.DEC.PubSub;
using M5x.Schemas;
using NATS.Client;
using Robby.Domain;
using Robby.Schema;
using Serilog;
using Aggregate = Robby.Schema.Aggregate;

namespace Robby.Infra
{
    public static class InitializeSim
    {
        
        public interface IActor :
            IActor<Schema.Aggregate.ID, Contract.InitializeSim.Req, Contract.InitializeSim.Rsp, Domain.InitializeSim.Cmd>
        {
        }
        
        internal class Actor: Actor<Domain.Aggregate.Root, 
            Aggregate.ID, 
            Contract.InitializeSim.Req, 
            Contract.InitializeSim.Rsp,
            Domain.InitializeSim.Cmd, Contract.InitializeSim.Evt>, IActor
        {
            public Actor(IEventRepo eventRepo,
                IAggregateSubscriber subscriber,
                IEnumerable<IEmitter> handlers,
                ILogger logger) : base(eventRepo,
                subscriber,
                handlers,
                logger)
            {
            }

            protected override Domain.InitializeSim.Cmd CreateCommand(Contract.InitializeSim.Req req)
            {
                return new(Schema.Aggregate.ID.NewComb(req.AggregateId), req);
            }

            protected override async Task<Contract.InitializeSim.Rsp> Act(Domain.InitializeSim.Cmd cmd)
            {
                var rsp = new Contract.InitializeSim.Rsp(cmd.CorrelationId, cmd.Name);
                try
                {
                    var root = await EventRepo.GetByIdAsync(cmd.AggregateId);
                    if (root == null) 
                        throw new Domain.InitializeSim.Excp($"Aggregate [{cmd.AggregateId.Id}] does not exist");
                    if (root.Execute(cmd))
                    {
                        rsp.Id = cmd.AggregateId.Value;
                        await EventRepo.SaveAsync(root);
                    }
                }
                catch (Exception e)
                {
                    rsp.State.Errors.Add(Contract.Config.Errors.ServiceError, e.AsApiError());
                    if(Logger!=null) Logger.Fatal(JsonSerializer.Serialize(e.AsApiError()));
                }
                return rsp;
            }
        }

        
        
        
        
        public interface IEmitter: IEventEmitter<Aggregate.ID, Contract.InitializeSim.Evt> {}

        internal class Emitter: STANEmitter<Aggregate.ID, Contract.InitializeSim.Evt>, IEmitter
        {
            public Emitter(IEncodedConnection conn, ILogger logger) : base(conn, logger)
            {
            }
        }
        
        public class Listener : STANListener<Aggregate.ID,Contract.InitializeSim.Evt>
        {
            public Listener(IEncodedConnection conn,
                IEnumerable<IWriter> handlers,
                ILogger logger) : base(conn,
                handlers,
                logger)
            {
            }
        }
        
        public class Responder : STANResponder<Aggregate.ID, Domain.InitializeSim.Cmd, Contract.InitializeSim.Req,
            Contract.InitializeSim.Rsp>
        {
            public Responder(IEncodedConnection conn,
                IActor actor,
                ILogger logger) : base(conn, actor, logger)
            {
            }
        }
        
        public interface IWriter: IModelWriter<Aggregate.ID, Contract.InitializeSim.Evt, RoboSim>
        {
        }
        
        internal class Writer : CouchWriter<Aggregate.ID,Contract.InitializeSim.Evt,RoboSim>, IWriter
        {
            public Writer(IRoboSimStore store, ILogger logger) : base(store, logger)
            {
            }

            public override async Task<RoboSim> UpdateAsync(Contract.InitializeSim.Evt @event)
            {
                try
                {
                    var roboSim = await Store.GetByIdAsync(@event.AggregateId.Id);
                    if (roboSim == null)
                    {
                        roboSim = RoboSim.CreateNew(@event.AggregateId.Id, @event.Name);
                        return await Store.AddOrUpdateAsync(roboSim);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                return null;
            }
        }
        
        
        
    }

    

}