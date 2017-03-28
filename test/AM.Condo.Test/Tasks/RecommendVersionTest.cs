// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RecommendVersionTest.cs" company="automotiveMastermind and contributors">
//   © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AM.Condo.Tasks
{
    using System;
    using System.Collections.Generic;

    using Condo.IO;

    using NuGet.Versioning;

    using Xunit;

    [Class(nameof(RecommendVersion))]
    public class RecommendVersionTest
    {
        private readonly IGitRepositoryFactory repository = new GitRepositoryFactory();

        [Fact]
        [Priority(1)]
        [Purpose(PurposeType.Unit)]
        public void Execute_WithNullLog_Throws()
        {
            // arrange
            var gitlog = default(GitLog);

            var engine = MSBuildMocks.CreateEngine();

            // act
            Action act = () => new RecommendVersion(gitlog);

            // assert
            Assert.Throws<ArgumentNullException>(nameof(gitlog), act);
        }

        [Fact]
        [Priority(1)]
        [Purpose(PurposeType.Unit)]
        public void Execute_WhenInitialRelease_IncrementsMinorSegment()
        {
            // arrange
            var buildQuality = "alpha";
            var releaseBranchBuildQuality = "rc";

            var latestVersion = new SemanticVersion(0, 2, 0, buildQuality);
            var latestVersionHash = "2";

            var type = "feat";

            var commit = new GitCommit
            {
                Hash = latestVersionHash,
                HeaderCorrespondence = { { nameof(type), type } }
            };

            var gitlog = new GitLog
            {
                Versions = { { latestVersion, new List<GitCommit> { commit } } },
                Commits = { commit }
            };

            var engine = MSBuildMocks.CreateEngine();

            var expected = new
            {
                CurrentRelease = "0.2.0",
                RecommendedRelease = "0.3.0",
                NextRelease = "1.0.0"
            };

            var actual = new RecommendVersion(gitlog)
            {
                BuildEngine = engine,
                BuildQuality = buildQuality,
                ReleaseBranchBuildQuality = releaseBranchBuildQuality,
                MinorCorrespondence = nameof(type),
                MinorValue = "feat",
                LatestVersion = latestVersion.ToFullString(),
                LatestVersionCommit = latestVersionHash
            };

            // act
            var result = actual.Execute();

            // assert
            Assert.True(result);
            Assert.Equal(expected.CurrentRelease, actual.CurrentRelease);
            Assert.Equal(expected.RecommendedRelease, actual.RecommendedRelease);
            Assert.Equal(expected.NextRelease, actual.NextRelease);
        }

        [Fact]
        [Priority(1)]
        [Purpose(PurposeType.Unit)]
        public void Execute_WhenInitialReleaseOnReleaseBranch_IncrementsMajorSegment()
        {
            // arrange
            var buildQuality = "rc";
            var releaseBranchBuildQuality = "rc";
            var latestVersion = new SemanticVersion(0, 2, 0, buildQuality);
            var latestVersionHash = "2";

            var type = "bugfix";

            var commit = new GitCommit
            {
                Hash = latestVersionHash,
                HeaderCorrespondence = { { nameof(type), type } },
                Notes = { new GitNote { Header = "BREAKING CHANGE" } }
            };

            var gitlog = new GitLog
            {
                Versions = { { latestVersion, new List<GitCommit> { commit } } },
                Commits = { commit }
            };

            var engine = MSBuildMocks.CreateEngine();

            var expected = new
            {
                CurrentRelease = "0.2.0",
                RecommendedRelease = "1.0.0",
                NextRelease = "1.0.0"
            };

            var actual = new RecommendVersion(gitlog)
            {
                BuildEngine = engine,
                BuildQuality = buildQuality,
                ReleaseBranchBuildQuality = releaseBranchBuildQuality,
                MinorCorrespondence = nameof(type),
                MinorValue = "feat",
                LatestVersion = latestVersion.ToFullString(),
                LatestVersionCommit = latestVersionHash
            };

            // act
            var result = actual.Execute();

            // assert
            Assert.True(result);
            Assert.Equal(expected.CurrentRelease, actual.CurrentRelease);
            Assert.Equal(expected.RecommendedRelease, actual.RecommendedRelease);
            Assert.Equal(expected.NextRelease, actual.NextRelease);
        }

        [Fact]
        [Priority(1)]
        [Purpose(PurposeType.Unit)]
        public void Execute_WhenAdditionalReleaseOnReleaseBranch_DoesNotIncrementAndEmitsNoWarnings()
        {
            // arrange
            var buildQuality = "rc";
            var releaseBranchBuildQuality = "rc";
            var latestVersion = new SemanticVersion(1, 0, 0, buildQuality);
            var latestVersionHash = "2";

            var type = "bugfix";

            var commit = new GitCommit
            {
                Hash = latestVersionHash,
                HeaderCorrespondence = { { nameof(type), type } },
                Notes = { new GitNote { Header = "BREAKING CHANGE" } }
            };

            var gitlog = new GitLog
            {
                Versions = { { latestVersion, new List<GitCommit> { commit } } },
                Commits = { commit }
            };

            var engine = MSBuildMocks.CreateEngine();

            var expected = new
            {
                CurrentRelease = "1.0.0",
                RecommendedRelease = "1.0.0",
                NextRelease = "1.0.0"
            };

            var actual = new RecommendVersion(gitlog)
            {
                BuildEngine = engine,
                BuildQuality = buildQuality,
                ReleaseBranchBuildQuality = releaseBranchBuildQuality,
                MinorCorrespondence = nameof(type),
                MinorValue = "feat",
                LatestVersion = latestVersion.ToFullString(),
                LatestVersionCommit = latestVersionHash
            };

            // act
            var result = actual.Execute();

            // assert
            Assert.True(result);
            Assert.Equal(expected.CurrentRelease, actual.CurrentRelease);
            Assert.Equal(expected.RecommendedRelease, actual.RecommendedRelease);
            Assert.Equal(expected.NextRelease, actual.NextRelease);
        }

        [Fact]
        [Priority(1)]
        [Purpose(PurposeType.Unit)]
        public void Execute_WhenInitialReleaseOnProductionBranch_IncrementsMajorSegment()
        {
            // arrange
            var buildQuality = string.Empty;
            var releaseBranchBuildQuality = "rc";
            var latestVersion = new SemanticVersion(0, 2, 0, buildQuality);
            var latestVersionHash = "2";

            var type = "bugfix";

            var commit = new GitCommit
            {
                Hash = latestVersionHash,
                HeaderCorrespondence = { { nameof(type), type } }
            };

            var gitlog = new GitLog
            {
                Versions = { { latestVersion, new List<GitCommit> { commit } } },
                Commits = { commit }
            };

            var engine = MSBuildMocks.CreateEngine();

            var expected = new
            {
                CurrentRelease = "0.2.0",
                RecommendedRelease = "1.0.0",
                NextRelease = "1.0.0"
            };

            var actual = new RecommendVersion(gitlog)
            {
                BuildEngine = engine,
                BuildQuality = buildQuality,
                ReleaseBranchBuildQuality = releaseBranchBuildQuality,
                MinorCorrespondence = nameof(type),
                MinorValue = "feat",
                LatestVersion = latestVersion.ToFullString(),
                LatestVersionCommit = latestVersionHash
            };

            // act
            var result = actual.Execute();

            // assert
            Assert.True(result);
            Assert.Equal(expected.CurrentRelease, actual.CurrentRelease);
            Assert.Equal(expected.RecommendedRelease, actual.RecommendedRelease);
            Assert.Equal(expected.NextRelease, actual.NextRelease);
        }

        [Fact]
        [Priority(1)]
        [Purpose(PurposeType.Unit)]
        public void Execute_WhenReleaseAfterFirstReleaseWithBreakingChangeInPreviousCommit_IncrementsMajorSegment()
        {
            // arrange
            var buildQuality = "alpha";
            var releaseBranchBuildQuality = "rc";
            var releaseVersion = new SemanticVersion(1, 0, 0);
            var releaseVersionHash = "1";

            var latestVersion = new SemanticVersion(1, 0, 1, "beta");
            var latestVersionHash = "2";

            var type = "bugfix";

            var releaseCommit = new GitCommit
            {
                Hash = releaseVersionHash,
                HeaderCorrespondence = { { nameof(type), "chore" } }
            };

            var previousCommit = new GitCommit
            {
                Hash = "2",
                HeaderCorrespondence = { { nameof(type), type } },
                Notes = { new GitNote { Header = "BREAKING CHANGE" } }
            };

            var currentCommit = new GitCommit
            {
                Hash = latestVersionHash,
                HeaderCorrespondence = { { nameof(type), type } }
            };

            var gitlog = new GitLog
            {
                Versions =
                {
                     { releaseVersion, new List<GitCommit> { releaseCommit } },
                     { latestVersion, new List<GitCommit> { currentCommit } }
                },
                Commits = { releaseCommit, previousCommit, currentCommit }
            };

            var engine = MSBuildMocks.CreateEngine();

            var expected = new
            {
                CurrentRelease = "1.0.1",
                RecommendedRelease = "2.0.0",
                NextRelease = "2.0.0"
            };

            var actual = new RecommendVersion(gitlog)
            {
                BuildEngine = engine,
                BuildQuality = buildQuality,
                ReleaseBranchBuildQuality = releaseBranchBuildQuality,
                MinorCorrespondence = nameof(type),
                MinorValue = "feat",
                LatestVersion = latestVersion.ToFullString(),
                LatestVersionCommit = latestVersionHash
            };

            // act
            var result = actual.Execute();

            // assert
            Assert.True(result);
            Assert.Equal(expected.CurrentRelease, actual.CurrentRelease);
            Assert.Equal(expected.RecommendedRelease, actual.RecommendedRelease);
            Assert.Equal(expected.NextRelease, actual.NextRelease);
        }

        [Fact]
        [Priority(1)]
        [Purpose(PurposeType.Unit)]
        public void Execute_WhenReleaseAfterFirstReleaseWithBugfixInPreviousCommit_IncrementsPatchSegment()
        {
            // arrange
            var buildQuality = "alpha";
            var releaseBranchBuildQuality = "rc";
            var releaseVersion = new SemanticVersion(1, 0, 0);
            var releaseVersionHash = "1";

            var latestVersion = new SemanticVersion(1, 0, 1, "beta");
            var latestVersionHash = "2";

            var type = "bugfix";

            var releaseCommit = new GitCommit
            {
                Hash = releaseVersionHash,
                HeaderCorrespondence = { { nameof(type), "chore" } }
            };

            var previousCommit = new GitCommit
            {
                Hash = "2",
                HeaderCorrespondence = { { nameof(type), type } }
            };

            var currentCommit = new GitCommit
            {
                Hash = latestVersionHash,
                HeaderCorrespondence = { { nameof(type), type } }
            };

            var gitlog = new GitLog
            {
                Versions =
                {
                     { releaseVersion, new List<GitCommit> { releaseCommit } },
                     { latestVersion, new List<GitCommit> { currentCommit } }
                },
                Commits = { releaseCommit, previousCommit, currentCommit }
            };

            var engine = MSBuildMocks.CreateEngine();

            var expected = new
            {
                CurrentRelease = "1.0.1",
                RecommendedRelease = "1.0.1",
                NextRelease = "1.0.1"
            };

            var actual = new RecommendVersion(gitlog)
            {
                BuildEngine = engine,
                BuildQuality = buildQuality,
                ReleaseBranchBuildQuality = releaseBranchBuildQuality,
                MinorCorrespondence = nameof(type),
                MinorValue = "feat",
                LatestVersion = latestVersion.ToFullString(),
                LatestVersionCommit = latestVersionHash
            };

            // act
            var result = actual.Execute();

            // assert
            Assert.True(result);
            Assert.Equal(expected.CurrentRelease, actual.CurrentRelease);
            Assert.Equal(expected.RecommendedRelease, actual.RecommendedRelease);
            Assert.Equal(expected.NextRelease, actual.NextRelease);
        }

        [Fact]
        [Priority(1)]
        [Purpose(PurposeType.Unit)]
        public void Execute_WhenReleaseAfterFirstReleaseWithFeatureInPreviousCommit_IncrementsMinorSegment()
        {
            // arrange
            var buildQuality = "alpha";
            var releaseBranchBuildQuality = "rc";
            var releaseVersion = new SemanticVersion(1, 0, 0);
            var releaseVersionHash = "1";

            var latestVersion = new SemanticVersion(1, 0, 1, "beta");
            var latestVersionHash = "2";

            var type = "feat";

            var releaseCommit = new GitCommit
            {
                Hash = releaseVersionHash,
                HeaderCorrespondence = { { nameof(type), "chore" } }
            };

            var previousCommit = new GitCommit
            {
                Hash = "2",
                HeaderCorrespondence = { { nameof(type), type } }
            };

            var currentCommit = new GitCommit
            {
                Hash = latestVersionHash,
                HeaderCorrespondence = { { nameof(type), type } }
            };

            var gitlog = new GitLog
            {
                Versions =
                {
                     { releaseVersion, new List<GitCommit> { releaseCommit } },
                     { latestVersion, new List<GitCommit> { currentCommit } }
                },
                Commits = { releaseCommit, previousCommit, currentCommit }
            };

            var engine = MSBuildMocks.CreateEngine();

            var expected = new
            {
                CurrentRelease = "1.0.1",
                RecommendedRelease = "1.1.0",
                NextRelease = "1.1.0"
            };

            var actual = new RecommendVersion(gitlog)
            {
                BuildEngine = engine,
                BuildQuality = buildQuality,
                ReleaseBranchBuildQuality = releaseBranchBuildQuality,
                MinorCorrespondence = nameof(type),
                MinorValue = "feat",
                LatestVersion = latestVersion.ToFullString(),
                LatestVersionCommit = latestVersionHash,
            };

            // act
            var result = actual.Execute();

            // assert
            Assert.True(result);
            Assert.Equal(expected.CurrentRelease, actual.CurrentRelease);
            Assert.Equal(expected.RecommendedRelease, actual.RecommendedRelease);
            Assert.Equal(expected.NextRelease, actual.NextRelease);
        }

        [Fact]
        [Priority(1)]
        [Purpose(PurposeType.Unit)]
        public void Execute_WhenReleaseAfterReleaseBranchReleaseWithBreakingChangeInPreviousCommit_DoesNotIncrement()
        {
            // arrange
            var buildQuality = "alpha";
            var releaseBranchBuildQuality = "rc";
            var releaseVersion = new SemanticVersion(1, 0, 0, releaseBranchBuildQuality);
            var releaseVersionHash = "1";

            var type = "feat";

            var releaseCommit = new GitCommit
            {
                Hash = releaseVersionHash,
                HeaderCorrespondence = { { nameof(type), "chore" } }
            };

            var currentCommit = new GitCommit
            {
                Hash = "3",
                HeaderCorrespondence = { { nameof(type), type } },
                Notes = { new GitNote { Header = "BREAKING CHANGE" } }
            };

            var gitlog = new GitLog
            {
                Versions =
                {
                     { releaseVersion, new List<GitCommit> { releaseCommit } }
                },
                Commits = { releaseCommit, currentCommit }
            };

            var engine = MSBuildMocks.CreateEngine();

            var expected = new
            {
                CurrentRelease = "1.0.0",
                RecommendedRelease = "1.0.0",
                NextRelease = "1.0.0"
            };

            var actual = new RecommendVersion(gitlog)
            {
                BuildEngine = engine,
                BuildQuality = buildQuality,
                ReleaseBranchBuildQuality = releaseBranchBuildQuality,
                MinorCorrespondence = nameof(type),
                MinorValue = "feat",
                LatestVersion = releaseVersion.ToString(),
                LatestVersionCommit = releaseVersionHash
            };

            // act
            var result = actual.Execute();

            // assert
            Assert.True(result);
            Assert.Equal(expected.CurrentRelease, actual.CurrentRelease);
            Assert.Equal(expected.RecommendedRelease, actual.RecommendedRelease);
            Assert.Equal(expected.NextRelease, actual.NextRelease);
        }
    }
}
