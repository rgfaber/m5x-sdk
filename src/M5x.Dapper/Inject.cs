using System;
using System.Data;
using Microsoft.Extensions.DependencyInjection;

namespace M5x.Dapper;

public static class Inject
{
    public static IServiceCollection AddDapper(this IServiceCollection services)
    {
        return services?
            .AddTransient<IDbConnection>(sp => new SqlConnection(DapperConfig.DbConnection))
            .AddTransient<IDapperConnection>();
    }

    public class DapperConnection : IDapperConnection
    {
    }
}

public static class DapperConfig
{
    public static string DbConnection = Environment.GetEnvironmentVariable(EnVars.DB_CONNECTION);
}

public static class EnVars
{
    public const string DB_CONNECTION = "DB_CONNECTION";
}

public interface IDapperConnection
{
}