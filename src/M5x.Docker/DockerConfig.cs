using System;

namespace M5x.Docker;

public static class DockerConfig
{
    public static int RestartCount =>
        Convert.ToInt32(Environment.GetEnvironmentVariable(EnVars.DOCKER_MAX_RESTARTS) ?? "10");

    public static bool ForceNewContainer =>
        Convert.ToBoolean(Environment.GetEnvironmentVariable(EnVars.DOCKER_FORCE_NEW_CONTAINER) ?? "false");

    public static bool ForceNewImage =>
        Convert.ToBoolean(Environment.GetEnvironmentVariable(EnVars.DOCKER_FORCE_NEW_IMAGE) ?? "false");

    public static void SetTestEnvironment()
    {
        Environment.SetEnvironmentVariable(EnVars.DOCKER_FORCE_NEW_CONTAINER, "false");
        Environment.SetEnvironmentVariable(EnVars.DOCKER_FORCE_NEW_IMAGE, "false");
        Environment.SetEnvironmentVariable(EnVars.DOCKER_MAX_RESTARTS, "5");
    }
}