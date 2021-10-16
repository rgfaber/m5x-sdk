using System;

namespace M5x.Store
{
    public class StoreConfig
    {
        public static string ClusterHost =>
            Environment.GetEnvironmentVariable(EnVars.COUCH_CLUSTER_HOST) ?? "sofa.macula.io";

        public static int ClusterPort =>
            Convert.ToInt32(Environment.GetEnvironmentVariable(EnVars.COUCH_CLUSTER_PORT) ?? "443");

        public static string ClusterUser => Environment.GetEnvironmentVariable(EnVars.COUCH_CLUSTER_USER) ?? "root";
        public static string ClusterPwd => Environment.GetEnvironmentVariable(EnVars.COUCH_CLUSTER_PWD) ?? "dev";

        public static string ClusterProtocol =>
            Environment.GetEnvironmentVariable(EnVars.COUCH_CLUSTER_PROTOCOL) ?? "https";


        public static string LocalHost => Environment.GetEnvironmentVariable(EnVars.COUCH_LOCAL_HOST) ?? "localhost";
        public static string LocalProtocol => Environment.GetEnvironmentVariable(EnVars.COUCH_LOCAL_PROTOCOL) ?? "http";


        public static int LocalPort =>
            Convert.ToInt32(Environment.GetEnvironmentVariable(EnVars.COUCH_LOCAL_PORT) ?? "5984");

        public static string LocalUser => Environment.GetEnvironmentVariable(EnVars.COUCH_LOCAL_USER) ?? "root";
        public static string LocalPwd => Environment.GetEnvironmentVariable(EnVars.COUCH_LOCAL_PWD) ?? "dev";


        public static string ClusterServer =>
            $"{ClusterProtocol}://{ClusterUser}:{ClusterPwd}@{ClusterHost}:{ClusterPort}";

        public static string LocalServer =>
            $"{LocalProtocol}://{LocalUser}:{LocalPwd}@{LocalHost}:{LocalPort}";

        //public static string LocalServer =>
        //    $"{LocalProtocol}://{LocalUser}:{LocalPwd}@localhost:{LocalPort}";


        public static string ClusterSource =>
            $"{ClusterProtocol}://{ClusterHost}:{ClusterPort}";

        public static string LocalSource =>
            $"{LocalProtocol}://{LocalHost}:{LocalPort}";

        public static bool CanReplicate =>
            Convert.ToBoolean(Environment.GetEnvironmentVariable(EnVars.COUCH_CAN_REPLICATE) ?? "true");
        public static string StrictLocalhostServer =>
            $"{LocalProtocol}://{LocalUser}:{LocalPwd}@localhost:{LocalPort}";

        public static bool IsTest => Convert.ToBoolean(Environment.GetEnvironmentVariable(EnVars.IS_TEST));

        public static void BuildEnvironment()
        {
            Environment.SetEnvironmentVariable(EnVars.COUCH_LOCAL_HOST, LocalHost);
            Environment.SetEnvironmentVariable(EnVars.COUCH_LOCAL_PROTOCOL, LocalProtocol);
            Environment.SetEnvironmentVariable(EnVars.COUCH_LOCAL_PORT, Convert.ToString(LocalPort));
            Environment.SetEnvironmentVariable(EnVars.COUCH_LOCAL_USER, LocalUser);
            Environment.SetEnvironmentVariable(EnVars.COUCH_LOCAL_PWD, LocalPwd);
            Environment.SetEnvironmentVariable(EnVars.COUCH_CAN_REPLICATE, Convert.ToString(CanReplicate));
            Environment.SetEnvironmentVariable(EnVars.COUCH_CLUSTER_HOST, ClusterHost);
            Environment.SetEnvironmentVariable(EnVars.COUCH_CLUSTER_PROTOCOL, ClusterProtocol);
            Environment.SetEnvironmentVariable(EnVars.COUCH_CLUSTER_PORT, Convert.ToString(ClusterPort));
            Environment.SetEnvironmentVariable(EnVars.COUCH_CLUSTER_USER, ClusterUser);
            Environment.SetEnvironmentVariable(EnVars.COUCH_CLUSTER_PWD, ClusterPwd);
        }
    }
}