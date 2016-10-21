namespace PulseBridge.Condo.Build
{
    using System;
    using System.IO;

    using static System.FormattableString;

    using Moq;

    using NuGet.Configuration;
    using NuGet.Frameworks;
    using NuGet.Packaging;
    using NuGet.Versioning;

    public static class NuGetMocks
    {
        public static IPackageSourceProvider CreateProvider(string source, string push)
        {
            // create the mock
            var mock = new Mock<IPackageSourceProvider>();

            // setup the default source
            mock.SetupGet(property => property.DefaultPushSource).Returns(push);

            // define a variable to retain an absolute URL
            Uri url;

            // determine if the uri is not a URL
            if (!Uri.TryCreate(push, UriKind.Absolute, out url))
            {
                // create the directory
                Directory.CreateDirectory(push);
            }

            // return the mock object
            return mock.Object;
        }

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

        private static IPackageFile CreateFile(string name)
        {
            var file = new Mock<IPackageFile>();
            file.SetupGet(f => f.Path).Returns(name);
            file.Setup(f => f.GetStream()).Returns(new MemoryStream());

            string effectivePath;
            var fx = FrameworkNameUtility.ParseFrameworkNameFromFilePath(name, out effectivePath);
            file.SetupGet(f => f.EffectivePath).Returns(effectivePath);
            file.SetupGet(f => f.TargetFramework).Returns(fx);

            return file.Object;
        }
    }
}