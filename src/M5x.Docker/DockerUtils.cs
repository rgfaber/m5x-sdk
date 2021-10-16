using System;

namespace M5x.Docker
{
    public static class DockerUtils
    {
        public static bool IsLinux
        {
            get
            {
                var p = (int)Environment.OSVersion.Platform;
                return p == 4 || p == 6 || p == 128;
            }
        }

        public static Uri DaemonUri => IsLinux
            ? new Uri("unix:///var/run/docker.sock")
            : new Uri("npipe://./pipe/docker_engine");
    }
}