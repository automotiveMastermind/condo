namespace PulseBridge.Condo.Tasks
{
    using System.Linq;

    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;
    using NuGet.Configuration;

    /// <summary>
    /// Represents a NuGet task that gets NuGet package sources that are Visual Studio Team Services (VSTS) secured
    /// feeds.
    /// </summary>
    public class GetVstsPackageFeeds : Task
    {
        #region Fields
        private const string VstsPackageFeed = "pkgs.visualstudio.com/";

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
        /// Gets or sets the sources
        /// </summary>
        [Output]
        public ITaskItem[] Sources { get; set; }
        #endregion

        #region Constructors and Finalizers
        /// <summary>
        /// Initializes a new instance of the <see cref="GetVstsPackageFeeds"/> class.
        /// </summary>
        public GetVstsPackageFeeds()
            : this(null, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="GetVstsPackageFeeds"/> class.
        /// </summary>
        /// <param name="settings">
        /// The settings used to push or restore packages.
        /// </param>
        /// <param name="provider">
        /// The package source provider used to push or restore packages.
        /// </param>
        public GetVstsPackageFeeds(ISettings settings, IPackageSourceProvider provider)
        {
            // set the settings and package source provider
            this.settings = settings;
            this.provider = provider;
        }

        /// <summary>
        /// Execute the set credentials task.
        /// </summary>
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
                this.provider = new PackageSourceProvider(settings);
            }

            // get the package sources that contains the expected feed uri
            this.Sources = this.provider.LoadPackageSources()
                .Where(source => source.Source.ToLowerInvariant().Contains(VstsPackageFeed))
                .Select(source => new TaskItem(source.Source))
                .ToArray();

            // return
            return true;
        }
        #endregion
    }
}