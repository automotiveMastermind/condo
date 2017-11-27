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
    using AM.Condo.IO;

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
            var condoVerbosity = "normal";
            string[] options = { };

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
                    case "--":
                        options = args.Skip(i).ToArray();
                        break;
                }
            }

            // Build paths
            var path = Directory.GetCurrentDirectory();
            var buildSettings = Path.Combine(path, "condo.msbuild.rsp");
            var msbuildLog = Path.Combine(string.Empty, "/src/artifacts/log/condo.msbuild.log");

            Console.WriteLine(path);
            Console.WriteLine(buildSettings);
            Console.WriteLine(msbuildLog);

            // Append verbosity and additional options to msbuild script
            using (var build = new StreamWriter(buildSettings, append: true))
            {
                build.WriteLine($"-flp:LogFile=\"{msbuildLog}\";Encoding=UTF-8;Verbosity={condoVerbosity}");
                build.WriteLine($"-clp:DisableConsoleColor;Verbosity={condoVerbosity}");
                build.Write(options);
            }

            IProcessInvoker invoker = new ProcessInvoker(path, "dotnet", subCommand: null, logger: new NoOpLogger());
            var output = invoker.Execute($"msbuild {buildSettings}", throwOnError: true);
            return output.ExitCode;
        }
    }
}
