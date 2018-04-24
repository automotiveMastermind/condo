// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetCommitInfoTest.cs" company="automotiveMastermind and contributors">
//   © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AM.Condo.Tasks
{
    using System;
    using System.IO;

    using Microsoft.Build.Framework;
    using Moq;
    using Xunit;

    using AM.Condo.IO;

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
            Assert.Null(actual.Commits);
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
                Assert.Null(actual.Commits);
            }
        }

        [Fact]
        [Priority(2)]
        [Purpose(PurposeType.Integration)]
        public void Execute_WhenRepositoryRootValidWithNonVersionTag_Succeeds()
        {
            using (var repo = repository.Initialize())
            {
                // set the username and email
                repo.Username = "condo";
                repo.Email = "open@automotivemastermind.com";

                // arrange
                var root = repo.RepositoryPath;
                var expected = new
                {
                    RepositoryRoot = root,
                    Commits = new[]
                    {
                        new
                        {
                            Raw = $"docs(kansas): somewhere over the rainbow{Environment.NewLine}where skies are blue.{Environment.NewLine}{Environment.NewLine}BREAKING CHANGE: something bad happened{Environment.NewLine}{Environment.NewLine}Closes: #34, #22",
                            Header = "docs(kansas): somewhere over the rainbow",
                            Body = "where skies are blue.",
                            Type = "docs",
                            Scope = "kansas",
                            Subject = "somewhere over the rainbow",
                            Footer = $"BREAKING CHANGE: something bad happened{Environment.NewLine}{Environment.NewLine}Closes: #34, #22",
                            Notes = "1",
                            Close = "34;22",
                            References = "34;22"
                        }
                    },
                    LatestVersion = "0.0.0"
                };

                foreach (var commit in expected.Commits)
                {
                    repo.Save(Path.GetRandomFileName())
                        .Add()
                        .Commit(commit.Raw);
                }

                repo.Tag(expected.LatestVersion, message: null);

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
                Assert.Equal(expected.Commits.Length, instance.Commits.Length);

                for (var i = 0; i < expected.Commits.Length; i++)
                {
                    var current = expected.Commits[i];
                    var commit = instance.Commits[instance.Commits.Length - i - 1];

                    var actual = new
                    {
                        Header = commit.GetMetadata(nameof(current.Header)),
                        Raw = commit.GetMetadata(nameof(current.Raw)),
                        Body = commit.GetMetadata(nameof(current.Body)),

                        Type = commit.GetMetadata(nameof(current.Type)),
                        Scope = commit.GetMetadata(nameof(current.Scope)),
                        Subject = commit.GetMetadata(nameof(current.Subject)),

                        Notes = commit.GetMetadata(nameof(current.Notes)),

                        References = commit.GetMetadata(nameof(current.References)),
                        Close = commit.GetMetadata(nameof(current.Close))
                    };

                    Assert.Equal(current.Header, actual.Header);
                    Assert.Equal(current.Raw, actual.Raw);
                    Assert.Equal(current.Body, actual.Body);

                    Assert.Equal(current.Type, actual.Type);
                    Assert.Equal(current.Scope, actual.Scope);
                    Assert.Equal(current.Subject, actual.Subject);

                    Assert.Equal(current.Notes, actual.Notes);

                    Assert.Equal(current.References, actual.References);
                    Assert.Equal(current.Close, actual.Close);
                }
            }
        }
    }
}
