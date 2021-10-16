using System;

namespace M5x.Twilio
{
    public static class Config
    {
        public static string UserName =
            Environment.GetEnvironmentVariable(EnVars.TWILIO_USERNAME);

        public static string Password = Environment.GetEnvironmentVariable(EnVars.TWILIO_PASSWORD);
        public static string AccountSid = Environment.GetEnvironmentVariable(EnVars.TWILIO_ACCOUNT_SID);
        public static string AuthToken = Environment.GetEnvironmentVariable(EnVars.TWILIO_AUTH_TOKEN);
        public static string Region = Environment.GetEnvironmentVariable(EnVars.TWILIO_REGION);
        public static string Edge = Environment.GetEnvironmentVariable(EnVars.TWILIO_EDGE);
    }
}