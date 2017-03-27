// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SetNuGetPackageSourcesTest.cs" company="automotiveMastermind and contributors">
//   © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AM.Condo.Tasks
{
    using Microsoft.Build.Utilities;
    using NuGet.Configuration;
    using Xunit;

    using AM.Condo.IO;

    [Class(nameof(SetNuGetPackageSources))]
    [Purpose(PurposeType.Unit)]
    public class SetNuGetPackageSourcesTest
    {
        [Fact]
        [Priority(1)]
        public void Execute_WithValidCredentials_UpdatesNuGetConfig()
        {
            using (var temp = new TemporaryPath())
            {
                // arrange
                var root = temp.FullPath;
                var source = "https://nuget.localtest.me/blah/blah/mah/mah";
                var prefix = source.Substring(20);
                var name = "test";
                var username = "vsts";
                var password = "example";

                var engine = MSBuildMocks.CreateEngine();
                var settings = NuGetMocks.CreateSettings(root, source, name);
                var provider = new PackageSourceProvider(settings);

                var actual = new SetNuGetPackageSources(settings, provider)
                {
                    RepositoryRoot = root,
                    ArtifactsRoot = root,
                    BuildEngine = engine,
                    Username = username,
                    Password = password,
                    Prefixes = new[] { new TaskItem(prefix) }
                };

                // act
                var result = actual.Execute();

                // assert
                Assert.True(result);
            }
        }
    }
}
