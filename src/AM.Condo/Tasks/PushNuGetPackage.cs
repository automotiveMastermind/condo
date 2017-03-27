// <copyright file="PushNuGetPackage.cs" company="automotiveMastermind and contributors">
// © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>

namespace AM.Condo.Tasks
{
    using System;
    using System.Collections.Concurrent;
    using System.IO;
    using System.Threading.Tasks;

    using Microsoft.Build.Framework;

    using NuGet.Commands;
    using NuGet.Configuration;

    using MSBuildTask = Microsoft.Build.Utilities.Task;

    /// <summary>
    /// Represents a Microsoft Build task used to publish a package to a NuGet feed.
    /// </summary>
    public class PushNuGetPackage : MSBuildTask
    {
        #region Fields
        private volatile bool success;

        private ISettings settings;

        private IPackageSourceProvider provider;
        #endregion

        #region Constructors and Finalizers
        /// <summary>
        /// Initializes a new instance of the <see cref="PushNuGetPackage"/> class.
        /// </summary>
        public PushNuGetPackage()
            : this(settings: null, provider: null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PushNuGetPackage"/> class.
        /// </summary>
        /// <param name="settings">
        /// The settings used to push packages.
        /// </param>
        /// <param name="provider">
        /// The package source provider used to push packages.
        /// </param>
        public PushNuGetPackage(ISettings settings, IPackageSourceProvider provider)
        {
            // set success to false
            this.success = false;

            // set the settings and package source provider
            this.settings = settings;
            this.provider = provider;
        }
        #endregion

        #region Properties and Indexers
        /// <summary>
        /// Gets or sets the packages that should be published to the feed.
        /// </summary>
        [Required]
        public ITaskItem[] Packages { get; set; }

        /// <summary>
        /// Gets or sets the path to the nuget configuration file.
        /// </summary>
        [Required]
        public string NuGetConfigPath { get; set; }

        /// <summary>
        /// Gets or sets the URI of the feed.
        /// </summary>
        [Required]
        public string Uri { get; set; }

        /// <summary>
        /// Gets or sets the API key for the feed.
        /// </summary>
        [Required]
        public string ApiKey { get; set; }

        /// <summary>
        /// Gets or sets the URI of the symbol feed.
        /// </summary>
        public string SymbolUri { get; set; }

        /// <summary>
        /// Gets or sets the API key for the symbol feed.
        /// </summary>
        public string SymbolApiKey { get; set; }

        /// <summary>
        /// Gets or sets the timeout (in milliseconds) before attempting to republish a package to a feed.
        /// </summary>
        public int Timeout { get; set; } = 10000;

        /// <summary>
        /// Gets or sets the available number of tasks (processes) used to publish packages to the feed.
        /// </summary>
        public int Parallelism { get; set; } = Environment.ProcessorCount * 2;

        /// <summary>
        /// Gets or sets the number of retries allowed before failing to publish a package.
        /// </summary>
        public int Retries { get; set; } = 2;

        /// <summary>
        /// Gets or sets a value indicating whether or not to allow symbol packages to be pushed.
        /// </summary>
        public bool NoSymbols { get; set; }
        #endregion

        /// <summary>
        /// Executes the <see cref="PushNuGetPackage"/> task.
        /// </summary>
        /// <returns>
        /// A value indicating whether or not the task was successfully executed.
        /// </returns>
        public override bool Execute()
        {
            // determine if the settings are null
            if (this.settings == null)
            {
                // get the directory and filename
                var root = Path.GetDirectoryName(this.NuGetConfigPath);
                var filename = Path.GetFileName(this.NuGetConfigPath);

                // initialize the settings
                this.settings = new Settings(root, filename, isMachineWideSettings: false);
            }

            // determine if the provider is null
            if (this.provider == null)
            {
                // initialize the package source provider
                this.provider = new PackageSourceProvider(this.settings);
            }

            // ensure that packages are specified
            if (this.Packages == null)
            {
                // log an error
                this.Log.LogError($"{nameof(this.Packages)} must be specified.");

                // move on immediately
                return false;
            }

            // ensure that at least one package exists
            if (this.Packages.Length == 0)
            {
                // log a warning
                this.Log.LogWarning($"{nameof(this.Packages)} does not contain any packages. Skipping publish.");

                // move on immediately
                return true;
            }

            // set success to true
            this.success = true;

            // create parallel options for executing multiple tasks
            var options = new ParallelOptions
            {
                MaxDegreeOfParallelism = this.Parallelism
            };

            // create a new logger
            var logger = new NuGetMSBuildLogger(this.Log);

            // capture tasks
            var tasks = new ConcurrentBag<Task>();

            // create a log for logging
            var @lock = new object();

            // execute the push using the appropriate parallelism
            Parallel.ForEach(this.Packages, options, package =>
            {
                // create an action logger
                var actions = new NuGetActionLogger();

                // push the package
                var result = this.Push(package, actions).Result;

                // determine if the push was unsuccessful
                if (!result)
                {
                    // set the success flag
                    this.success = false;
                }

                // log the results
                tasks.Add(Task.Run(() =>
                {
                    // lock so log messages are congruent
                    lock (@lock)
                    {
                        // replay the log actions against the msbuild log
                        actions.Replay(logger);
                    }
                }));
            });

            // wait for tasks to complete
            Task.WhenAll(tasks).Wait();

            // return success
            return this.success;
        }

        private async Task<bool> Push(ITaskItem package, NuGet.Common.ILogger logger)
        {
            // get the full path to the package
            var path = package.GetMetadata("FullPath");
            var name = package.GetMetadata("Identity");

            // count the number of attempts
            var attempts = 0;

            // continue to attempt to push forever (short circuit is in catch)
            while (true)
            {
                try
                {
                    // increment the attempt counter
                    attempts++;

                    // attempt to push the packages
                    await PushRunner.Run
                        (
                            this.settings,
                            this.provider,
                            path,
                            this.Uri,
                            this.ApiKey,
                            this.SymbolUri,
                            this.SymbolApiKey,
                            this.Timeout / 1000,
                            disableBuffering: false,
                            noSymbols: this.NoSymbols,
                            logger: logger
                        );

                    // log a success message
                    this.Log.LogMessage(MessageImportance.High, $"Successfully pushed package {name} after {attempts} attempts.");

                    // move on immediately
                    return true;
                }
                catch when (attempts <= this.Retries)
                {
                    // log a warning
                    this.Log.LogWarning
                        ($"NuGet push failed for package: {name} after {attempts} attempts with {this.Retries - attempts} retries remaining.");
                }
                catch (Exception netEx)
                {
                    // log an error
                    this.Log.LogError($"Failed to push package: {name} after {this.Retries} attempts.");

                    // capture the exception
                    var exception = netEx;

                    // continue logging until exception is null
                    while (exception != null)
                    {
                        // log the exception
                        this.Log.LogErrorFromException(exception);

                        // get the inner exception
                        exception = exception.InnerException;
                    }

                    // move on immediately
                    return false;
                }
            }
        }
    }
}
