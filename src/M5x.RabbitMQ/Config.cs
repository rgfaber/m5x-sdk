using System;
using M5x.Config;

namespace M5x.RabbitMQ
{
    public static class Config
    {
        public static string Host = DotEnv.Get(EnVars.RABBITMQ_HOST) ?? "rabbitmq";
        public static string User = DotEnv.Get(EnVars.RABBITMQ_USER) ?? "guest";
        public static string Password = DotEnv.Get(EnVars.RABBITMQ_PWD) ?? "guest";
        public static int Port = Convert.ToInt32(DotEnv.Get(EnVars.RABBITMQ_PORT) ?? "5672");
        public static string VHost = DotEnv.Get(EnVars.RABBITMQ_VHOST) ?? "/";
        public static bool DispatchConsumerAsync =
            Convert.ToBoolean(DotEnv.Get(EnVars.RABBITMQ_DISPATCH_CONSUMER_ASYNC));
    }
}