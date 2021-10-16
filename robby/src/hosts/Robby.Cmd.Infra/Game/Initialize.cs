using System;
using System.Threading.Tasks;
using M5x.DEC.Infra.STAN;
using M5x.DEC.Schema;
using M5x.DEC.Schema.Extensions;
using M5x.DEC.Schema.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NATS.Client;
using Robby.Contract.Game;
using Robby.Schema;
using Serilog;
using static Robby.Domain.Game.Initialize;

namespace Robby.Cmd.Infra.Game
{
    [ApiController]
    [Route(Config.Endpoints.Initialize)]
    public class InitializeContextController : ControllerBase
    {
        private readonly Client.Infra.Features.Initialize.IRequester _requester;
        private readonly ILogger _logger;

        public InitializeContextController(Client.Infra.Features.Initialize.IRequester requester, ILogger logger)
        {
            _requester = requester;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<Contract.Game.Features.Initialize.Feedback>> Post(
            [FromBody] Contract.Game.Features.Initialize.Hope hope)
        {
            if (hope == null) return BadRequest("There must be hope");
            var rsp = Contract.Game.Features.Initialize.Feedback.Empty(hope.CorrelationId);
            try
            {
                rsp = await _requester.RequestAsync(hope);
                if (rsp.IsSuccess)
                    return Ok(rsp);
            }
            catch (Exception e)
            {
                rsp.ErrorState.Errors.Add(Constants.GameErrors.ApiError, e.AsApiError());
                _logger.Error(e.InnerAndOuter());
            }

            return BadRequest(rsp);
        }
    }


    public static class InitializeGame
    {
        public static IServiceCollection AddInitializeContextCmd(this IServiceCollection services)
        {
            return services?
                .AddInitializeActor()
                .AddTransient<IEmitter, Emitter>()
                .AddHostedService<Responder>();
        }

        internal class Emitter : STANEmitter<Schema.Game.ID, Contract.Game.Features.Initialize.Fact>, IEmitter
        {
            public Emitter(IEncodedConnection conn, ILogger logger) : base(conn, logger)
            {
            }
        }

        public class Responder : STANAsyncResponder<Domain.Game.Aggregate.Root,
            Schema.Game.ID,
            Contract.Game.Features.Initialize.Hope,
            Domain.Game.Initialize.Cmd,
            Contract.Game.Features.Initialize.Feedback>
        {
            public Responder(IEncodedConnection conn,
                IActor actor,
                ILogger logger) : base(conn, actor, logger)
            {
            }

            protected override Domain.Game.Initialize.Cmd ToCommand(Contract.Game.Features.Initialize.Hope hope)
            {
                return Domain.Game.Initialize.Cmd.New(
                    Schema.Game.ID.With(hope.AggregateId), 
                    hope.CorrelationId, 
                    hope.Payload);
            }
        }
    }
}