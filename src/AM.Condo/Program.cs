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
    using System.Reflection;
    using System.Text;

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
            // defaults
            var verbosity = "normal";
            var color = string.Empty;
            string[] options = { };

            // parse options
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

            // get the build paths
            var path = Directory.GetCurrentDirectory();

            // get the location of the condo executable
            var condo = Path.GetDirectoryName(Assembly.GetAssembly(typeof(Program)).Location);

            // get the response and log paths
            var artifacts = Path.Combine(path, "artifacts");
            var response = Path.Combine(artifacts, "condo.msbuild.rsp");
            var log = Path.Combine(artifacts, "condo.msbuild.log");

            // determine if the artifacts path does not exist
            if (!Directory.Exists(artifacts))
            {
                // create the artifacts path
                Directory.CreateDirectory(artifacts);
            }

            // determine if the response file already exits
            if (File.Exists(response))
            {
                // delete the response file
                File.Delete(response);
            }

            // determine if the log file already exists
            if (File.Exists(log))
            {
                // delete the log file
                File.Delete(log);
            }

            // get the template
            var template = File.ReadAllText(Path.Combine(condo, "condo.msbuild.rsp"));

            var builder = new StringBuilder(template)
                .Replace("$TARGET_PATH", path)
                .Replace("$CONDO_PATH", condo)
                .Replace("$LOG_PATH", log)
                .Replace("$COLOR", color)
                .Replace("$VERBOSITY", verbosity);

            // append the options
            Array.ForEach(options, option => builder.AppendLine(option));

            // save the response file
            File.WriteAllText(response, builder.ToString());

            // Execute dotnet msbuild against the response file
            var invoker = new ProcessInvoker(path, "dotnet", "msbuild", new ConsoleLogger(), isRealtime: true);
            var output = invoker.Execute($"@{response}", throwOnError: true);

            // return the exit code
            return output.ExitCode;
        }
    }
}
