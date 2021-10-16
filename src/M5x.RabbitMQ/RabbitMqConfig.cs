using System;
using RabbitMQ.Client;

namespace M5x.RabbitMQ
{
    public static class RabbitMqConfig
    {
        public static string Url = Environment.GetEnvironmentVariable(EnVars.RMQ_URL) ?? "amqp://rmq.local";

        public static string UserName =
            Environment.GetEnvironmentVariable(EnVars.RMQ_USER) ?? ConnectionFactory.DefaultUser;

        public static string Password =
            Environment.GetEnvironmentVariable(EnVars.RMQ_PWD) ?? ConnectionFactory.DefaultPass;
    }

    public static class EnVars
    {
        public const string RMQ_USER = "RMQ_USER";
        public const string RMQ_URL = "RMQ_URL";
        public const string RMQ_PWD = "RMQ_PWD";
    }
}