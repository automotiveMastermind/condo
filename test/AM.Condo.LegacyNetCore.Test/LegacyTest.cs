// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LegacyTest.cs" company="automotiveMastermind">
//   © automotiveMastermind. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AM.Condo.LegacyNetCore
{
    using Xunit;

    [Purpose(PurposeType.Unit)]
    [Priority(1)]
    public class LegacyTest
    {
        [Fact]
        public void TestExecutes()
        {
            var value = "test";

            Assert.NotNull(value);
        }
    }
}
