// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SetNuGetCredentials.cs" company="automotiveMastermind and contributors">
//   © automotiveMastermind and contributors. Licensed under MIT. See LICENSE for details.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AM.Condo.Tasks
{
    using System.Linq;
    using System.IO;

    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;
    using NuGet.Configuration;
    using System;

    /// <summary>
    /// Represents a NuGet task that sets the credentials for a NuGet Package source.
    /// </summary>
    public class SetNuGetCredentials : Task
    {
        #region Fields
        private ISettings settings;

        private IPackageSourceProvider provider;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the root of the repository.
        /// </summary>
        [Required]
        public string RepositoryRoot { get; set; }

        /// <summary>
        /// Gets or sets the name of the source that should be updated to use the credential manager.
        /// </summary>
        [Required]
        public ITaskItem[] Sources { get; set; }

        /// <summary>
        /// Gets the path to the NuGet configuration file that should be used for nuget tasks.
        /// </summary>
        [Output]
        public string NuGetConfigPath { get; private set; }

        /// <summary>
        /// Gets or sets the username that should be assigned to the source.
        /// </summary>
        [Required]
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the access token used to access packages protected by the credential manager.
        /// </summary>
        [Required]
        public string Password { get; set; }
        #endregion

        #region Constructors and Finalizers
        /// <summary>
        /// Initializes a new instance of the <see cref="SetNuGetCredentials"/> class.
        /// </summary>
        public SetNuGetCredentials()
            : this(settings: null, provider: null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SetNuGetCredentials"/> class.
        /// </summary>
        /// <param name="settings">
        /// The settings used to push or restore packages.
        /// </param>
        /// <param name="provider">
        /// The package source provider used to push or restore packages.
        /// </param>
        public SetNuGetCredentials(ISettings settings, IPackageSourceProvider provider)
        {
            // set the settings and package source provider
            this.settings = settings;
            this.provider = provider;
        }

        /// <summary>
        /// Execute the set credentials task.
        /// </summary>
        /// <returns>
        /// A value indicating whether or not the task executed successfully.
        /// </returns>
        public override bool Execute()
        {
            // determine if the settings are not specified
            if (this.settings == null)
            {
                // load the settings
                this.settings = Settings.LoadDefaultSettings(this.RepositoryRoot);
            }

            // determine if the provider is not specified
            if (this.provider == null)
            {
                // create a new package source provider
                this.provider = new PackageSourceProvider(this.settings);
            }

            // capture the priority config file
            var priority = this.settings.Priority.FirstOrDefault();

            // determine if a nuget file was found
            if (priority == null)
            {
                // log a warning
                this.Log.LogWarning($"No configuration file could be found for repository {this.RepositoryRoot}");

                // move on immediately
                return false;
            }

            // set the nuget config path
            this.NuGetConfigPath = Path.Combine(priority.Root, priority.FileName);

            // get the package sources
            var sources = this.provider.LoadPackageSources();

            // iterate over each source name
            foreach (var name in this.Sources.Select(s => s.ItemSpec))
            {
                // get the current source
                var source = sources.FirstOrDefault
                (
                    s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase)
                    || s.Source.Equals(name, StringComparison.OrdinalIgnoreCase)
                );

                // determine if a source was found
                if (source == null)
                {
                    // log a warning
                    this.Log.LogWarning($"A source with name {name} does not exist within the nuget configuration.");

                    // move on immediately
                    return false;
                }

                // set the credentials on the source
                source.Credentials = new PackageSourceCredential(source.Name, this.Username, this.Password, true);

                // log a message
                this.Log.LogMessage(MessageImportance.High, $"Set credentials on {source.Name} ({source.Source})");
            }

            // save the modified sources (to the credential config)
            this.provider.SavePackageSources(sources);

            // return
            return true;
        }
        #endregion
    }
}
