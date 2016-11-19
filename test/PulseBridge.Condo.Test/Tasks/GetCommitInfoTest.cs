namespace PulseBridge.Condo.Tasks
{
    using System;
    using System.IO;

    using Microsoft.Build.Framework;
    using Moq;
    using Xunit;

    using PulseBridge.Condo.IO;

    [Class(nameof(GetCommitInfo))]
    public class GetCommitInfoTest
    {
        private readonly IGitRepositoryFactory repository = new GitRepositoryFactory();

        [Fact]
        [Priority(2)]
        [Purpose(PurposeType.Unit)]
        public void Execute_WhenRepositoryRootNull_Succeeds()
        {
            // arrange
            var root = default(string);
            var engine = new Mock<IBuildEngine>();
            engine.Setup(mock => mock.LogWarningEvent(It.IsAny<BuildWarningEventArgs>())).Verifiable();

            var actual = new GetCommitInfo
            {
                RepositoryRoot = root,
                BuildEngine = engine.Object
            };

            // act
            var result = actual.Execute();

            // assert
            Assert.True(result);
            Assert.Equal(root, actual.RepositoryRoot);
            Assert.Equal(root, actual.RepositoryRoot);
            Assert.Null(actual.LatestTag);
            Assert.Null(actual.LatestTagCommit);
            Assert.Null(actual.Commits);

            engine.Verify(mock => mock.LogWarningEvent(It.IsAny<BuildWarningEventArgs>()), Times.Once);
        }

        [Fact]
        [Priority(2)]
        [Purpose(PurposeType.Unit)]
        public void Execute_WhenRepositoryRootDoesNotExist_Succeeds()
        {
            // arrange
            var root = default(string);
            var engine = new Mock<IBuildEngine>();
            engine.Setup(mock => mock.LogWarningEvent(It.IsAny<BuildWarningEventArgs>())).Verifiable();

            using (var temp = new TemporaryPath())
            {
                root = temp.FullPath;
            }

            var actual = new GetCommitInfo
            {
                RepositoryRoot = root,
                BuildEngine = engine.Object
            };

            // act
            var result = actual.Execute();

            // assert
            Assert.True(result);
            Assert.Equal(root, actual.RepositoryRoot);
            Assert.Equal(root, actual.RepositoryRoot);
            Assert.Null(actual.LatestTag);
            Assert.Null(actual.LatestTagCommit);
            Assert.Null(actual.Commits);

            engine.Verify(mock => mock.LogWarningEvent(It.IsAny<BuildWarningEventArgs>()), Times.Once);
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
                var engine = new Mock<IBuildEngine>();
                engine.Setup(mock => mock.LogWarningEvent(It.IsAny<BuildWarningEventArgs>())).Verifiable();


                var actual = new GetCommitInfo
                {
                    RepositoryRoot = root,
                    BuildEngine = engine.Object
                };

                // act
                var result = actual.Execute();

                // assert
                Assert.True(result);
                Assert.Equal(root, actual.RepositoryRoot);
                Assert.Equal(root, actual.RepositoryRoot);
                Assert.Null(actual.LatestTag);
                Assert.Null(actual.LatestTagCommit);
                Assert.Null(actual.Commits);

                engine.Verify(mock => mock.LogWarningEvent(It.IsAny<BuildWarningEventArgs>()), Times.Once);
            }
        }

        [Fact]
        [Priority(2)]
        [Purpose(PurposeType.Integration)]
        public void Execute_WhenRepositoryRootValid_Succeeds()
        {
            using (var repo = repository.Initialize())
            {
                // arrange
                var root = repo.RepositoryPath;
                var expected = new
                {
                    RepositoryRoot = root,
                    Commits = new[]
                    {
                        new
                        {
                            Message = $"chore(kansas): somewhere over the rainbow{Environment.NewLine}where skies are blue.",
                            Header = "chore(kansas): somewhere over the rainbow",
                            Body = "where skies are blue.",
                            Type = "chore",
                            Scope = "kansas",
                            Subject = "somewhere over the rainbow"
                        }
                    },
                    LatestTag = "latest"
                };

                foreach (var commit in expected.Commits)
                {
                    repo.Save(Path.GetRandomFileName())
                        .Add()
                        .Commit(commit.Type, commit.Scope, commit.Subject, commit.Body);
                }

                repo.Tag(expected.LatestTag);

                var engine = MSBuildMocks.CreateEngine();

                var instance = new GetCommitInfo
                {
                    RepositoryRoot = root,
                    BuildEngine = engine
                };

                // act
                var result = instance.Execute();

                // assert
                Assert.True(result);
                Assert.Equal(expected.RepositoryRoot, instance.RepositoryRoot);
                Assert.Equal(expected.LatestTag, instance.LatestTag);
                Assert.Equal(expected.Commits.Length, instance.Commits.Length);

                Assert.Equal(instance.Commits[0].GetMetadata("Hash"), instance.LatestTagCommit);

                for (var i = 0; i < expected.Commits.Length; i++)
                {
                    var current = expected.Commits[i];
                    var commit = instance.Commits[instance.Commits.Length - i - 1];

                    var actual = new
                    {
                        Header = commit.GetMetadata(nameof(current.Header)),
                        Message = commit.GetMetadata(nameof(current.Message)),
                        Body = commit.GetMetadata(nameof(current.Body)),

                        Type = commit.GetMetadata(nameof(current.Type)),
                        Scope = commit.GetMetadata(nameof(current.Scope)),
                        Subject = commit.GetMetadata(nameof(current.Subject))
                    };

                    Assert.Equal(current.Header, actual.Header);
                    Assert.Equal(current.Message, actual.Message);
                    Assert.Equal(current.Body, actual.Body);

                    Assert.Equal(current.Type, actual.Type);
                    Assert.Equal(current.Scope, actual.Scope);
                    Assert.Equal(current.Subject, actual.Subject);
                }
            }
        }
    }
}