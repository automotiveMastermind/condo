// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="automotiveMastermind and contributors">
// © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AM.Condo
{
    using System;
    using System.IO;
    using System.Linq;

    using AM.Condo.Diagnostics;

    /// <summary>
    /// Represents the default entry point of the module.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Represents the default entry point of the module.
        /// </summary>
        /// <param name="args">
        /// Arguments passed to the module; usually at the command line.
        /// </param>
        /// <returns>
        /// The exit code of the module.
        /// </returns>
        public static int Main(string[] args)
        {
            // Defaults
            var condoVerbosity = "normal";
            var condoColor = string.Empty;
            var condoSkipFrontend = string.Empty;
            string[] options = { };

            // Build paths
            var path = Directory.GetCurrentDirectory();
            var buildSettingsPath = Path.Combine(path, "condo.msbuild.rsp");
            var msbuildLog = Path.Combine(string.Empty, "/target/artifacts/log/condo.msbuild.log");

            // Test for arguments
            for (int i = 0; i < args.Length; i++)
            {
                var arg = args[i];
                switch (arg)
                {
                    case "-v":
                    case "--verbosity":
                        i++;
                        condoVerbosity = args[i];
                        break;
                    case "-nc":
                    case "--no-color":
                        condoColor = "DisableConsoleColor";
                        break;
                    case "--":
                        i++;
                        options = args.Skip(i).ToArray();
                        break;
                    default:
                        throw new ArgumentException($"Unknown argument: {arg}");
                }
            }

            // Get default build arguments from file
            using (var build = new StreamWriter(buildSettingsPath, append: true))
            {
                // Append args and options to build arguments
                build.WriteLine($"-flp:LogFile=\"{msbuildLog}\";Encoding=UTF-8;Verbosity={condoVerbosity} \n");
                build.WriteLine($"-clp:{condoColor};Verbosity={condoVerbosity} \n");
                build.WriteLine(condoSkipFrontend);

                foreach (var option in options)
                {
                    build.WriteLine(option);
                }
            }

            // Execute dotnet msbuild
            IProcessInvoker invoker = new ProcessInvoker(path, "dotnet", subCommand: "msbuild", logger: new ConsoleLogger(), isRealtime: true);
            var output = invoker.Execute($"@{buildSettingsPath}", throwOnError: true);

            return output.ExitCode;
        }
    }
}
