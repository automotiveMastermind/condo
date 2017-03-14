// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InstallNuGetPackage.cs" company="automotiveMastermind and contributors">
//   © automotiveMastermind and contributors. Licensed under MIT. See LICENSE for details.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AM.Condo.Tasks
{
    using System.IO;
    using System.Threading;

    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;
    using NuGet.Packaging.Core;
    using NuGet.Packaging;

    /// <summary>
    /// Represents a Microsoft Build (MSBuild) task that installs a NuGet package to the specified path.
    /// </summary>
    public class InstallNuGetPackage : Task
    {
        #region Properties
        /// <summary>
        /// Gets or sets the packages that should be installed.
        /// </summary>
        [Required]
        public ITaskItem[] Packages { get; set; }

        /// <summary>
        /// Gets or sets the path to which the packages should be installed.
        /// </summary>
        [Required]
        public string Path { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Installs the <see cref="Packages"/> to the specified <see cref="Path"/>.
        /// </summary>
        /// <returns>
        /// A value indicating whether or not all of the packages were successfully installed.
        /// </returns>
        public override bool Execute()
        {
            // determine if the path does not exist
            if (!Directory.Exists(this.Path))
            {
                // create the path
                Directory.CreateDirectory(this.Path);
            }

            // create a new logger
            var logger = new NuGetMSBuildLogger(this.Log);

            // iterate over each package
            foreach (var package in this.Packages)
            {
                // get the package path
                var path = package.GetMetadata("FullPath");

                // determine if the package exists
                if (!File.Exists(path))
                {
                    // log an error
                    this.Log.LogError($"The package could not be found: {path}");

                    // move on immediately
                    return false;
                }

                // open a file stream
                using (var stream = File.OpenRead(path))
                {
                    PackageIdentity id;

                    // create a package archive reader
                    using (var reader = new PackageArchiveReader(stream, leaveStreamOpen: true))
                    {
                        // get the package identity
                        id = reader.GetIdentity();
                    }

                    // reset the stream
                    stream.Seek(0, SeekOrigin.Begin);

                    // get the context
                    var context = new VersionFolderPathContext
                        (
                            id,
                            this.Path,
                            logger,
                            packageSaveMode: PackageSaveMode.Nupkg | PackageSaveMode.Nuspec,
                            xmlDocFileSaveMode: XmlDocFileSaveMode.None
                        );

                    // install the package
                    PackageExtractor.InstallFromSourceAsync(stream.CopyToAsync, context, CancellationToken.None).Wait();
                }
            }

            // assume we were successful
            return true;
        }
        #endregion
    }
}
