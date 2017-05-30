// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GitRepositoryTest.cs" company="automotiveMastermind and contributors">
//   © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AM.Condo.IO
{
    using System;

    using Xunit;

    [Class(nameof(GitRepository))]
    [Priority(2)]
    [Purpose(PurposeType.Unit)]
    public class GitRepositoryTest
    {
        [Fact]
        public void Ctor_WhenPathIsNotRepo_DoesNotSetCurrentBranch()
        {
            using (var temp = new TemporaryPath())
            {
                // arrange
                var path = temp.FullPath;

                // act
                var actual = new GitRepository(temp);

                // assert
                Assert.NotNull(actual.ClientVersion);
                Assert.Null(actual.CurrentBranch);
                Assert.Equal(path, actual.RepositoryPath);
            }
        }

        [Fact]
        public void Authorization_WhenSet_ReturnsAuthorizationValue()
        {
            using (var temp = new TemporaryPath())
            {
                // arrange
                var path = temp.FullPath;
                var target = new GitRepository(temp);
                target.Initialize();

                var expected = Guid.NewGuid().ToString();

                // act
                target.Authorization = expected;
                var actual = target.Authorization;

                // assert
                Assert.Equal(expected, actual);
            }
        }
    }
}
