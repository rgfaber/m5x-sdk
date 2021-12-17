using System;

namespace M5x.Swagger;

public static class OpenApiConfig
{
    public static bool DoNotFilterNamespaceSchema =
        Convert.ToBoolean(Environment.GetEnvironmentVariable(EnVars.OPENAPI_DONOT_FILTER_NAMESPACE_SCHEMA));

    public static bool DoNotUseFullTypeNames =
        Convert.ToBoolean(Environment.GetEnvironmentVariable(EnVars.OPENAPI_DONOT_USE_FULL_TYPE_NAMES));
}