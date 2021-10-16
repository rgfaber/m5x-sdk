using System;
using M5x.Config;
using Serilog.Core;
using Serilog.Events;

namespace M5x.Serilog
{
    public class EnvLogLevelSwitch : LoggingLevelSwitch
    {
        public EnvLogLevelSwitch(string environmentVariable)
        {
            var level = LogEventLevel.Information;
            MinimumLevel = level;
            if (Enum.TryParse(DotEnv.Expand(EnVars.LOG_LEVEL_MIN), true, out level)) MinimumLevel = level;
        }
    }
}