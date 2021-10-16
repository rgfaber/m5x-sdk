using M5x.Swagger;

namespace Robby.Qry
{
    public static class Config
    {
        public static readonly IApiSettings QryDef = new ApiSettings("Robby.Qry",
            "",
            "macula",
            "Robby Queries",
            new[]
            {
                "api"
            });
    }
}