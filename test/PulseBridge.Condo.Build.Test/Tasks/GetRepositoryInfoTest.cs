namespace PulseBridge.Condo.Build.Tasks
{
    using System.IO;

    using Microsoft.Build.Framework;

    using Moq;

    using Xunit;

    [Class(nameof(GetRepositoryInfo))]
    public class GetRepositoryInfoTest
    {
        [Fact]
        [Priority(2)]
        public void Execute_WhenRepositoryRootNull_Succeeds()
        {
            // arrange
            var root = default(string);
            var engine = Mock.Of<IBuildEngine>();

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
        public void Execute_WhenRepositoryRootDoesNotExist_Succeeds()
        {
            // todo: create and then delete a temporary path instead of hard-coding a path

            // arrange
            var root = "/nowhere";
            var engine = Mock.Of<IBuildEngine>();

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
        public void Execute_WhenRepositoryRootNotRepository_Succeeds()
        {
            // arrange
            var root = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var engine = Mock.Of<IBuildEngine>();

            Directory.CreateDirectory(root);

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

        [Fact]
        [Priority(2)]
        public void Execute_WhenRepositoryRootValid_Succeeds()
        {
            // todo: do not use own repo -- create a new one instead

            // arrange
            var root = Directory.GetCurrentDirectory();
            var engine = Mock.Of<IBuildEngine>();

            while (!File.Exists(Path.Combine(root, "condo.sh")))
            {
                root = Directory.GetParent(root).FullName;
            }

            var expected = new
            {
                RepositoryRoot = root
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
            Assert.NotNull(actual.RepositoryUri);
            Assert.NotNull(actual.CommitId);

            Assert.NotEqual("<unknown>", actual.Branch);
        }

        [Fact]
        [Priority(2)]
        public void TryCommandLine_WhenRepositoryRootNull_Succeeds()
        {
            // arrange
            var root = default(string);
            var engine = Mock.Of<IBuildEngine>();

            var actual = new GetRepositoryInfo
            {
                RepositoryRoot = root,
                BuildEngine = engine
            };

            // act
            var result = actual.TryCommandLine();

            // assert
            Assert.True(result);
            Assert.Equal(root, actual.RepositoryRoot);
            Assert.NotNull(actual.RepositoryUri);
            Assert.NotNull(actual.Branch);
            Assert.NotNull(actual.CommitId);
        }

        [Fact]
        [Priority(2)]
        public void TryCommandLine_WhenRepositoryRootDoesNotExist_Fails()
        {
            // todo: create and then delete a temporary path instead of hard-coding a path

            // arrange
            var root = "/nowhere";
            var engine = Mock.Of<IBuildEngine>();

            var actual = new GetRepositoryInfo
            {
                RepositoryRoot = root,
                BuildEngine = engine
            };

            // act
            var result = actual.TryCommandLine();

            // assert
            Assert.False(result);
            Assert.Null(actual.RepositoryUri);
            Assert.Null(actual.Branch);
            Assert.Null(actual.CommitId);
        }

        [Fact]
        [Priority(2)]
        public void TryCommandLine_WhenRepositoryRootNotRepository_Fails()
        {
            // arrange
            var root = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var engine = Mock.Of<IBuildEngine>();

            Directory.CreateDirectory(root);

            var actual = new GetRepositoryInfo
            {
                RepositoryRoot = root,
                BuildEngine = engine
            };

            // act
            var result = actual.TryCommandLine();

            // assert
            Assert.True(result);
            Assert.Equal(root, actual.RepositoryRoot);
        }

        [Fact]
        [Priority(2)]
        public void TryCommandLine_WhenRepositoryRootValid_Succeeds()
        {
            // todo: do not use own repo -- create a new one instead

            // arrange
            var root = Directory.GetCurrentDirectory();
            var engine = Mock.Of<IBuildEngine>();

            while (!File.Exists(Path.Combine(root, "condo.sh")))
            {
                root = Directory.GetParent(root).FullName;
            }

            var expected = new
            {
                RepositoryRoot = root
            };

            var actual = new GetRepositoryInfo
            {
                RepositoryRoot = root,
                BuildEngine = engine
            };

            // act
            var result = actual.TryCommandLine();

            // assert
            Assert.True(result);
            Assert.Equal(expected.RepositoryRoot, actual.RepositoryRoot);
            Assert.NotEqual("<unknown>", actual.Branch);
            Assert.NotNull(actual.RepositoryUri);
            Assert.NotNull(actual.CommitId);
        }

        [Fact]
        [Priority(2)]
        public void TryFileSystem_WhenRepositoryRootNull_Fails()
        {
            // arrange
            var root = default(string);
            var engine = Mock.Of<IBuildEngine>();

            var actual = new GetRepositoryInfo
            {
                RepositoryRoot = root,
                BuildEngine = engine
            };

            // act
            var result = actual.TryFileSystem();

            // assert
            Assert.False(result);
            Assert.Null(actual.RepositoryUri);
            Assert.Null(actual.Branch);
            Assert.Null(actual.CommitId);
        }

        [Fact]
        [Priority(2)]
        public void TryFileSystem_WhenRepositoryRootDoesNotExist_Succeeds()
        {
            // todo: create and then delete a temporary path instead of hard-coding a path

            // arrange
            var root = "/nowhere";
            var engine = Mock.Of<IBuildEngine>();

            var actual = new GetRepositoryInfo
            {
                RepositoryRoot = root,
                BuildEngine = engine
            };

            // act
            var result = actual.TryFileSystem();

            // assert
            Assert.True(result);
            Assert.Null(actual.RepositoryUri);
            Assert.Null(actual.Branch);
            Assert.Null(actual.CommitId);
        }

        [Fact]
        [Priority(2)]
        public void TryFileSystem_WhenRepositoryRootNotRepository_Fails()
        {
            // arrange
            var root = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var engine = Mock.Of<IBuildEngine>();

            Directory.CreateDirectory(root);

            var actual = new GetRepositoryInfo
            {
                RepositoryRoot = root,
                BuildEngine = engine
            };

            // act
            var result = actual.TryFileSystem();

            // assert
            Assert.True(result);
            Assert.Equal(root, actual.RepositoryRoot);
        }

        [Fact]
        [Priority(2)]
        public void TryFileSystem_WhenRepositoryRootValid_Succeeds()
        {
            // todo: do not use own repo -- create a new one instead

            // arrange
            var root = Directory.GetCurrentDirectory();
            var engine = Mock.Of<IBuildEngine>();

            while (!File.Exists(Path.Combine(root, "condo.sh")))
            {
                root = Directory.GetParent(root).FullName;
            }

            var expected = new
            {
                RepositoryRoot = root
            };

            var actual = new GetRepositoryInfo
            {
                RepositoryRoot = root,
                BuildEngine = engine
            };

            // act
            var result = actual.TryFileSystem();

            // assert
            Assert.True(result);
            Assert.Equal(expected.RepositoryRoot, actual.RepositoryRoot);
            Assert.NotEqual("<unknown>", actual.Branch);
            Assert.NotNull(actual.RepositoryUri);
            Assert.NotNull(actual.CommitId);
        }
    }
}