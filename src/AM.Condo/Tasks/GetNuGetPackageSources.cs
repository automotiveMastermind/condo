// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetNuGetPackageSources.cs" company="automotiveMastermind and contributors">
// © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AM.Condo.Tasks
{
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;
    using NuGet.Configuration;

    /// <summary>
    /// Represents a NuGet task that gets NuGet package sources from the current configuration.
    /// </summary>
    public class GetNuGetPackageSources : Task
    {
        #region Fields
        private ISettings settings;

        private IPackageSourceProvider provider;
        #endregion

        #region Constructors and Finalizers
        /// <summary>
        /// Initializes a new instance of the <see cref="GetNuGetPackageSources"/> class.
        /// </summary>
        public GetNuGetPackageSources()
            : this(settings: null, provider: null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GetNuGetPackageSources"/> class.
        /// </summary>
        /// <param name="settings">
        /// The settings used to push or restore packages.
        /// </param>
        /// <param name="provider">
        /// The package source provider used to push or restore packages.
        /// </param>
        public GetNuGetPackageSources(ISettings settings, IPackageSourceProvider provider)
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
        /// Gets or sets the sources associated with the NuGet feed.
        /// </summary>
        [Output]
        public ITaskItem[] Sources { get; set; }
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
            // determine if the settings are not specified
            if (this.settings == null)
            {
                // load the settings
                this.settings = Settings.LoadDefaultSettings
                    (
                        this.RepositoryRoot,
                        configFileName: null,
                        machineWideSettings: new NuGetMachineSettings()
                    );
            }

            // determine if the provider is not specified
            if (this.provider == null)
            {
                // create a new package source provider
                this.provider = new PackageSourceProvider(this.settings);
            }

            // collect the sources as a list
            var sources = new HashSet<ITaskItem>();

            // iterate over each package source
            foreach (var source in this.provider.LoadPackageSources())
            {
                // create a new task item
                var item = new TaskItem(source.Name);
                item.SetMetadata(nameof(source.Source), source.Source);
                item.SetMetadata(nameof(source.ProtocolVersion), source.ProtocolVersion.ToString());
                item.SetMetadata(nameof(source.IsEnabled), source.IsEnabled.ToString());

                // add the item to the source collection
                sources.Add(item);
            }

            // set the sources array
            this.Sources = sources.ToArray();

            // return true
            return true;
        }
        #endregion
    }
}
