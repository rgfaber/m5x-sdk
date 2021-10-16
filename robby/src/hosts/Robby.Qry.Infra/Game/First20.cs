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
    [Route(Config.Endpoints.First20)]
    public class First20Controller : ControllerBase
    {
        private readonly First20.IReader _reader;

        public First20Controller(First20.IReader reader)
        {
            _reader = reader;
        }


        [HttpPost]
        public async Task<ActionResult<Contract.Game.Queries.First20.Rsp>> Post([FromBody] Contract.Game.Queries.First20.Qry qry)
        {
            var rsp = Contract.Game.Queries.First20.Rsp.New(qry);
            try
            {
                rsp.Data = await _reader.FindAllAsync(qry);
                return Ok(rsp);
            }
            catch (Exception e)
            {
                rsp.ErrorState.Errors.Add(Constants.GameErrors.ApiError, e.AsApiError());
            }

            return BadRequest(rsp);
        }
    }


    public static class First20
    {
        public static IServiceCollection AddFirst20Qry(this IServiceCollection services)
        {
            return services
                .AddTransient<IReader, Reader>();
        }


        public interface IReader : IModelReader<Contract.Game.Queries.First20.Qry, Schema.Game>
        {
        }

        internal class Reader : CouchReader<Contract.Game.Queries.First20.Qry, Schema.Game>, IReader
        {
            public Reader(IGameDb db, ILogger logger) : base(db, logger)
            {
            }


            public override async Task<IEnumerable<Schema.Game>> FindAllAsync(Contract.Game.Queries.First20.Qry qry)
            {
                return await Db.RetrieveRecent(1, 20);
            }
        }
    }
}