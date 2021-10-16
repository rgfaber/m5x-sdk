using M5x.Swagger;

namespace Robby.Cmd
{
    public static class Config
    {
        public static IApiSettings CmdDef = new ApiSettings(
            "Robby.Cmd",
            "",
            "macula",
            "Robby Commands",
            new[] {"api"});
    }
}