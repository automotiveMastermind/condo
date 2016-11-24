namespace PulseBridge.Condo.Tasks
{
    using Xunit;

    using PulseBridge.Condo.IO;

    [Class(nameof(GetRepositoryInfo))]
    public class GetRepositoryInfoTest
    {
        private readonly IGitRepositoryFactory repository = new GitRepositoryFactory();

        [Fact]
        [Priority(2)]
        [Purpose(PurposeType.Unit)]
        public void Execute_WhenRepositoryRootNull_Succeeds()
        {
            // arrange
            var root = default(string);
            var engine = MSBuildMocks.CreateEngine();

            var actual = new GetRepositoryInfo
            {
                RepositoryRoot = root,
                BuildEngine = engine
            };

            // act
            var result = actual.Execute();

            // assert
            Assert.True(result);
            Assert.Equal(root, actual.RepositoryRoot);
            Assert.Equal(root, actual.RepositoryRoot);
            Assert.Null(actual.RepositoryUri);
            Assert.Null(actual.Branch);
            Assert.Null(actual.CommitId);
        }

        [Fact]
        [Priority(2)]
        [Purpose(PurposeType.Unit)]
        public void Execute_WhenRepositoryRootDoesNotExist_Succeeds()
        {
            // arrange
            var root = default(string);
            var engine = MSBuildMocks.CreateEngine();

            using (var temp = new TemporaryPath())
            {
                root = temp.FullPath;
            }

            var actual = new GetRepositoryInfo
            {
                RepositoryRoot = root,
                BuildEngine = engine
            };

            // act
            var result = actual.Execute();

            // assert
            Assert.True(result);
            Assert.Equal(root, actual.RepositoryRoot);
            Assert.Equal(root, actual.RepositoryRoot);
            Assert.Null(actual.RepositoryUri);
            Assert.Null(actual.Branch);
            Assert.Null(actual.CommitId);
        }

        [Fact]
        [Priority(2)]
        [Purpose(PurposeType.Unit)]
        public void Execute_WhenRepositoryRootNotRepository_Succeeds()
        {
            using (var temp = new TemporaryPath())
            {
                // arrange
                var root = temp.FullPath;
                var engine = MSBuildMocks.CreateEngine();

                var actual = new GetRepositoryInfo
                {
                    RepositoryRoot = root,
                    BuildEngine = engine
                };

                // act
                var result = actual.Execute();

                // assert
                Assert.True(result);
                Assert.Equal(root, actual.RepositoryRoot);
                Assert.Null(actual.RepositoryUri);
                Assert.Null(actual.Branch);
                Assert.Null(actual.CommitId);
            }
        }

        [Fact]
        [Priority(2)]
        [Purpose(PurposeType.Integration)]
        public void Execute_WhenRepositoryRootValid_Succeeds()
        {
            using (var repo = repository.Initialize().Commit("initial"))
            {
                // arrange
                var root = repo.RepositoryPath;
                var engine = MSBuildMocks.CreateEngine();

                var expected = new
                {
                    RepositoryRoot = root,
                    Branch = "master"
                };

                var actual = new GetRepositoryInfo
                {
                    RepositoryRoot = root,
                    BuildEngine = engine
                };

                // act
                var result = actual.Execute();

                // assert
                Assert.True(result);
                Assert.Equal(expected.RepositoryRoot, actual.RepositoryRoot);
                Assert.Equal(expected.Branch, actual.Branch);
                Assert.NotNull(actual.CommitId);
            }
        }

        [Fact]
        [Priority(2)]
        [Purpose(PurposeType.Integration)]
        public void Execute_WhenBranchRefSet_UsesAbbreviatedBranch()
        {
            using (var repo = repository.Initialize().Commit("initial"))
            {
                // arrange
                var root = repo.RepositoryPath;
                var engine = MSBuildMocks.CreateEngine();
                var branch = "refs/heads/master";

                var expected = new
                {
                    RepositoryRoot = root,
                    Branch = "master"
                };

                var actual = new GetRepositoryInfo
                {
                    RepositoryRoot = root,
                    BuildEngine = engine,
                    Branch = branch
                };

                // act
                var result = actual.Execute();

                // assert
                Assert.True(result);
                Assert.Equal(expected.RepositoryRoot, actual.RepositoryRoot);
                Assert.Equal(expected.Branch, actual.Branch);
                Assert.NotNull(actual.CommitId);
            }
        }

        [Fact]
        [Priority(2)]
        [Purpose(PurposeType.Unit)]
        public void TryCommandLine_WhenRepositoryRootNull_Fails()
        {
            // arrange
            var root = default(string);
            var engine = MSBuildMocks.CreateEngine();

            var actual = new GetRepositoryInfo
            {
                RepositoryRoot = root,
                BuildEngine = engine
            };

            // act
            var result = actual.TryCommandLine(null);

            // assert
            Assert.False(result);
            Assert.Equal(root, actual.RepositoryRoot);
            Assert.Null(actual.RepositoryUri);
            Assert.Null(actual.Branch);
            Assert.Null(actual.CommitId);
        }

        [Fact]
        [Priority(2)]
        [Purpose(PurposeType.Unit)]
        public void TryCommandLine_WhenRepositoryRootDoesNotExist_Fails()
        {
            // arrange
            var root = default(string);
            var engine = MSBuildMocks.CreateEngine();

           using (var temp = new TemporaryPath())
            {
                root = temp.FullPath;
            }

            var actual = new GetRepositoryInfo
            {
                RepositoryRoot = root,
                BuildEngine = engine
            };

            // act
            var result = actual.TryCommandLine(null);

            // assert
            Assert.False(result);
            Assert.Null(actual.RepositoryUri);
            Assert.Null(actual.Branch);
            Assert.Null(actual.CommitId);
    }

        [Fact]
        [Priority(2)]
        [Purpose(PurposeType.Unit)]
        public void TryCommandLine_WhenRepositoryRootNotRepository_Fails()
        {
            using (var temp = new TemporaryPath())
            {
                // arrange
                var root = temp.FullPath;
                var engine = MSBuildMocks.CreateEngine();

                var actual = new GetRepositoryInfo
                {
                    RepositoryRoot = root,
                    BuildEngine = engine
                };

                // act
                var result = actual.TryCommandLine(null);

                // assert
                Assert.True(result);
                Assert.Equal(root, actual.RepositoryRoot);
            }
        }

        [Fact]
        [Priority(2)]
        [Purpose(PurposeType.Unit)]
        public void TryCommandLine_WhenRepositoryRootValid_Succeeds()
        {
            using (var repo = repository.Initialize().Commit("initial"))
            {
                // arrange
                var root = repo.RepositoryPath;
                var engine = MSBuildMocks.CreateEngine();

                var expected = new
                {
                    RepositoryRoot = root,
                    Branch = "master"
                };

                var actual = new GetRepositoryInfo
                {
                    RepositoryRoot = root,
                    BuildEngine = engine
                };

                // act
                var result = actual.TryCommandLine(null);

                // assert
                Assert.True(result);
                Assert.Equal(expected.RepositoryRoot, actual.RepositoryRoot);
                Assert.Equal(expected.Branch, actual.Branch);
                Assert.NotNull(actual.CommitId);
            }
        }
    }
}