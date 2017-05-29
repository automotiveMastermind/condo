// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SetGitTagTest.cs" company="automotiveMastermind and contributors">
//   © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AM.Condo.Tasks
{
    using Xunit;

    using AM.Condo.IO;

    [Class(nameof(SetGitTag))]
    public class SetGitTagTest
    {
        private readonly IGitRepositoryFactory repository = new GitRepositoryFactory();

        [Fact]
        [Purpose(PurposeType.Unit)]
        public void Execute_WhenRepositoryNull_Fails()
        {
            // arrange
            var root = default(string);
            var tag = "tag-1-2-3";

            var engine = MSBuildMocks.CreateEngine();

            var actual = new SetGitTag
            {
                RepositoryRoot = root,
                Tag = tag,
                BuildEngine = engine
            };

            // act
            var result = actual.Execute();

            // assert
            Assert.False(result);
        }

        [Fact]
        [Purpose(PurposeType.Unit)]
        public void Execute_WhenRepositoryEmpty_Fails()
        {
            // arrange
            var root = string.Empty;
            var tag = "tag-1-2-3";

            var engine = MSBuildMocks.CreateEngine();

            var actual = new SetGitTag
            {
                RepositoryRoot = root,
                Tag = tag,
                BuildEngine = engine
            };

            // act
            var result = actual.Execute();

            // assert
            Assert.False(result);
        }

        [Fact]
        [Purpose(PurposeType.Unit)]
        public void Execute_WhenRepositoryDoesNotExist_Fails()
        {
            // arrange
            var root = default(string);
            var tag = "tag-1-2-3";

            var engine = MSBuildMocks.CreateEngine();

            var actual = new SetGitTag
            {
                RepositoryRoot = root,
                Tag = tag,
                BuildEngine = engine
            };

            using (var temp = new TemporaryPath())
            {
                root = temp.FullPath;
            }

            // act
            var result = actual.Execute();

            // assert
            Assert.False(result);
        }

        [Fact]
        [Purpose(PurposeType.Unit)]
        public void Execute_WhenRepositoryIsInvalid_SucceedsWithWarning()
        {
            using (var temp = new TemporaryPath())
            {
                // arrange
                var root = temp.FullPath;
                var tag = "tag-1-2-3";

                var engine = MSBuildMocks.CreateEngine();

                var actual = new SetGitTag
                {
                    RepositoryRoot = root,
                    Tag = tag,
                    BuildEngine = engine
                };

                // act
                var result = actual.Execute();

                // assert
                Assert.False(result);
            }
        }

        [Fact]
        [Purpose(PurposeType.Unit)]
        public void Execute_WhenRepositoryHasNoCommits_Fails()
        {
            using (var repo = repository.Initialize())
            {
                // arrange
                var root = repo.RepositoryPath;
                var tag = "tag-1-2-3";

                var engine = MSBuildMocks.CreateEngine();

                var actual = new SetGitTag
                {
                    RepositoryRoot = root,
                    Tag = tag,
                    BuildEngine = engine
                };

                // act
                var result = actual.Execute();

                // assert
                Assert.False(result);
            }
        }

        [Fact]
        [Purpose(PurposeType.Unit)]
        public void Execute_WhenRepositoryValid_CreatesTag()
        {
            using (var bare = repository.Bare())
            {
                using (var repo = repository.Clone(bare.RepositoryPath))
                {
                    // set the username and email
                    repo.Username = "condo";
                    repo.Email = "open@automotivemastermind.com";

                    // commit
                    repo.Commit("initial");

                    // arrange
                    var root = repo.RepositoryPath;
                    var tag = "tag-1-2-3";

                    var engine = MSBuildMocks.CreateEngine();

                    var actual = new SetGitTag
                    {
                        RepositoryRoot = root,
                        Tag = tag,
                        BuildEngine = engine
                    };

                    // act
                    var result = actual.Execute();

                    // assert
                    Assert.True(result);
                }
            }
        }
    }
}
