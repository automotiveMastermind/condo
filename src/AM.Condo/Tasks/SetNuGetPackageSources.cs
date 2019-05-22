// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SetNuGetPackageSources.cs" company="automotiveMastermind and contributors">
// © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AM.Condo.Tasks
{
    using System;
    using System.IO;
    using System.Linq;

    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;
    using NuGet.Configuration;

    /// <summary>
    /// Represents a NuGet task that sets the credentials for a NuGet Package source.
    /// </summary>
    public class SetNuGetPackageSources : Task
    {
        #region Fields
        private ISettings settings;

        private IPackageSourceProvider provider;
        #endregion

        #region Constructors and Finalizers
        /// <summary>
        /// Initializes a new instance of the <see cref="SetNuGetPackageSources"/> class.
        /// </summary>
        public SetNuGetPackageSources()
            : this(settings: null, provider: null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SetNuGetPackageSources"/> class.
        /// </summary>
        /// <param name="settings">
        /// The settings used to push or restore packages.
        /// </param>
        /// <param name="provider">
        /// The package source provider used to push or restore packages.
        /// </param>
        public SetNuGetPackageSources(ISettings settings, IPackageSourceProvider provider)
        {
            // set the settings and package source provider
            this.settings = settings;
            this.provider = provider;
        }
        #endregion

        #region Properties and Indexers
        /// <summary>
        /// Gets or sets the root of the repository.
        /// </summary>
        [Required]
        public string RepositoryRoot { get; set; }

        /// <summary>
        /// Gets or sets the path to the NuGet configuration file that should be used for nuget tasks.
        /// </summary>
        public string ArtifactsRoot { get; set; }

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

        /// <summary>
        /// Gets or sets a value indicating whether or not to store the password in clear text.
        /// </summary>
        public bool IsPasswordClearText { get; set; } = true;

        /// <summary>
        /// Gets or sets the prefixes used to isolate nuget feeds to configure.
        /// </summary>
        public ITaskItem[] Prefixes { get; set; } = new[] { new TaskItem("pkgs.visualstudio.com/") };

        /// <summary>
        /// Gets or sets the file name of the resulting NuGet configuration file.
        /// </summary>
        public string FileName { get; set; } = "nuget.config";

        /// <summary>
        /// Gets or sets the package feed URI.
        /// </summary>
        public string Uri { get; set; }

        /// <summary>
        /// Gets or sets the symbol feed URI.
        /// </summary>
        public string SymbolUri { get; set; }

        /// <summary>
        /// Gets the final configuration path of the nuget file.
        /// </summary>
        [Output]
        public string NuGetConfigPath { get; private set; }
        #endregion

        #region Methods
        /// <summary>
        /// Execute the set credentials task.
        /// </summary>
        /// <returns>
        /// A value indicating whether or not the task executed successfully.
        /// </returns>
        public override bool Execute()
        {
            // get the current settings
            var currentSettings = Settings.LoadDefaultSettings
                    (
                        this.RepositoryRoot,
                        configFileName: null,
                        machineWideSettings: new NuGetMachineSettings()
                    );

            // get a provider for the current settings
            var currentProvider = new PackageSourceProvider(currentSettings);

            // determine if the settings are null
            if (this.settings == null)
            {
                // initialize the settings
                this.settings = string.IsNullOrEmpty(this.ArtifactsRoot)
                    ? currentSettings
                    : new Settings(this.ArtifactsRoot, this.FileName, isMachineWide: false);
            }

            // determine if the provider is null
            if (this.provider == null)
            {
                // initialize the package source provider
                this.provider = new PackageSourceProvider(this.settings);
            }

            // set the nuget config path
            this.NuGetConfigPath = Path.Combine(this.settings.GetConfigRoots()[0], this.settings.GetConfigFilePaths()[0]);

            // collect the prefixes
            var prefixes = this.Prefixes.Select(prefix => prefix.ItemSpec);

            // create a hash set to retain the sources
            var sources = currentProvider.LoadPackageSources().ToList();

            // determine if the uri is specified and not already in the nuget.config
            if (!string.IsNullOrEmpty(this.Uri)
                && !sources.Any(source => source.Source.Equals(this.Uri, StringComparison.OrdinalIgnoreCase)))
            {
                // add the push source
                sources.Add(new PackageSource(this.Uri, "condo-push-source"));
            }

            // determine if the symbol uri is specified and not already in the nuget.config
            if (!string.IsNullOrEmpty(this.SymbolUri)
                && !sources.Any(source => source.Source.Equals(this.SymbolUri, StringComparison.OrdinalIgnoreCase)))
            {
                // add the symbol source
                sources.Add(new PackageSource(this.SymbolUri, "condo-symbol-source"));
            }

            // iterate over each source
            foreach (var source in sources)
            {
                // determine if this is a secure feed
                if (prefixes.Any(prefix => source.Source.Contains(prefix)))
                {
                    // set the credentials on the source
                    source.Credentials = new PackageSourceCredential
                        (source.Name, this.Username, this.Password, this.IsPasswordClearText, validAuthenticationTypesText: null);
                }

                // log a verbose message
                this.Log.LogMessage(MessageImportance.Low, $"Saved source {source.Name} - {source.Source}");
            }

            // save the modified sources (to the new config file)
            this.provider.SavePackageSources(sources);

            // return
            return true;
        }
        #endregion
    }
}
