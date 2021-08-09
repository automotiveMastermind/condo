// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NuGetMocks.cs" company="automotiveMastermind and contributors">
//   © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AM.Condo
{
    using System;
    using System.IO;

    using static System.FormattableString;

    using Moq;

    using NuGet.Configuration;
    using NuGet.Frameworks;
    using NuGet.Packaging;
    using NuGet.Versioning;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a set of mocks for use with the <see cref="NuGet"/> API.
    /// </summary>
    public static class NuGetMocks
    {
        /// <summary>
        /// Creates a new mock of a package source provider that uses the specified <paramref name="source"/> and
        /// <paramref name="push"/> locations.
        /// </summary>
        /// <param name="source">
        /// The location of the source feed, which can be any URI or location on a file system.
        /// </param>
        /// <param name="push">
        /// The location to which packages should be pushed, which can be a URI or a location on a file system.
        /// </param>
        /// <returns>
        /// The mock of a package source provider used for testing purposes.
        /// </returns>
        public static IPackageSourceProvider CreateProvider(string source, string push)
        {
            // create the mock
            var mock = new Mock<IPackageSourceProvider>();

            // setup the default source
            mock.SetupGet(property => property.DefaultPushSource).Returns(push);

            // define a variable to retain an absolute URL
            Uri url;

            // determine if the uri is not a URL
            // todo: replace with Uri.UriSchemeFile when it becomes available
            if (!Uri.TryCreate(push, UriKind.Absolute, out url) || url.Scheme.Equals("file"))
            {
                // create the directory
                Directory.CreateDirectory(push);
            }

            // return the mock object
            return mock.Object;
        }

        /// <summary>
        /// Creates a new package with the specified <paramref name="id"/> and <paramref name="version"/> and saves it
        /// at the specified <paramref name="path"/>.
        /// </summary>
        /// <param name="id">
        /// The unique identifier of the package.
        /// </param>
        /// <param name="version">
        /// The semantic version of the package.
        /// </param>
        /// <param name="path">
        /// The path where the package should be saved.
        /// </param>
        /// <returns>
        /// The fully-qualified file path where the newly created package now exists on the file system.
        /// </returns>
        public static string CreatePackage(string id, string version, string path)
        {
            // create  a package builder and set the id and version
            var builder = new PackageBuilder
            {
                Id = id,
                Version = new NuGetVersion(version)
            };

            // define a description (required)
            builder.Description = Invariant($"description for {id} - {version}");

            // allow any framework (testing purposes only)
            var framework = NuGetFramework.AnyFramework;

            // create an example file in the correct path for the framework
            var lib = Invariant($"lib/{framework.GetShortFolderName()}/test.dll");
            builder.Files.Add(NuGetMocks.CreateFile(lib));

            // set the author (testing purposes only)
            builder.Authors.Add("author");

            // create the file nme from the id and version
            var name = Invariant($"{id}.{version}.nupkg");
            var package = Path.Combine(path, name);

            // create the directory (if it doesn't already exist)
            Directory.CreateDirectory(path);

            // crete an empty file
            using (var stream = File.Create(package))
            {
                // save the package to the path
                builder.Save(stream);
            }

            // return the path
            return package;
        }

        public static ISettings CreateSettings(string root, string source, string name)
        {
            // create empty settings
            ISettings settings = new Settings(root, "nuget.config");

            // create a provider
            var provider = new PackageSourceProvider(settings);

            // create a new list for sources
            var sources = new List<PackageSource>(provider.LoadPackageSources())
            {
                // add the new source
                new PackageSource(source, name, true) { ProtocolVersion = 3 }
            };

            // save the sources
            provider.SavePackageSources(sources);

            // load specific settings
            return Settings.LoadSpecificSettings(root, "nuget.config");
        }

        private static IPackageFile CreateFile(string name)
        {
            var file = new Mock<IPackageFile>();
            file.SetupGet(f => f.Path).Returns(name);
            file.Setup(f => f.GetStream()).Returns(new MemoryStream());

            var date = DateTimeOffset.UtcNow;

            string effectivePath;
            var fx = FrameworkNameUtility.ParseNuGetFrameworkFromFilePath(name, out effectivePath);
            file.SetupGet(f => f.EffectivePath).Returns(effectivePath);
            file.SetupGet(f => f.NuGetFramework).Returns(fx);
            file.SetupGet(f => f.LastWriteTime).Returns(date);

            return file.Object;
        }
    }
}
