using System;
using System.IO;
using System.Reflection;

namespace M5x.DEC.Schema.Utils;

public static class AssemblyUtils
{
    public static Module[] GetLoadedModules()
    {
        var asm = Assembly.GetExecutingAssembly();
        return asm.GetLoadedModules();
    }

    public static string GetVersion(this Assembly assembly)
    {
        return assembly.GetName().Version.ToString();
    }

    public static string ShortName(this Assembly assembly)
    {
        return assembly.FullName.Split(',')[0];
    }

    public static Stream GetEmbeddedFile(this Assembly assembly, string fileName)
    {
        var assemblyName = assembly.ShortName();
        try
        {
            var str = assembly.GetManifestResourceStream($"{assemblyName}.{fileName}");
            if (str == null)
                throw new Exception("Could not locate embedded resource '" + fileName + "' in assembly '" +
                                    assemblyName + "'");
            return str;
        }
        catch (Exception e)
        {
            throw new Exception(assemblyName + ": " + e.Message);
        }
    }


    public static Stream GetEmbeddedFile(string assemblyName, string fileName)
    {
        try
        {
            var a = Assembly.Load(assemblyName);
            var str = a.GetManifestResourceStream(assemblyName + "." + fileName);

            if (str == null)
                throw new Exception("Could not locate embedded resource '" + fileName + "' in assembly '" +
                                    assemblyName + "'");
            return str;
        }
        catch (Exception e)
        {
            throw new Exception(assemblyName + ": " + e.Message);
        }
    }
}