using System.Collections.Generic;
using System.Threading.Tasks;
using M5x.DEC.Schema;

namespace M5x.DEC.Persistence;

public interface IModelReader
{
}

public interface ISingleModelReader<in TQuery, TPayload> : IModelReader
    where TQuery : IQuery
    where TPayload : IPayload
{
    Task<TPayload> GetByIdAsync(string id);
}

public interface IModelReader<in TQuery, TPayload> : ISingleModelReader<TQuery, TPayload>
    where TQuery : IQuery
    where TPayload : IPayload
{
    Task<IEnumerable<TPayload>> FindAllAsync(TQuery qry);
}