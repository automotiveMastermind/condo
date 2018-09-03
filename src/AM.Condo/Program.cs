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
            var verbosity = "normal";
            var color = string.Empty;
            string[] options = { };

            // Build paths
            var path = Directory.GetCurrentDirectory();
            var response = Path.Combine(string.Empty, "/condo/condo.msbuild.rsp");
            var log = Path.Combine(string.Empty, "/target/artifacts/log/condo.msbuild.log");

            // Test for arguments
            for (var i = 0; i < args.Length; i++)
            {
                var arg = args[i];

                switch (arg)
                {
                    case "-v":
                    case "--verbosity":
                        i++;
                        verbosity = args[i];
                        break;
                    case "-nc":
                    case "--no-color":
                        color = "DisableConsoleColor";
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
            using (var build = new StreamWriter(response, append: true))
            {
                // Append args and options to build arguments
                build.WriteLine($"-flp:LogFile=\"{log}\";Encoding=UTF-8;Verbosity={verbosity} \n");
                build.WriteLine($"-clp:{color};Verbosity={verbosity} \n");

                foreach (var option in options)
                {
                    build.WriteLine(option);
                }
            }

            // Execute dotnet msbuild
            var invoker = new ProcessInvoker(path, "dotnet", "msbuild", new ConsoleLogger(), isRealtime: true);
            var output = invoker.Execute($"@{response}", throwOnError: true);

            return output.ExitCode;
        }
    }
}
