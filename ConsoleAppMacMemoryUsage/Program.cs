using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConsoleAppMacMemoryUsage
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            for (int i = 0; i < 50; i++)
            {
                var i1 = i;
                Task.Factory.StartNew(() => {Console.WriteLine($"Welcome from - {i1}"); });
            }

            var osXOUTPUT = GetOsXTopOutput();
            var cpuPercentage = GetOverallCpuUsagePercentage();
            
        }
        
        private static string[] GetOsXTopOutput()
        {
            var info = new ProcessStartInfo("top");
            info.Arguments = "-l 1 -n 0";
            info.RedirectStandardOutput = true;

            string output;
            using (var process = Process.Start(info))
            {
                output = process.StandardOutput.ReadToEnd();
            }

            return output.Split('\n');
        }

        public static double GetOverallCpuUsagePercentage()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                var lines = GetOsXTopOutput();

                // Example: "CPU usage: 8.69% user, 21.73% sys, 69.56% idle"
                var pattern = @"CPU usage: \d+\.\d+% user, \d+\.\d+% sys, (\d+\.\d+)% idle";
                Regex r = new Regex(pattern, RegexOptions.IgnoreCase);
                foreach (var line in lines)
                {
                    Match m = r.Match(line);
                    if (m.Success)
                    {
                        var idle = double.Parse(m.Groups[1].Value);
                        var used = 100 - idle;
                        return used;
                    }
                }

                // Or throw an exception
                return -1.0;
            }
            else
            {
                // E.g., Melih Altıntaş's solution: https://stackoverflow.com/a/59465268/132042
            }

            return -1.0;
        }
    }
}