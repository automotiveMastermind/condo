// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetBuildQualityTest.cs" company="automotiveMastermind and contributors">
//   © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Globalization;
using Xunit;

namespace AM.Condo.Tasks
{
    [Class(nameof(GetAssemblyInfo))]
    public class GetBuildQualityTest
    {
        [Theory]
        [InlineData("master", null)]
        [InlineData("main", null)]
        [InlineData("some feature", "alpha")]
        [Priority(2)]
        [Purpose(PurposeType.Unit)]
        public void Execute_WhenBranchName_ThenProperBuildQuality(string branchName, string expectedBuildQuality)
        {
            // arrange
            var ci = true;

            var expected = new
            {
                BuildQuality = expectedBuildQuality,
                CI = ci,
            };

            var actual = new GetBuildQuality
            {
                BuildQuality = expected.BuildQuality,
                CI = ci,
                Branch = branchName
            };

            // act
            var result = actual.Execute();

            // assert
            Assert.True(result);
            Assert.Equal(expected.BuildQuality, actual.BuildQuality);
        }
    }
}
