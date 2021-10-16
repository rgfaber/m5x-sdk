using System.Diagnostics;

namespace M5x.DEC.Schema.Utils
{
    public static class Bash
    {
        public static string Exec(this string cmd)
        {
            var escapedArgs = cmd.Replace("\"", "\\\"");

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"{escapedArgs}\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            var result = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            return result;
        }
    }
}