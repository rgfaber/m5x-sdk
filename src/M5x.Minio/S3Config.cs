using System;

namespace M5x.Minio
{
    public static class S3Config
    {
        public static string Endpoint =>
            Environment.GetEnvironmentVariable(EnVars.S3_ENDPOINT) ?? "http://s3.local";

        public static string PublicKey =>
            Environment.GetEnvironmentVariable(EnVars.S3_PUBLIC_KEY) ?? "minio-mk-public-key";

        public static string PrivateKey =>
            Environment.GetEnvironmentVariable(EnVars.S3_PRIVATE_KEY) ?? "minio-mk-private-key";

        public static string Region =>
            Environment.GetEnvironmentVariable(EnVars.S3_REGION) ?? "eu-west-1";

        public static string SessionToken =>
            Environment.GetEnvironmentVariable(EnVars.S3_SESSION_TOKEN) ?? "";
    }
}