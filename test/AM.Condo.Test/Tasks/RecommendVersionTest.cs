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
    using Xunit.Abstractions;

    [Class(nameof(RecommendVersion))]
    public class RecommendVersionTest
    {
        private readonly ITestOutputHelper output;

        public RecommendVersionTest(ITestOutputHelper output)
        {
            this.output = output;
        }

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

        [MemberData(nameof(Releases))]
        [Theory]
        [Priority(1)]
        [Purpose(PurposeType.Unit)]
        public void Execute_WhenRelease_RecommendsValidRelease(ReleaseData release)
        {
            // arrange
            var previous = new GitCommit
            {
                Hash = "0"
            };

            var current = new GitCommit
            {
                Hash = "1",
                HeaderCorrespondence = { { nameof(release.Type), release.Type } },
            };

            if (release.BreakingChange)
            {
                current.Notes.Add(new GitNote());
            }

            var log = new GitLog
            {
                Commits = { previous, current }
            };

            if (!string.IsNullOrEmpty(release.CurrentVersion))
            {
                var version = SemanticVersion.Parse(release.CurrentVersion);
                var tag = new GitTag { Hash = previous.Hash, Name = release.CurrentVersion };
                tag.TryParseVersion(versionTagPrefix: null);

                log.Tags.Add(tag);
                log.Versions.Add(version, new List<GitCommit> { previous });
            }

            var engine = MSBuildMocks.CreateEngine(this.output);

            var actual = new RecommendVersion(log)
            {
                BuildEngine = engine,
                BuildQuality = release.BuildQuality,
                MinorCorrespondence = nameof(release.Type),
                MinorValue = "feat",
            };

            // act
            var result = actual.Execute();

            // assert
            Assert.True(result);
            Assert.Equal(release.CurrentVersion ?? "0.0.0", actual.CurrentVersion);
            Assert.Equal(release.CurrentRelease, actual.CurrentRelease);
            Assert.Equal(release.RecommendedRelease, actual.RecommendedRelease);
            Assert.Equal(release.NextRelease, actual.NextRelease);
        }

        public static TheoryData<ReleaseData> Releases => new TheoryData<ReleaseData>
        {
            // BUGFIX: NULL -> ALPHA (DEFAULT)
            new ReleaseData
            {
                Type = "bugfix",
                BuildQuality = "alpha",
                CurrentVersion = null,
                CurrentRelease = "0.0.0",
                RecommendedRelease = "0.0.1",
                NextRelease = "1.0.0",
                BreakingChange = false
            },
            new ReleaseData
            {
                Type = "bugfix",
                BuildQuality = "alpha",
                CurrentVersion = null,
                CurrentRelease = "0.0.0",
                RecommendedRelease = "0.0.1",
                NextRelease = "1.0.0",
                BreakingChange = true
            },
            // BUGFIX: ALPHA -> ALPHA (NO INCREMENT)
            new ReleaseData
            {
                Type = "bugfix",
                BuildQuality = "alpha",
                CurrentVersion = "0.0.1-alpha-1",
                CurrentRelease = "0.0.1",
                RecommendedRelease = "0.0.1",
                NextRelease = "1.0.0",
                BreakingChange = false
            },
            new ReleaseData
            {
                Type = "bugfix",
                BuildQuality = "alpha",
                CurrentVersion = "0.0.1-alpha-2",
                CurrentRelease = "0.0.1",
                RecommendedRelease = "0.0.1",
                NextRelease = "1.0.0",
                BreakingChange = true
            },
            // BUGFIX: BETA -> ALPHA (INCREMENT PATCH)
            new ReleaseData
            {
                Type = "bugfix",
                BuildQuality = "alpha",
                CurrentVersion = "0.0.1-beta-1",
                CurrentRelease = "0.0.1",
                RecommendedRelease = "0.0.2",
                NextRelease = "1.0.0",
                BreakingChange = false
            },
            new ReleaseData
            {
                Type = "bugfix",
                BuildQuality = "alpha",
                CurrentVersion = "0.0.1-beta-2",
                CurrentRelease = "0.0.1",
                RecommendedRelease = "0.0.2",
                NextRelease = "1.0.0",
                BreakingChange = true
            },
            // BUGFIX: HOTFIX -> ALPHA (INCREMENT PATCH)
            new ReleaseData
            {
                Type = "bugfix",
                BuildQuality = "alpha",
                CurrentVersion = "0.0.1-hotfix-1",
                CurrentRelease = "0.0.1",
                RecommendedRelease = "0.0.2",
                NextRelease = "1.0.0",
                BreakingChange = false
            },
            new ReleaseData
            {
                Type = "bugfix",
                BuildQuality = "alpha",
                CurrentVersion = "0.0.1-hotfix-2",
                CurrentRelease = "0.0.1",
                RecommendedRelease = "0.0.2",
                NextRelease = "1.0.0",
                BreakingChange = true
            },
            // BUGFIX: SERVICING -> ALPHA (INCREMENT PATCH)
            new ReleaseData
            {
                Type = "bugfix",
                BuildQuality = "alpha",
                CurrentVersion = "0.0.1-servicing-1",
                CurrentRelease = "0.0.1",
                RecommendedRelease = "0.0.2",
                NextRelease = "1.0.0",
                BreakingChange = false
            },
            new ReleaseData
            {
                Type = "bugfix",
                BuildQuality = "alpha",
                CurrentVersion = "0.0.1-servicing-2",
                CurrentRelease = "0.0.1",
                RecommendedRelease = "0.0.2",
                NextRelease = "1.0.0",
                BreakingChange = true
            },
            // BUGFIX: RC -> ALPHA (INCREMENT PATCH)
            new ReleaseData
            {
                Type = "bugfix",
                BuildQuality = "alpha",
                CurrentVersion = "0.0.1-rc-1",
                CurrentRelease = "0.0.1",
                RecommendedRelease = "0.0.2",
                NextRelease = "1.0.0",
                BreakingChange = false
            },
            new ReleaseData
            {
                Type = "bugfix",
                BuildQuality = "alpha",
                CurrentVersion = "0.0.1-rc-2",
                CurrentRelease = "0.0.1",
                RecommendedRelease = "0.0.2",
                NextRelease = "1.0.0",
                BreakingChange = true
            },
            // BUGFIX: FINAL -> ALPHA (INCREMENT PATCH)
            new ReleaseData
            {
                Type = "bugfix",
                BuildQuality = "alpha",
                CurrentVersion = "1.0.0",
                CurrentRelease = "1.0.0",
                RecommendedRelease = "1.0.1",
                NextRelease = "1.0.1",
                BreakingChange = false
            },
            new ReleaseData
            {
                Type = "bugfix",
                BuildQuality = "alpha",
                CurrentVersion = "1.0.0",
                CurrentRelease = "1.0.0",
                RecommendedRelease = "1.0.1",
                NextRelease = "2.0.0",
                BreakingChange = true
            },
            // FEAT: NULL -> ALPHA (DEFAULT)
            new ReleaseData
            {
                Type = "feat",
                BuildQuality = "alpha",
                CurrentVersion = null,
                CurrentRelease = "0.0.0",
                RecommendedRelease = "0.0.1",
                NextRelease = "1.0.0",
                BreakingChange = false
            },
            new ReleaseData
            {
                Type = "feat",
                BuildQuality = "alpha",
                CurrentVersion = null,
                CurrentRelease = "0.0.0",
                RecommendedRelease = "0.0.1",
                NextRelease = "1.0.0",
                BreakingChange = true
            },
            // FEAT: ALPHA -> ALPHA (NO INCREMENT)
            new ReleaseData
            {
                Type = "feat",
                BuildQuality = "alpha",
                CurrentVersion = "0.0.1-alpha-1",
                CurrentRelease = "0.0.1",
                RecommendedRelease = "0.0.1",
                NextRelease = "1.0.0",
                BreakingChange = false
            },
            new ReleaseData
            {
                Type = "feat",
                BuildQuality = "alpha",
                CurrentVersion = "0.0.1-alpha-2",
                CurrentRelease = "0.0.1",
                RecommendedRelease = "0.0.1",
                NextRelease = "1.0.0",
                BreakingChange = true
            },
            // FEAT: BETA -> ALPHA (INCREMENT PATCH)
            new ReleaseData
            {
                Type = "feat",
                BuildQuality = "alpha",
                CurrentVersion = "0.0.1-beta-1",
                CurrentRelease = "0.0.1",
                RecommendedRelease = "0.0.2",
                NextRelease = "1.0.0",
                BreakingChange = false
            },
            new ReleaseData
            {
                Type = "feat",
                BuildQuality = "alpha",
                CurrentVersion = "0.0.1-beta-2",
                CurrentRelease = "0.0.1",
                RecommendedRelease = "0.0.2",
                NextRelease = "1.0.0",
                BreakingChange = true
            },
            // FEAT: HOTFIX -> ALPHA (INCREMENT PATCH)
            new ReleaseData
            {
                Type = "feat",
                BuildQuality = "alpha",
                CurrentVersion = "0.0.1-hotfix-1",
                CurrentRelease = "0.0.1",
                RecommendedRelease = "0.0.2",
                NextRelease = "1.0.0",
                BreakingChange = false
            },
            new ReleaseData
            {
                Type = "feat",
                BuildQuality = "alpha",
                CurrentVersion = "0.0.1-hotfix-2",
                CurrentRelease = "0.0.1",
                RecommendedRelease = "0.0.2",
                NextRelease = "1.0.0",
                BreakingChange = true
            },
            // FEAT: SERVICING -> ALPHA (INCREMENT PATCH)
            new ReleaseData
            {
                Type = "feat",
                BuildQuality = "alpha",
                CurrentVersion = "0.0.1-servicing-1",
                CurrentRelease = "0.0.1",
                RecommendedRelease = "0.0.2",
                NextRelease = "1.0.0",
                BreakingChange = false
            },
            new ReleaseData
            {
                Type = "feat",
                BuildQuality = "alpha",
                CurrentVersion = "0.0.1-servicing-2",
                CurrentRelease = "0.0.1",
                RecommendedRelease = "0.0.2",
                NextRelease = "1.0.0",
                BreakingChange = true
            },
            // FEAT: RC -> ALPHA (INCREMENT PATCH)
            new ReleaseData
            {
                Type = "feat",
                BuildQuality = "alpha",
                CurrentVersion = "0.0.1-rc-1",
                CurrentRelease = "0.0.1",
                RecommendedRelease = "0.0.2",
                NextRelease = "1.0.0",
                BreakingChange = false
            },
            new ReleaseData
            {
                Type = "feat",
                BuildQuality = "alpha",
                CurrentVersion = "0.0.1-rc-2",
                CurrentRelease = "0.0.1",
                RecommendedRelease = "0.0.2",
                NextRelease = "1.0.0",
                BreakingChange = true
            },
            // FEAT: FINAL -> ALPHA (INCREMENT MINOR)
            new ReleaseData
            {
                Type = "feat",
                BuildQuality = "alpha",
                CurrentVersion = "1.0.0",
                CurrentRelease = "1.0.0",
                RecommendedRelease = "1.0.1",
                NextRelease = "1.1.0",
                BreakingChange = false
            },
            new ReleaseData
            {
                Type = "feat",
                BuildQuality = "alpha",
                CurrentVersion = "1.0.0",
                CurrentRelease = "1.0.0",
                RecommendedRelease = "1.0.1",
                NextRelease = "2.0.0",
                BreakingChange = true
            },
            // BUGFIX: NULL -> BETA (DEFAULT)
            new ReleaseData
            {
                Type = "bugfix",
                BuildQuality = "beta",
                CurrentVersion = null,
                CurrentRelease = "0.0.0",
                RecommendedRelease = "0.0.1",
                NextRelease = "1.0.0",
                BreakingChange = false
            },
            new ReleaseData
            {
                Type = "bugfix",
                BuildQuality = "beta",
                CurrentVersion = null,
                CurrentRelease = "0.0.0",
                RecommendedRelease = "0.0.1",
                NextRelease = "1.0.0",
                BreakingChange = true
            },
            // BUGFIX: ALPHA -> BETA (NO INCREMENT)
            new ReleaseData
            {
                Type = "bugfix",
                BuildQuality = "beta",
                CurrentVersion = "0.0.1-alpha-1",
                CurrentRelease = "0.0.1",
                RecommendedRelease = "0.0.1",
                NextRelease = "1.0.0",
                BreakingChange = false
            },
            new ReleaseData
            {
                Type = "bugfix",
                BuildQuality = "beta",
                CurrentVersion = "0.0.1-alpha-2",
                CurrentRelease = "0.0.1",
                RecommendedRelease = "0.0.1",
                NextRelease = "1.0.0",
                BreakingChange = true
            },
            // BUGFIX: BETA -> BETA (NO INCREMENT)
            new ReleaseData
            {
                Type = "bugfix",
                BuildQuality = "beta",
                CurrentVersion = "0.0.1-beta-1",
                CurrentRelease = "0.0.1",
                RecommendedRelease = "0.0.1",
                NextRelease = "1.0.0",
                BreakingChange = false
            },
            new ReleaseData
            {
                Type = "bugfix",
                BuildQuality = "beta",
                CurrentVersion = "0.0.1-beta-2",
                CurrentRelease = "0.0.1",
                RecommendedRelease = "0.0.1",
                NextRelease = "1.0.0",
                BreakingChange = true
            },
            // BUGFIX: HOTFIX -> BETA (INCREMENT PATCH)
            new ReleaseData
            {
                Type = "bugfix",
                BuildQuality = "beta",
                CurrentVersion = "0.0.1-hotfix-1",
                CurrentRelease = "0.0.1",
                RecommendedRelease = "0.0.2",
                NextRelease = "1.0.0",
                BreakingChange = false
            },
            new ReleaseData
            {
                Type = "bugfix",
                BuildQuality = "beta",
                CurrentVersion = "0.0.1-hotfix-2",
                CurrentRelease = "0.0.1",
                RecommendedRelease = "0.0.2",
                NextRelease = "1.0.0",
                BreakingChange = true
            },
            // BUGFIX: SERVICING -> BETA (INCREMENT PATCH)
            new ReleaseData
            {
                Type = "bugfix",
                BuildQuality = "beta",
                CurrentVersion = "0.0.1-servicing-1",
                CurrentRelease = "0.0.1",
                RecommendedRelease = "0.0.2",
                NextRelease = "1.0.0",
                BreakingChange = false
            },
            new ReleaseData
            {
                Type = "bugfix",
                BuildQuality = "beta",
                CurrentVersion = "0.0.1-servicing-2",
                CurrentRelease = "0.0.1",
                RecommendedRelease = "0.0.2",
                NextRelease = "1.0.0",
                BreakingChange = true
            },
            // BUGFIX: RC -> BETA (INCREMENT PATCH)
            new ReleaseData
            {
                Type = "bugfix",
                BuildQuality = "beta",
                CurrentVersion = "0.0.1-rc-1",
                CurrentRelease = "0.0.1",
                RecommendedRelease = "0.0.2",
                NextRelease = "1.0.0",
                BreakingChange = false
            },
            new ReleaseData
            {
                Type = "bugfix",
                BuildQuality = "beta",
                CurrentVersion = "0.0.1-rc-2",
                CurrentRelease = "0.0.1",
                RecommendedRelease = "0.0.2",
                NextRelease = "1.0.0",
                BreakingChange = true
            },
            // BUGFIX: FINAL -> BETA (INCREMENT PATCH)
            new ReleaseData
            {
                Type = "bugfix",
                BuildQuality = "beta",
                CurrentVersion = "1.0.0",
                CurrentRelease = "1.0.0",
                RecommendedRelease = "1.0.1",
                NextRelease = "1.0.1",
                BreakingChange = false
            },
            new ReleaseData
            {
                Type = "bugfix",
                BuildQuality = "beta",
                CurrentVersion = "1.0.0",
                CurrentRelease = "1.0.0",
                RecommendedRelease = "1.0.1",
                NextRelease = "2.0.0",
                BreakingChange = true
            },
            // FEAT: NULL -> BETA (DEFAULT)
            new ReleaseData
            {
                Type = "feat",
                BuildQuality = "beta",
                CurrentVersion = null,
                CurrentRelease = "0.0.0",
                RecommendedRelease = "0.0.1",
                NextRelease = "1.0.0",
                BreakingChange = false
            },
            new ReleaseData
            {
                Type = "feat",
                BuildQuality = "beta",
                CurrentVersion = null,
                CurrentRelease = "0.0.0",
                RecommendedRelease = "0.0.1",
                NextRelease = "1.0.0",
                BreakingChange = true
            },
            // FEAT: ALPHA -> BETA (NO INCREMENT)
            new ReleaseData
            {
                Type = "feat",
                BuildQuality = "beta",
                CurrentVersion = "0.0.1-alpha-1",
                CurrentRelease = "0.0.1",
                RecommendedRelease = "0.0.1",
                NextRelease = "1.0.0",
                BreakingChange = false
            },
            new ReleaseData
            {
                Type = "feat",
                BuildQuality = "beta",
                CurrentVersion = "0.0.1-alpha-2",
                CurrentRelease = "0.0.1",
                RecommendedRelease = "0.0.1",
                NextRelease = "1.0.0",
                BreakingChange = true
            },
            // FEAT: BETA -> BETA (NO INCREMENT)
            new ReleaseData
            {
                Type = "feat",
                BuildQuality = "beta",
                CurrentVersion = "0.0.1-beta-1",
                CurrentRelease = "0.0.1",
                RecommendedRelease = "0.0.1",
                NextRelease = "1.0.0",
                BreakingChange = false
            },
            new ReleaseData
            {
                Type = "feat",
                BuildQuality = "beta",
                CurrentVersion = "0.0.1-beta-2",
                CurrentRelease = "0.0.1",
                RecommendedRelease = "0.0.1",
                NextRelease = "1.0.0",
                BreakingChange = true
            },
            // FEAT: HOTFIX -> BETA (INCREMENT PATCH)
            new ReleaseData
            {
                Type = "feat",
                BuildQuality = "beta",
                CurrentVersion = "0.0.1-hotfix-1",
                CurrentRelease = "0.0.1",
                RecommendedRelease = "0.0.2",
                NextRelease = "1.0.0",
                BreakingChange = false
            },
            new ReleaseData
            {
                Type = "feat",
                BuildQuality = "beta",
                CurrentVersion = "0.0.1-hotfix-2",
                CurrentRelease = "0.0.1",
                RecommendedRelease = "0.0.2",
                NextRelease = "1.0.0",
                BreakingChange = true
            },
            // FEAT: SERVICING -> BETA (INCREMENT PATCH)
            new ReleaseData
            {
                Type = "feat",
                BuildQuality = "beta",
                CurrentVersion = "0.0.1-servicing-1",
                CurrentRelease = "0.0.1",
                RecommendedRelease = "0.0.2",
                NextRelease = "1.0.0",
                BreakingChange = false
            },
            new ReleaseData
            {
                Type = "feat",
                BuildQuality = "beta",
                CurrentVersion = "0.0.1-servicing-2",
                CurrentRelease = "0.0.1",
                RecommendedRelease = "0.0.2",
                NextRelease = "1.0.0",
                BreakingChange = true
            },
            // FEAT: RC -> BETA (INCREMENT PATCH)
            new ReleaseData
            {
                Type = "feat",
                BuildQuality = "beta",
                CurrentVersion = "0.0.1-rc-1",
                CurrentRelease = "0.0.1",
                RecommendedRelease = "0.0.2",
                NextRelease = "1.0.0",
                BreakingChange = false
            },
            new ReleaseData
            {
                Type = "feat",
                BuildQuality = "beta",
                CurrentVersion = "0.0.1-rc-2",
                CurrentRelease = "0.0.1",
                RecommendedRelease = "0.0.2",
                NextRelease = "1.0.0",
                BreakingChange = true
            },
            // FEAT: FINAL -> BETA (INCREMENT MINOR)
            new ReleaseData
            {
                Type = "feat",
                BuildQuality = "beta",
                CurrentVersion = "1.0.0",
                CurrentRelease = "1.0.0",
                RecommendedRelease = "1.0.1",
                NextRelease = "1.1.0",
                BreakingChange = false
            },
            new ReleaseData
            {
                Type = "feat",
                BuildQuality = "beta",
                CurrentVersion = "1.0.0",
                CurrentRelease = "1.0.0",
                RecommendedRelease = "1.0.1",
                NextRelease = "2.0.0",
                BreakingChange = true
            },
            // BUGFIX: NULL -> FINAL (SET NEXT VERSION)
            new ReleaseData
            {
                Type = "bugfix",
                BuildQuality = "",
                CurrentVersion = null,
                CurrentRelease = "0.0.0",
                RecommendedRelease = "1.0.0",
                NextRelease = "1.0.0",
                BreakingChange = false
            },
            new ReleaseData
            {
                Type = "bugfix",
                BuildQuality = "",
                CurrentVersion = null,
                CurrentRelease = "0.0.0",
                RecommendedRelease = "1.0.0",
                NextRelease = "1.0.0",
                BreakingChange = true
            },
            // BUGFIX: ALPHA -> FINAL (SET NEXT VERSION)
            new ReleaseData
            {
                Type = "bugfix",
                BuildQuality = "",
                CurrentVersion = "0.0.1-alpha-1",
                CurrentRelease = "0.0.1",
                RecommendedRelease = "1.0.0",
                NextRelease = "1.0.0",
                BreakingChange = false
            },
            new ReleaseData
            {
                Type = "bugfix",
                BuildQuality = "",
                CurrentVersion = "1.0.1-alpha-2",
                CurrentRelease = "1.0.1",
                RecommendedRelease = "2.0.0",
                NextRelease = "2.0.0",
                BreakingChange = true
            },
            // BUGFIX: BETA -> FINAL (SET NEXT VERSION)
            new ReleaseData
            {
                Type = "bugfix",
                BuildQuality = "",
                CurrentVersion = "0.0.1-beta-1",
                CurrentRelease = "0.0.1",
                RecommendedRelease = "1.0.0",
                NextRelease = "1.0.0",
                BreakingChange = false
            },
            new ReleaseData
            {
                Type = "bugfix",
                BuildQuality = "",
                CurrentVersion = "1.0.1-beta-2",
                CurrentRelease = "1.0.1",
                RecommendedRelease = "2.0.0",
                NextRelease = "2.0.0",
                BreakingChange = true
            },
            // BUGFIX: HOTFIX -> FINAL (SET NEXT VERSION)
            new ReleaseData
            {
                Type = "bugfix",
                BuildQuality = "",
                CurrentVersion = "0.0.1-hotfix-1",
                CurrentRelease = "0.0.1",
                RecommendedRelease = "1.0.0",
                NextRelease = "1.0.0",
                BreakingChange = false
            },
            new ReleaseData
            {
                Type = "bugfix",
                BuildQuality = "",
                CurrentVersion = "1.0.1-hotfix-2",
                CurrentRelease = "1.0.1",
                RecommendedRelease = "2.0.0",
                NextRelease = "2.0.0",
                BreakingChange = true
            },
            // BUGFIX: SERVICING -> FINAL (SET NEXT VERSION)
            new ReleaseData
            {
                Type = "bugfix",
                BuildQuality = "",
                CurrentVersion = "0.0.1-servicing-1",
                CurrentRelease = "0.0.1",
                RecommendedRelease = "1.0.0",
                NextRelease = "1.0.0",
                BreakingChange = false
            },
            new ReleaseData
            {
                Type = "bugfix",
                BuildQuality = "",
                CurrentVersion = "1.0.1-servicing-2",
                CurrentRelease = "1.0.1",
                RecommendedRelease = "2.0.0",
                NextRelease = "2.0.0",
                BreakingChange = true
            },
            // BUGFIX: RC -> FINAL (SET NEXT VERSION)
            new ReleaseData
            {
                Type = "bugfix",
                BuildQuality = "",
                CurrentVersion = "0.0.1-rc-1",
                CurrentRelease = "0.0.1",
                RecommendedRelease = "1.0.0",
                NextRelease = "1.0.0",
                BreakingChange = false
            },
            new ReleaseData
            {
                Type = "bugfix",
                BuildQuality = "",
                CurrentVersion = "1.0.1-rc-2",
                CurrentRelease = "1.0.1",
                RecommendedRelease = "2.0.0",
                NextRelease = "2.0.0",
                BreakingChange = true
            },
            // BUGFIX: FINAL -> FINAL (SET NEXT VERSION)
            new ReleaseData
            {
                Type = "bugfix",
                BuildQuality = "",
                CurrentVersion = "1.0.0",
                CurrentRelease = "1.0.0",
                RecommendedRelease = "1.0.1",
                NextRelease = "1.0.1",
                BreakingChange = false
            },
            new ReleaseData
            {
                Type = "bugfix",
                BuildQuality = "",
                CurrentVersion = "1.0.0",
                CurrentRelease = "1.0.0",
                RecommendedRelease = "2.0.0",
                NextRelease = "2.0.0",
                BreakingChange = true
            },
            // FEAT: NULL -> FINAL (SET NEXT VERSION)
            new ReleaseData
            {
                Type = "feat",
                BuildQuality = "",
                CurrentVersion = null,
                CurrentRelease = "0.0.0",
                RecommendedRelease = "1.0.0",
                NextRelease = "1.0.0",
                BreakingChange = false
            },
            new ReleaseData
            {
                Type = "feat",
                BuildQuality = "",
                CurrentVersion = null,
                CurrentRelease = "0.0.0",
                RecommendedRelease = "1.0.0",
                NextRelease = "1.0.0",
                BreakingChange = true
            },
            // FEAT: ALPHA -> FINAL (SET NEXT VERSION)
            new ReleaseData
            {
                Type = "feat",
                BuildQuality = "",
                CurrentVersion = "0.0.1-alpha-1",
                CurrentRelease = "0.0.1",
                RecommendedRelease = "1.0.0",
                NextRelease = "1.0.0",
                BreakingChange = false
            },
            new ReleaseData
            {
                Type = "feat",
                BuildQuality = "",
                CurrentVersion = "1.0.1-alpha-2",
                CurrentRelease = "1.0.1",
                RecommendedRelease = "2.0.0",
                NextRelease = "2.0.0",
                BreakingChange = true
            },
            // FEAT: BETA -> FINAL (SET NEXT VERSION)
            new ReleaseData
            {
                Type = "feat",
                BuildQuality = "",
                CurrentVersion = "0.0.1-beta-1",
                CurrentRelease = "0.0.1",
                RecommendedRelease = "1.0.0",
                NextRelease = "1.0.0",
                BreakingChange = false
            },
            new ReleaseData
            {
                Type = "feat",
                BuildQuality = "",
                CurrentVersion = "1.0.1-beta-2",
                CurrentRelease = "1.0.1",
                RecommendedRelease = "2.0.0",
                NextRelease = "2.0.0",
                BreakingChange = true
            },
            // FEAT: HOTFIX -> FINAL (SET NEXT VERSION)
            new ReleaseData
            {
                Type = "feat",
                BuildQuality = "",
                CurrentVersion = "0.0.1-hotfix-1",
                CurrentRelease = "0.0.1",
                RecommendedRelease = "1.0.0",
                NextRelease = "1.0.0",
                BreakingChange = false
            },
            new ReleaseData
            {
                Type = "feat",
                BuildQuality = "",
                CurrentVersion = "1.0.1-hotfix-2",
                CurrentRelease = "1.0.1",
                RecommendedRelease = "2.0.0",
                NextRelease = "2.0.0",
                BreakingChange = true
            },
            // FEAT: SERVICING -> FINAL (SET NEXT VERSION)
            new ReleaseData
            {
                Type = "feat",
                BuildQuality = "",
                CurrentVersion = "0.0.1-servicing-1",
                CurrentRelease = "0.0.1",
                RecommendedRelease = "1.0.0",
                NextRelease = "1.0.0",
                BreakingChange = false
            },
            new ReleaseData
            {
                Type = "feat",
                BuildQuality = "",
                CurrentVersion = "1.0.1-servicing-2",
                CurrentRelease = "1.0.1",
                RecommendedRelease = "2.0.0",
                NextRelease = "2.0.0",
                BreakingChange = true
            },
            // FEAT: RC -> FINAL (SET NEXT VERSION
            new ReleaseData
            {
                Type = "feat",
                BuildQuality = "",
                CurrentVersion = "0.0.1-rc-1",
                CurrentRelease = "0.0.1",
                RecommendedRelease = "1.0.0",
                NextRelease = "1.0.0",
                BreakingChange = false
            },
            new ReleaseData
            {
                Type = "feat",
                BuildQuality = "",
                CurrentVersion = "1.0.1-rc-2",
                CurrentRelease = "1.0.1",
                RecommendedRelease = "2.0.0",
                NextRelease = "2.0.0",
                BreakingChange = true
            },
            // FEAT: FINAL -> FINAL (SET NEXT VERSION)
            new ReleaseData
            {
                Type = "feat",
                BuildQuality = "",
                CurrentVersion = "1.0.0",
                CurrentRelease = "1.0.0",
                RecommendedRelease = "1.1.0",
                NextRelease = "1.1.0",
                BreakingChange = false
            },
            new ReleaseData
            {
                Type = "feat",
                BuildQuality = "",
                CurrentVersion = "1.0.0",
                CurrentRelease = "1.0.0",
                RecommendedRelease = "2.0.0",
                NextRelease = "2.0.0",
                BreakingChange = true
            },
            // BUGFIX: NULL -> FINAL (SET NEXT VERSION)
            new ReleaseData
            {
                Type = "bugfix",
                BuildQuality = "",
                CurrentVersion = null,
                CurrentRelease = "0.0.0",
                RecommendedRelease = "1.0.0",
                NextRelease = "1.0.0",
                BreakingChange = false
            },
            new ReleaseData
            {
                Type = "bugfix",
                BuildQuality = "",
                CurrentVersion = null,
                CurrentRelease = "0.0.0",
                RecommendedRelease = "1.0.0",
                NextRelease = "1.0.0",
                BreakingChange = true
            },
            // BUGFIX: ALPHA -> FINAL (SET NEXT VERSION)
            new ReleaseData
            {
                Type = "bugfix",
                BuildQuality = "",
                CurrentVersion = "0.0.1-alpha-1",
                CurrentRelease = "0.0.1",
                RecommendedRelease = "1.0.0",
                NextRelease = "1.0.0",
                BreakingChange = false
            },
            new ReleaseData
            {
                Type = "bugfix",
                BuildQuality = "",
                CurrentVersion = "1.0.1-alpha-2",
                CurrentRelease = "1.0.1",
                RecommendedRelease = "2.0.0",
                NextRelease = "2.0.0",
                BreakingChange = true
            },
            // BUGFIX: BETA -> FINAL (SET NEXT VERSION)
            new ReleaseData
            {
                Type = "bugfix",
                BuildQuality = "",
                CurrentVersion = "0.0.1-beta-1",
                CurrentRelease = "0.0.1",
                RecommendedRelease = "1.0.0",
                NextRelease = "1.0.0",
                BreakingChange = false
            },
            new ReleaseData
            {
                Type = "bugfix",
                BuildQuality = "",
                CurrentVersion = "1.0.1-beta-2",
                CurrentRelease = "1.0.1",
                RecommendedRelease = "2.0.0",
                NextRelease = "2.0.0",
                BreakingChange = true
            },
            // BUGFIX: HOTFIX -> FINAL (SET NEXT VERSION)
            new ReleaseData
            {
                Type = "bugfix",
                BuildQuality = "",
                CurrentVersion = "0.0.1-hotfix-1",
                CurrentRelease = "0.0.1",
                RecommendedRelease = "1.0.0",
                NextRelease = "1.0.0",
                BreakingChange = false
            },
            new ReleaseData
            {
                Type = "bugfix",
                BuildQuality = "",
                CurrentVersion = "1.0.1-hotfix-2",
                CurrentRelease = "1.0.1",
                RecommendedRelease = "2.0.0",
                NextRelease = "2.0.0",
                BreakingChange = true
            },
            // BUGFIX: SERVICING -> FINAL (SET NEXT VERSION)
            new ReleaseData
            {
                Type = "bugfix",
                BuildQuality = "",
                CurrentVersion = "0.0.1-servicing-1",
                CurrentRelease = "0.0.1",
                RecommendedRelease = "1.0.0",
                NextRelease = "1.0.0",
                BreakingChange = false
            },
            new ReleaseData
            {
                Type = "bugfix",
                BuildQuality = "",
                CurrentVersion = "1.0.1-servicing-2",
                CurrentRelease = "1.0.1",
                RecommendedRelease = "2.0.0",
                NextRelease = "2.0.0",
                BreakingChange = true
            },
            // BUGFIX: RC -> FINAL (SET NEXT VERSION)
            new ReleaseData
            {
                Type = "bugfix",
                BuildQuality = "",
                CurrentVersion = "0.0.1-rc-1",
                CurrentRelease = "0.0.1",
                RecommendedRelease = "1.0.0",
                NextRelease = "1.0.0",
                BreakingChange = false
            },
            new ReleaseData
            {
                Type = "bugfix",
                BuildQuality = "",
                CurrentVersion = "1.0.1-rc-2",
                CurrentRelease = "1.0.1",
                RecommendedRelease = "2.0.0",
                NextRelease = "2.0.0",
                BreakingChange = true
            },
            // BUGFIX: FINAL -> FINAL (SET NEXT VERSION)
            new ReleaseData
            {
                Type = "bugfix",
                BuildQuality = "",
                CurrentVersion = "1.0.0",
                CurrentRelease = "1.0.0",
                RecommendedRelease = "1.0.1",
                NextRelease = "1.0.1",
                BreakingChange = false
            },
            new ReleaseData
            {
                Type = "bugfix",
                BuildQuality = "",
                CurrentVersion = "1.0.0",
                CurrentRelease = "1.0.0",
                RecommendedRelease = "2.0.0",
                NextRelease = "2.0.0",
                BreakingChange = true
            },
            // FEAT: NULL -> FINAL (SET NEXT VERSION)
            new ReleaseData
            {
                Type = "feat",
                BuildQuality = "",
                CurrentVersion = null,
                CurrentRelease = "0.0.0",
                RecommendedRelease = "1.0.0",
                NextRelease = "1.0.0",
                BreakingChange = false
            },
            new ReleaseData
            {
                Type = "feat",
                BuildQuality = "",
                CurrentVersion = null,
                CurrentRelease = "0.0.0",
                RecommendedRelease = "1.0.0",
                NextRelease = "1.0.0",
                BreakingChange = true
            },
            // FEAT: ALPHA -> FINAL (SET NEXT VERSION)
            new ReleaseData
            {
                Type = "feat",
                BuildQuality = "",
                CurrentVersion = "0.0.1-alpha-1",
                CurrentRelease = "0.0.1",
                RecommendedRelease = "1.0.0",
                NextRelease = "1.0.0",
                BreakingChange = false
            },
            new ReleaseData
            {
                Type = "feat",
                BuildQuality = "",
                CurrentVersion = "1.0.1-alpha-2",
                CurrentRelease = "1.0.1",
                RecommendedRelease = "2.0.0",
                NextRelease = "2.0.0",
                BreakingChange = true
            },
            // FEAT: BETA -> FINAL (SET NEXT VERSION)
            new ReleaseData
            {
                Type = "feat",
                BuildQuality = "",
                CurrentVersion = "0.0.1-beta-1",
                CurrentRelease = "0.0.1",
                RecommendedRelease = "1.0.0",
                NextRelease = "1.0.0",
                BreakingChange = false
            },
            new ReleaseData
            {
                Type = "feat",
                BuildQuality = "",
                CurrentVersion = "1.0.1-beta-2",
                CurrentRelease = "1.0.1",
                RecommendedRelease = "2.0.0",
                NextRelease = "2.0.0",
                BreakingChange = true
            },
            // FEAT: HOTFIX -> FINAL (SET NEXT VERSION)
            new ReleaseData
            {
                Type = "feat",
                BuildQuality = "",
                CurrentVersion = "0.0.1-hotfix-1",
                CurrentRelease = "0.0.1",
                RecommendedRelease = "1.0.0",
                NextRelease = "1.0.0",
                BreakingChange = false
            },
            new ReleaseData
            {
                Type = "feat",
                BuildQuality = "",
                CurrentVersion = "1.0.1-hotfix-2",
                CurrentRelease = "1.0.1",
                RecommendedRelease = "2.0.0",
                NextRelease = "2.0.0",
                BreakingChange = true
            },
            // FEAT: SERVICING -> FINAL (SET NEXT VERSION)
            new ReleaseData
            {
                Type = "feat",
                BuildQuality = "",
                CurrentVersion = "0.0.1-servicing-1",
                CurrentRelease = "0.0.1",
                RecommendedRelease = "1.0.0",
                NextRelease = "1.0.0",
                BreakingChange = false
            },
            new ReleaseData
            {
                Type = "feat",
                BuildQuality = "",
                CurrentVersion = "1.0.1-servicing-2",
                CurrentRelease = "1.0.1",
                RecommendedRelease = "2.0.0",
                NextRelease = "2.0.0",
                BreakingChange = true
            },
            // FEAT: RC -> FINAL (SET NEXT VERSION)
            new ReleaseData
            {
                Type = "feat",
                BuildQuality = "",
                CurrentVersion = "0.0.1-rc-1",
                CurrentRelease = "0.0.1",
                RecommendedRelease = "1.0.0",
                NextRelease = "1.0.0",
                BreakingChange = false
            },
            new ReleaseData
            {
                Type = "feat",
                BuildQuality = "",
                CurrentVersion = "1.0.1-rc-2",
                CurrentRelease = "1.0.1",
                RecommendedRelease = "2.0.0",
                NextRelease = "2.0.0",
                BreakingChange = true
            },
            // FEAT: FINAL -> FINAL (SET NEXT VERSION)
            new ReleaseData
            {
                Type = "feat",
                BuildQuality = "",
                CurrentVersion = "1.0.0",
                CurrentRelease = "1.0.0",
                RecommendedRelease = "1.1.0",
                NextRelease = "1.1.0",
                BreakingChange = false
            },
            new ReleaseData
            {
                Type = "feat",
                BuildQuality = "",
                CurrentVersion = "1.0.0",
                CurrentRelease = "1.0.0",
                RecommendedRelease = "2.0.0",
                NextRelease = "2.0.0",
                BreakingChange = true
            }
        };

        public class ReleaseData
        {
            public string Type { get; set; }

            public string BuildQuality { get; set; }

            public string CurrentVersion { get; set; }

            public string CurrentRelease { get; set; }

            public string RecommendedRelease { get; set; }

            public string NextRelease { get; set; }

            public bool BreakingChange { get; set; }
        }
    }
}
