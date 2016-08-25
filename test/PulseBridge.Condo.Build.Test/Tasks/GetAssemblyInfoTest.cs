namespace PulseBridge.Condo.Build.Tasks
{
    using System;
    using System.Globalization;

    using Xunit;
    using Xunit.Abstractions;

    public class GetAssemblyInfoTest
    {
        private ITestOutputHelper output;

        public GetAssemblyInfoTest(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        [Priority(2)]
        public void Execute_WhenNotCI_Succeeds()
        {
            // arrange
            var date = new DateTime(2016, 1, 2, 3, 4, 0).ToString("o", CultureInfo.InvariantCulture);
            var buildId = default(string);
            var commitId = default(string);
            var ci = false;

            var expected = new
            {
                SemanticVersion = "1.0.0",
                AssemblyVersion = "1.0.0",
                FileVersion = "1.0.160201.0304",
                InformationalVersion = "1.0.0-alpha-160201-0304",
                BuildQuality = "alpha",
                DateTimeUtc = date,
                CI = ci,

                BuildId = "160201",
                CommitId = "0304"
            };

            var actual = new GetAssemblyInfo
            {
                SemanticVersion = "1.0.0",
                DateTimeUtc = date,
                BuildId = buildId,
                CommitId = commitId,
                CI = ci
            };

            // act
            var result = actual.Execute();

            // assert
            Assert.True(result);
            Assert.Equal(expected.SemanticVersion, actual.SemanticVersion);
            Assert.Equal(expected.AssemblyVersion, actual.AssemblyVersion);
            Assert.Equal(expected.FileVersion, actual.FileVersion);
            Assert.Equal(expected.InformationalVersion, actual.InformationalVersion);
            Assert.Equal(expected.BuildQuality, actual.BuildQuality);
            Assert.Equal(expected.DateTimeUtc, actual.DateTimeUtc);
            Assert.Equal(expected.BuildId, actual.BuildId);
            Assert.Equal(expected.CommitId, actual.CommitId);
            Assert.Equal(expected.CI, actual.CI);
        }

        [Fact]
        [Priority(2)]
        public void Execute_WhenNotCIAndBranchSet_Succeeds()
        {
            // arrange
            var date = new DateTime(2016, 1, 2, 3, 4, 0).ToString("o", CultureInfo.InvariantCulture);
            var buildId = default(string);
            var commitId = default(string);
            var branch = "master";
            var ci = false;

            var expected = new
            {
                SemanticVersion = "1.0.0",
                AssemblyVersion = "1.0.0",
                FileVersion = "1.0.160201.0304",
                InformationalVersion = "1.0.0-alpha-160201-0304",
                BuildQuality = "alpha",
                DateTimeUtc = date,
                CI = ci,
                Branch = branch,

                BuildId = "160201",
                CommitId = "0304"
            };

            var actual = new GetAssemblyInfo
            {
                SemanticVersion = "1.0.0",
                DateTimeUtc = date,
                BuildId = buildId,
                CommitId = commitId,
                Branch = branch,
                CI = ci
            };

            // act
            var result = actual.Execute();

            // assert
            Assert.True(result);
            Assert.Equal(expected.SemanticVersion, actual.SemanticVersion);
            Assert.Equal(expected.AssemblyVersion, actual.AssemblyVersion);
            Assert.Equal(expected.FileVersion, actual.FileVersion);
            Assert.Equal(expected.InformationalVersion, actual.InformationalVersion);
            Assert.Equal(expected.BuildQuality, actual.BuildQuality);
            Assert.Equal(expected.DateTimeUtc, actual.DateTimeUtc);
            Assert.Equal(expected.Branch, actual.Branch);
            Assert.Equal(expected.BuildId, actual.BuildId);
            Assert.Equal(expected.CommitId, actual.CommitId);
            Assert.Equal(expected.CI, actual.CI);
        }

        [Fact]
        [Priority(2)]
        public void Execute_WhenCI_Succeeds()
        {
            // arrange
            var date = new DateTime(2016, 1, 2, 3, 4, 0).ToString("o", CultureInfo.InvariantCulture);
            var buildId = default(string);
            var commitId = default(string);
            var ci = true;

            var expected = new
            {
                SemanticVersion = "1.0.0",
                AssemblyVersion = "1.0.0",
                FileVersion = "1.0.160201.0304",
                InformationalVersion = "1.0.0-alpha-160201",
                BuildQuality = "alpha",
                DateTimeUtc = date,
                CI = ci,

                BuildId = "160201",
                CommitId = "0304"
            };

            var actual = new GetAssemblyInfo
            {
                SemanticVersion = "1.0.0",
                DateTimeUtc = date,
                BuildId = buildId,
                CommitId = commitId,
                CI = ci
            };

            // act
            var result = actual.Execute();

            // assert
            Assert.True(result);
            Assert.Equal(expected.SemanticVersion, actual.SemanticVersion);
            Assert.Equal(expected.AssemblyVersion, actual.AssemblyVersion);
            Assert.Equal(expected.FileVersion, actual.FileVersion);
            Assert.Equal(expected.InformationalVersion, actual.InformationalVersion);
            Assert.Equal(expected.BuildQuality, actual.BuildQuality);
            Assert.Equal(expected.DateTimeUtc, actual.DateTimeUtc);
            Assert.Equal(expected.BuildId, actual.BuildId);
            Assert.Equal(expected.CommitId, actual.CommitId);
            Assert.Equal(expected.CI, actual.CI);
        }

        [Fact]
        [Priority(2)]
        public void Execute_WhenCommitSet_Succeeds()
        {
            // arrange
            var date = new DateTime(2016, 1, 2, 3, 4, 0).ToString("o", CultureInfo.InvariantCulture);
            var buildId = default(string);
            var commitId = "98";
            var ci = true;

            var expected = new
            {
                SemanticVersion = "1.0.0",
                AssemblyVersion = "1.0.0",
                FileVersion = "1.0.160201.0304",
                InformationalVersion = "1.0.0-alpha-160201",
                BuildQuality = "alpha",
                DateTimeUtc = date,
                CI = ci,

                BuildId = "160201",
                CommitId = commitId
            };

            var actual = new GetAssemblyInfo
            {
                SemanticVersion = "1.0.0",
                DateTimeUtc = date,
                BuildId = buildId,
                CommitId = commitId,
                CI = ci
            };

            // act
            var result = actual.Execute();

            // assert
            Assert.True(result);
            Assert.Equal(expected.SemanticVersion, actual.SemanticVersion);
            Assert.Equal(expected.AssemblyVersion, actual.AssemblyVersion);
            Assert.Equal(expected.FileVersion, actual.FileVersion);
            Assert.Equal(expected.InformationalVersion, actual.InformationalVersion);
            Assert.Equal(expected.BuildQuality, actual.BuildQuality);
            Assert.Equal(expected.DateTimeUtc, actual.DateTimeUtc);
            Assert.Equal(expected.BuildId, actual.BuildId);
            Assert.Equal(expected.CommitId, actual.CommitId);
            Assert.Equal(expected.CI, actual.CI);
        }

        [Fact]
        [Priority(2)]
        public void Execute_WhenBuildSet_Succeeds()
        {
            // arrange
            var date = new DateTime(2016, 1, 2, 3, 4, 0).ToString("o", CultureInfo.InvariantCulture);
            var buildId = "99";
            var commitId = default(string);
            var ci = true;

            var expected = new
            {
                SemanticVersion = "1.0.0",
                AssemblyVersion = "1.0.0",
                FileVersion = "1.0.99.0304",
                InformationalVersion = "1.0.0-alpha-00099",
                BuildQuality = "alpha",
                DateTimeUtc = date,
                CI = ci,

                BuildId = "99",
                CommitId = "0304"
            };

            var actual = new GetAssemblyInfo
            {
                SemanticVersion = "1.0.0",
                DateTimeUtc = date,
                BuildId = buildId,
                CommitId = commitId,
                CI = ci
            };

            // act
            var result = actual.Execute();

            // assert
            Assert.True(result);
            Assert.Equal(expected.SemanticVersion, actual.SemanticVersion);
            Assert.Equal(expected.AssemblyVersion, actual.AssemblyVersion);
            Assert.Equal(expected.FileVersion, actual.FileVersion);
            Assert.Equal(expected.InformationalVersion, actual.InformationalVersion);
            Assert.Equal(expected.BuildQuality, actual.BuildQuality);
            Assert.Equal(expected.DateTimeUtc, actual.DateTimeUtc);
            Assert.Equal(expected.BuildId, actual.BuildId);
            Assert.Equal(expected.CommitId, actual.CommitId);
            Assert.Equal(expected.CI, actual.CI);
        }

        [Fact]
        [Priority(2)]
        public void Execute_WhenDevelopBranch_Succeeds()
        {
            // arrange
            var date = new DateTime(2016, 1, 2, 3, 4, 0).ToString("o", CultureInfo.InvariantCulture);
            var buildId = default(string);
            var commitId = default(string);
            var branch = "develop";
            var ci = true;

            var expected = new
            {
                SemanticVersion = "1.0.0",
                AssemblyVersion = "1.0.0",
                FileVersion = "1.0.160201.0304",
                InformationalVersion = "1.0.0-beta-160201",
                BuildQuality = "beta",
                DateTimeUtc = date,
                CI = ci,
                Branch = branch,

                BuildId = "160201",
                CommitId = "0304"
            };

            var actual = new GetAssemblyInfo
            {
                SemanticVersion = "1.0.0",
                DateTimeUtc = date,
                BuildId = buildId,
                CommitId = commitId,
                Branch = branch,
                CI = ci
            };

            // act
            var result = actual.Execute();

            // assert
            Assert.True(result);
            Assert.Equal(expected.SemanticVersion, actual.SemanticVersion);
            Assert.Equal(expected.AssemblyVersion, actual.AssemblyVersion);
            Assert.Equal(expected.FileVersion, actual.FileVersion);
            Assert.Equal(expected.InformationalVersion, actual.InformationalVersion);
            Assert.Equal(expected.BuildQuality, actual.BuildQuality);
            Assert.Equal(expected.DateTimeUtc, actual.DateTimeUtc);
            Assert.Equal(expected.Branch, actual.Branch);
            Assert.Equal(expected.BuildId, actual.BuildId);
            Assert.Equal(expected.CommitId, actual.CommitId);
            Assert.Equal(expected.CI, actual.CI);
        }

        [Fact]
        [Priority(2)]
        public void Execute_WhenFeatureBranch_Succeeds()
        {
            // arrange
            var date = new DateTime(2016, 1, 2, 3, 4, 0).ToString("o", CultureInfo.InvariantCulture);
            var buildId = default(string);
            var commitId = default(string);
            var branch = "feature/01-test";
            var ci = true;

            var expected = new
            {
                SemanticVersion = "1.0.0",
                AssemblyVersion = "1.0.0",
                FileVersion = "1.0.160201.0304",
                InformationalVersion = "1.0.0-alpha-160201",
                BuildQuality = "alpha",
                DateTimeUtc = date,
                CI = ci,
                Branch = branch,

                BuildId = "160201",
                CommitId = "0304"
            };

            var actual = new GetAssemblyInfo
            {
                SemanticVersion = "1.0.0",
                DateTimeUtc = date,
                BuildId = buildId,
                CommitId = commitId,
                Branch = branch,
                CI = ci
            };

            // act
            var result = actual.Execute();

            // assert
            Assert.True(result);
            Assert.Equal(expected.SemanticVersion, actual.SemanticVersion);
            Assert.Equal(expected.AssemblyVersion, actual.AssemblyVersion);
            Assert.Equal(expected.FileVersion, actual.FileVersion);
            Assert.Equal(expected.InformationalVersion, actual.InformationalVersion);
            Assert.Equal(expected.BuildQuality, actual.BuildQuality);
            Assert.Equal(expected.DateTimeUtc, actual.DateTimeUtc);
            Assert.Equal(expected.Branch, actual.Branch);
            Assert.Equal(expected.BuildId, actual.BuildId);
            Assert.Equal(expected.CommitId, actual.CommitId);
            Assert.Equal(expected.CI, actual.CI);
        }

        [Fact]
        [Priority(2)]
        public void Execute_WhenReleaseBranch_Succeeds()
        {
            // arrange
            var date = new DateTime(2016, 1, 2, 3, 4, 0).ToString("o", CultureInfo.InvariantCulture);
            var buildId = default(string);
            var commitId = default(string);
            var branch = "release/1.0.0";
            var ci = true;

            var expected = new
            {
                SemanticVersion = "1.0.0",
                AssemblyVersion = "1.0.0",
                FileVersion = "1.0.160201.0304",
                InformationalVersion = "1.0.0-rc-160201",
                BuildQuality = "rc",
                DateTimeUtc = date,
                CI = ci,
                Branch = branch,

                BuildId = "160201",
                CommitId = "0304"
            };

            var actual = new GetAssemblyInfo
            {
                SemanticVersion = "1.0.0",
                DateTimeUtc = date,
                BuildId = buildId,
                CommitId = commitId,
                Branch = branch,
                CI = ci
            };

            // act
            var result = actual.Execute();

            // assert
            Assert.True(result);
            Assert.Equal(expected.SemanticVersion, actual.SemanticVersion);
            Assert.Equal(expected.AssemblyVersion, actual.AssemblyVersion);
            Assert.Equal(expected.FileVersion, actual.FileVersion);
            Assert.Equal(expected.InformationalVersion, actual.InformationalVersion);
            Assert.Equal(expected.BuildQuality, actual.BuildQuality);
            Assert.Equal(expected.DateTimeUtc, actual.DateTimeUtc);
            Assert.Equal(expected.Branch, actual.Branch);
            Assert.Equal(expected.BuildId, actual.BuildId);
            Assert.Equal(expected.CommitId, actual.CommitId);
            Assert.Equal(expected.CI, actual.CI);
        }

        [Fact]
        [Priority(2)]
        public void Execute_WhenMasterBranch_Succeeds()
        {
            // arrange
            var date = new DateTime(2016, 1, 2, 3, 4, 0).ToString("o", CultureInfo.InvariantCulture);
            var buildId = default(string);
            var commitId = default(string);
            var branch = "master";
            var ci = true;

            var expected = new
            {
                SemanticVersion = "1.0.0",
                AssemblyVersion = "1.0.0",
                FileVersion = "1.0.160201.0304",
                InformationalVersion = "1.0.0",
                BuildQuality = default(string),
                DateTimeUtc = date,
                CI = ci,
                Branch = branch,

                BuildId = "160201",
                CommitId = "0304"
            };

            var actual = new GetAssemblyInfo
            {
                SemanticVersion = "1.0.0",
                DateTimeUtc = date,
                BuildId = buildId,
                CommitId = commitId,
                Branch = branch,
                CI = ci
            };

            // act
            var result = actual.Execute();

            // assert
            Assert.True(result);
            Assert.Equal(expected.SemanticVersion, actual.SemanticVersion);
            Assert.Equal(expected.AssemblyVersion, actual.AssemblyVersion);
            Assert.Equal(expected.FileVersion, actual.FileVersion);
            Assert.Equal(expected.InformationalVersion, actual.InformationalVersion);
            Assert.Equal(expected.BuildQuality, actual.BuildQuality);
            Assert.Equal(expected.DateTimeUtc, actual.DateTimeUtc);
            Assert.Equal(expected.Branch, actual.Branch);
            Assert.Equal(expected.BuildId, actual.BuildId);
            Assert.Equal(expected.CommitId, actual.CommitId);
            Assert.Equal(expected.CI, actual.CI);
        }

        [Fact]
        [Priority(2)]
        public void Execute_WhenMainBranch_Succeeds()
        {
            // arrange
            var date = new DateTime(2016, 1, 2, 3, 4, 0).ToString("o", CultureInfo.InvariantCulture);
            var buildId = default(string);
            var commitId = default(string);
            var branch = "main";
            var ci = true;

            var expected = new
            {
                SemanticVersion = "1.0.0",
                AssemblyVersion = "1.0.0",
                FileVersion = "1.0.160201.0304",
                InformationalVersion = "1.0.0",
                BuildQuality = default(string),
                DateTimeUtc = date,
                CI = ci,
                Branch = branch,

                BuildId = "160201",
                CommitId = "0304"
            };

            var actual = new GetAssemblyInfo
            {
                SemanticVersion = "1.0.0",
                DateTimeUtc = date,
                BuildId = buildId,
                CommitId = commitId,
                Branch = branch,
                CI = ci
            };

            // act
            var result = actual.Execute();

            // assert
            Assert.True(result);
            Assert.Equal(expected.SemanticVersion, actual.SemanticVersion);
            Assert.Equal(expected.AssemblyVersion, actual.AssemblyVersion);
            Assert.Equal(expected.FileVersion, actual.FileVersion);
            Assert.Equal(expected.InformationalVersion, actual.InformationalVersion);
            Assert.Equal(expected.BuildQuality, actual.BuildQuality);
            Assert.Equal(expected.DateTimeUtc, actual.DateTimeUtc);
            Assert.Equal(expected.Branch, actual.Branch);
            Assert.Equal(expected.BuildId, actual.BuildId);
            Assert.Equal(expected.CommitId, actual.CommitId);
            Assert.Equal(expected.CI, actual.CI);
        }

        [Fact]
        [Priority(2)]
        public void Execute_WhenHotfixBranch_Succeeds()
        {
            // arrange
            var date = new DateTime(2016, 1, 2, 3, 4, 0).ToString("o", CultureInfo.InvariantCulture);
            var buildId = default(string);
            var commitId = default(string);
            var branch = "hotfix/1.0.0";
            var ci = true;

            var expected = new
            {
                SemanticVersion = "1.0.0",
                AssemblyVersion = "1.0.0",
                FileVersion = "1.0.160201.0304",
                InformationalVersion = "1.0.0-rc-160201",
                BuildQuality = "rc",
                DateTimeUtc = date,
                CI = ci,
                Branch = branch,

                BuildId = "160201",
                CommitId = "0304"
            };

            var actual = new GetAssemblyInfo
            {
                SemanticVersion = "1.0.0",
                DateTimeUtc = date,
                BuildId = buildId,
                CommitId = commitId,
                Branch = branch,
                CI = ci
            };

            // act
            var result = actual.Execute();

            // assert
            Assert.True(result);
            Assert.Equal(expected.SemanticVersion, actual.SemanticVersion);
            Assert.Equal(expected.AssemblyVersion, actual.AssemblyVersion);
            Assert.Equal(expected.FileVersion, actual.FileVersion);
            Assert.Equal(expected.InformationalVersion, actual.InformationalVersion);
            Assert.Equal(expected.BuildQuality, actual.BuildQuality);
            Assert.Equal(expected.DateTimeUtc, actual.DateTimeUtc);
            Assert.Equal(expected.Branch, actual.Branch);
            Assert.Equal(expected.BuildId, actual.BuildId);
            Assert.Equal(expected.CommitId, actual.CommitId);
            Assert.Equal(expected.CI, actual.CI);
        }
    }
}