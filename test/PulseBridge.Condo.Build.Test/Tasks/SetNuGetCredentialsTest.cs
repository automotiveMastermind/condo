namespace PulseBridge.Condo.Build.Tasks
{
    using Microsoft.Build.Utilities;
    using Xunit;

    using PulseBridge.Condo.IO;

    [Class(nameof(SetNuGetCredentials))]
    [Purpose(PurposeType.Unit)]
    public class SetNuGetCredentialsTest
    {
        [Fact]
        [Priority(1)]
        public void Execute_WithValidCredentials_UpdatesNuGetConfig()
        {
            using (var temp = new TemporaryPath())
            {
                // arrange
                var root = temp.FullPath;
                var source = "http://nuget.localtest.me";
                var name = "test";
                var username = "vsts";
                var password = "example";

                var engine = MSBuildMocks.CreateEngine();
                var provider = NuGetMocks.CreateSettings(root, source, name);

                var actual = new SetNuGetCredentials
                {
                    RepositoryRoot = root,
                    BuildEngine = engine,
                    Username = username,
                    Password = password,
                    Sources = new[] { new TaskItem(name) }
                };

                // act
                var result = actual.Execute();

                // assert
                Assert.True(result);
            }
        }
    }
}
