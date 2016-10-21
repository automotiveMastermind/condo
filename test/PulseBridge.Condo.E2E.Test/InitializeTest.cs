namespace PulseBridge.Condo
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Threading.Tasks;

    using static System.FormattableString;

    using Xunit;
    using Xunit.Abstractions;

    using PulseBridge.Condo.IO;

    [Agent(AgentType.Local)]
    [Purpose(PurposeType.EndToEnd)]
    public class InitializeTest
    {
        private ITestOutputHelper output;

        public InitializeTest(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        [Priority(2)]
        [Platform(PlatformType.MacOS)]
        public async Task Initialize_OnTfsMacAgent_Succeeds()
        {
            using (var repo = new GitRepository().Initialize().Commit("initial"))
            {
                // arrange
                var definitionId = Guid.NewGuid().ToString();
                var ci = "true";
                var version = "1.0.0";
                var prId = "19";
                var branch = $"refs/pull/{prId}";
                var buildId = "1430";
                var commitId = "HASH";
                var buildFor = "E2E_User";
                var agent = "E2E_Machine";
                var provider = "TfsGit";
                var project = "PulseBridge.Condo.Build";
                var teamUri = "https://pulsebridge.visualstudio.com/";
                var repoUri = "https://pulsebridge.visualstudio.com/_git/condo";
                var buildUri = "vstfs:///pulsebridge/Build/1430";
                var buildName = "condo-ci-1430";
                var platform = "macOS";
                var configuration = "release";

                var cmd = "condo.sh";
                var working = repo.RepositoryPath;

                var root = Directory.GetCurrentDirectory();

                while (!File.Exists(Path.Combine(root, cmd)))
                {
                    root = Directory.GetParent(root).FullName;
                }

                repo.Condo(root);

                var args = $@"--path ""{root}/src/PulseBridge.Condo.Build"" --no-color --verbosity minimal -t:initialize "
                    + $"-p:TF_BUILD={ci} "
                    + $"-p:BUILD_VERSION={version} "
                    + $"-p:BUILD_SOURCEBRANCH={branch} "
                    + $"-p:BUILD_SOURCEVERSION={commitId} "
                    + $"-p:BUILD_BUILDID={buildId} "
                    + $"-p:BUILD_REQUESTEDFOR={buildFor} "
                    + $"-p:AGENT_NAME={agent} "
                    + $"-p:BUILD_REPOSITORY_PROVIDER={provider} "
                    + $"-p:SYSTEM_TEAMPROJECT={project} "
                    + $"-p:SYSTEM_TEAMFOUNDATIONCOLLECTIONURI={teamUri} "
                    + $"-p:BUILD_REPOSITORY_URI={repoUri} "
                    + $"-p:BUILD_BUILDURI={buildUri} "
                    + $"-p:BUILD_BUILDNUMBER={buildName} "
                    + $"-p:CONFIGURATION={configuration}";

                var start = new ProcessStartInfo
                {
                    FileName = Path.Combine(working, "condo.sh"),
                    Arguments = args,
                    WorkingDirectory = working,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                this.output.WriteLine(Invariant($"REPOSITORY PATH: ${working}"));

                // act
                var process = Process.Start(start);
                process.WaitForExit();

                var actual = await process.StandardOutput.ReadToEndAsync();

                // assert
                Assert.True(process.HasExited);
                Assert.Equal(0, process.ExitCode);

                Assert.Contains($"Build Quality      : alpha", actual);
                Assert.Contains($"Project Name       : {project}", actual);
                Assert.Contains($"Branch             : {branch}", actual);
                Assert.Contains($"Platform           : {platform}", actual);
                Assert.Contains($"Build URI          : {buildUri}", actual);
                Assert.Contains($"Team URI           : {teamUri}", actual);
                Assert.Contains($"Repository URI     : {repoUri}", actual);
                Assert.Contains($"Commit ID          : {commitId}", actual);
                Assert.Contains($"Build ID           : {buildId}", actual);
                Assert.Contains($"Build Name         : {buildName}", actual);
                Assert.Contains($"Pull Request ID    : {prId}", actual);
                Assert.Contains($"Build On (Agent)   : {agent}", actual);
                Assert.Contains($"Build For          : {buildFor}", actual);
                Assert.Contains($"Configuration      : {configuration}", actual);
            }
        }
    }
}