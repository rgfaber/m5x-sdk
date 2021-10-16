using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using M5x.DEC.Infra.CouchDb;
using M5x.DEC.Persistence;
using M5x.DEC.Schema.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Robby.Contract.Game;
using Robby.Schema;
using Serilog;

namespace Robby.Qry.Infra.Game
{
    [ApiController]
    [Route(Config.Endpoints.ById)]
    public class ByIdController : ControllerBase
    {
        private readonly ById.IReader _reader;

        public ByIdController(ById.IReader reader)
        {
            _reader = reader;
        }


        [HttpPost]
        public async Task<ActionResult<Contract.Game.Queries.ById.Rsp>> Post([FromBody] Contract.Game.Queries.ById.Qry qry)
        {
            var rsp = Contract.Game.Queries.ById.Rsp.New(qry);
            try
            {
                var game = await _reader.GetByIdAsync(qry.Id);
                rsp.Data = new[] {game};
                return Ok(rsp);
            }
            catch (Exception e)
            {
                Log.Error(e.InnerAndOuter());
                rsp.ErrorState.Errors.Add(Constants.GameErrors.ApiError, e.AsApiError());
            }
            
            return BadRequest(rsp);
        }
    }


    public static class ById
    {
        public static IServiceCollection AddById(this IServiceCollection services)
        {
            return services?
                .AddTransient<IGameDb, GameDb>()
                .AddTransient<IReader, Reader>();
        }

        public interface IReader : IModelReader<Contract.Game.Queries.ById.Qry, Schema.Game>
        {
        }


        internal class Reader : CouchReader<Contract.Game.Queries.ById.Qry, Schema.Game>, IReader
        {
            public Reader(IGameDb db, ILogger logger) : base(db, logger)
            {
            }

            public override async Task<IEnumerable<Schema.Game>> FindAllAsync(Contract.Game.Queries.ById.Qry qry)
            {
                var game = await GetByIdAsync(qry.Id);
                return new [] {game};
            }
        }
    }
}