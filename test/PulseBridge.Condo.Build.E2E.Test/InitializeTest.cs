namespace PulseBridge.Condo.Build.E2E
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Threading.Tasks;

    using PulseBridge.Condo;

    using Xunit;

    public class InitializeTest
    {
        [Fact]
        [FactType(FactType.EndToEnd)]
        [Platform(PlatformType.MacOS)]
        [Priority(2)]
        public async Task Initialize_OnTfsMacAgent_Succeeds()
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

            var args = $"--no-color --verbosity minimal -t:initialize "
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

            var cmd = "condo.sh";
            var working = Directory.GetCurrentDirectory();

            while (!File.Exists(Path.Combine(working, cmd)))
            {
                working = Directory.GetParent(working).FullName;
            }

            cmd = Path.Combine(working, cmd);

            var start = new ProcessStartInfo
            {
                FileName = cmd,
                Arguments = args,
                WorkingDirectory = working,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

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