using System.IO;
using System.Reflection;

namespace M5x.DEC.Schema.Utils
{
    public static class ResourceUtils
    {
        public static string GetEmbeddedResource(string resourceName, Assembly assembly)
        {
            resourceName = FormatResourceName(assembly, resourceName);
            using (var resourceStream = assembly.GetManifestResourceStream(resourceName))
            {
                if (resourceStream == null)
                    return null;
                using (var reader = new StreamReader(resourceStream))
                {
                    return reader.ReadToEnd();
                }
            }
        }


        public static Stream GetEmbeddedResourceStream(string resourceName, Assembly assembly)
        {
            resourceName = FormatResourceName(assembly, resourceName);
            return assembly.GetManifestResourceStream(resourceName);
        }


        private static string FormatResourceName(Assembly assembly, string resourceName)
        {
            return assembly.GetName().Name + "." + resourceName.Replace(" ", "_")
                .Replace("\\", ".")
                .Replace("/", ".");
        }
    }
}