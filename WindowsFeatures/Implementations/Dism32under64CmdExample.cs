using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace WindowsFeatures.Implementations
{
    /// <summary>
    /// Windows features list over dism.exe (Windows x64 && under 32 bit process).
    /// </summary>
    internal class Dism32Under64CmdExample
    {
        private const string Enabled = "Enabled";

        private const string CmdName = "cmd.exe";

        private const string FeatureState = "State : ";

        private const string FeatureName = "Feature Name : ";

        /// <summary>
        /// Execute example.
        /// </summary>
        public void Run()
        {
            if (!CheckValidEnvironment())
            {
                return;   
            }

            foreach (var featurePair in GetWindowsFeatures())
            {
                var featureName = featurePair.Key;
                var featureRunning = featurePair.Value ? "Enabled" : "Disabled";

                Console.WriteLine($"[Feature Name] {featureName}");
                Console.WriteLine($"[State] {featureRunning}");
                Console.WriteLine();
            }
        }

        private static Dictionary<string, bool> GetWindowsFeatures()
        {
            var tmpFilePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            
            var dismParameters = new[]
            {
                "dism.exe",

                // Targets the running operating system.
                "/Online",

                // Displays command line output in English.
                "/English",

                // - Displays information about all features in a package.
                "/Get-Features",

                // Write result to file. "BatCommand > C:\1.txt" prints BatCommand result to file stream.
                $@"> ""{tmpFilePath}"""};

            var processStartInfo = new ProcessStartInfo(CmdName)
            {
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                UseShellExecute = false
            };
            
            var process = new Process() { StartInfo = processStartInfo };

            // LOCATION: "C:\windows\SysWow64\
            process.Start();

            // LOCATION: "C:\Windows\" otherwise we will have in use x32.
            process.StandardInput.WriteLine("cd ..");

            // cmd64.exe символьная ссылка на конкретный файл (что бы винда не подсовывала реализацию х32 битного)
            var cmd64FullPath = Path.Combine(Environment.SystemDirectory, CmdName);
            process.StandardInput.WriteLine($@"mklink cmd64.exe ""{cmd64FullPath}""");

            process.StandardInput.WriteLine($"cmd64.exe /c {string.Join(" ", dismParameters)}");
            process.StandardInput.WriteLine("exit");
            process.WaitForExit();

            var result = ParseOutput(File.ReadAllText(tmpFilePath));
            File.Delete(tmpFilePath);

            return result;
        }

        private static Dictionary<string, bool> ParseOutput(string output)
        {
            var result = new Dictionary<string, bool>();
            var featureGroupLines = output.Split(
                new[] { Environment.NewLine + Environment.NewLine },
                StringSplitOptions.RemoveEmptyEntries);

            foreach (var featureGroupLine in featureGroupLines)
            {
                if (!featureGroupLine.Contains(Environment.NewLine))
                {
                    continue;
                }

                var featureLines = featureGroupLine.Split(
                    new[]
                    {
                        Environment.NewLine
                    },
                    StringSplitOptions.RemoveEmptyEntries);

                string featureState = null;
                string featureName = null; 
                
                foreach (var featureLine in featureLines)
                {
                    if (featureLine.StartsWith(FeatureName))
                    {
                        featureName = featureLine.Substring(FeatureName.Length);
                    }

                    if (featureLine.StartsWith(FeatureState))
                    {
                        featureState = featureLine.Substring(FeatureState.Length);
                    }
                }

                if (featureName != null && featureState != null)
                {
                    bool state = string.Equals(featureState, Enabled, StringComparison.OrdinalIgnoreCase);
                    result.Add(featureName, state);
                }
            }

            return result;
        }

        private bool CheckValidEnvironment()
        {
            var isx32Process = !Environment.Is64BitProcess;
            var is64Os = Environment.Is64BitOperatingSystem;

            if (isx32Process && is64Os)
            {
                return true;
            }

            Console.BackgroundColor = ConsoleColor.Green;
            var oldColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Black;

            Console.WriteLine("Your process isn't x32 or\\and your operation system isn't x64. If you gonna use a DISM in x64 process you won't have any problems and feel free to use example 1.");
            Console.WriteLine("For check this out switch application configuration from AnyCPU\\x64 to x32");


            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = oldColor;

            return false;
        }
    }
}