using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace M5x.Schemas
{
    public static class DotEnv
    {
        public static void FromEmbedded(Assembly assembly, string name = ".env")
        {
            var sIn = assembly.GetEmbeddedFile(name);
            var lines = sIn.AsString().Split('\n');
            LoadEnv(lines);
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
        
        
        public static string GetVersion(this Assembly assembly)
        {
            return assembly.GetName().Version.ToString();
        }

        public static string ShortName(this Assembly assembly)
        {
            return assembly.FullName.Split(',')[0];
        }


        
        
        public static string AsString(this Stream sIn)
        {
            if (sIn.CanSeek)
                sIn.Position = 0;
            var sr = new StreamReader(sIn);
            var s = sr.ReadToEnd();
            return s;
        }



        public static void FromFile(string filePath)
        {
            if (!File.Exists(filePath))
                return;
            LoadEnv(File.ReadAllLines(filePath));
        }

        private static void LoadEnv(IEnumerable<string> lines)
        {
            foreach (var line in lines)
            {
                var parts = line.Split(
                    '=',
                    StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != 2)
                    continue;
                Environment.SetEnvironmentVariable(parts[0], parts[1]);
            }
        }

        public static void FromEmbedded(string fileName = ".env")
        {
            var assy = Assembly.GetCallingAssembly();
            FromEmbedded(assy, fileName);
        }

        public static string Get(string name)
        {
            return Environment.GetEnvironmentVariable(name);
        }

        public static string Set(string name, object value)
        {
            Environment.SetEnvironmentVariable(name, Convert.ToString(value));
            return Get(name);
        }
    }
}