using System;
using System.Threading.Tasks;
using Api;
using Dgraph;
using Dgraph.Transactions;
using FluentResults;
using Grpc.Core;
using ILogger = Serilog.ILogger;

namespace M5x.DGraph;

internal class GraphClient : IGraphClient
{
    private readonly IDgraphClient _client;
    private readonly ILogger _logger;


    public GraphClient(IDgraphClient client, ILogger logger)
    {
        _client = client;
        _logger = logger;
    }

    public GraphClient(IDgraphClient client)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        _client?
            .Dispose();
    }

    public Task<Result> Alter(Operation op, CallOptions? options = null)
    {
        try
        {
            return _client.Alter(op, options);
        }
        catch (Exception e)
        {
            _logger?.Fatal(e.Message, e);
            Console.WriteLine(e);
            throw;
        }
    }

    public Task<Result<string>> CheckVersion(CallOptions? options = null)
    {
        try
        {
            return _client.CheckVersion(options);
        }
        catch (Exception e)
        {
            _logger?.Fatal(e.Message, e);
            Console.WriteLine(e);
            throw;
        }
    }

    public IQuery NewReadOnlyTransaction(bool bestEffort = false)
    {
        try
        {
            return _client.NewReadOnlyTransaction(bestEffort);
        }
        catch (Exception e)
        {
            _logger?.Fatal(e.Message, e);
            Console.WriteLine(e);
            throw;
        }
    }

    public ITransaction NewTransaction()
    {
        try
        {
            return _client.NewTransaction();
        }
        catch (Exception e)
        {
            _logger?.Fatal(e.Message, e);
            Console.WriteLine(e);
            throw;
        }
    }
}