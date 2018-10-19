// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommitComparerTest.cs" company="automotiveMastermind and contributors">
//   © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AM.Condo.ChangeLog
{
    using System;
    using System.Collections.Generic;
    using Xunit;

    [Class(nameof(CommitComparer))]
    [Purpose(PurposeType.Unit)]
    public class CommitComparerTest
    {
        [Fact]
        public void GivenCompare_WhenSingleSortByColumn_SortsAsExpected()
        {
            // arrange
            var key = "sortBy";

            var sortBy = new[] { "SortBy" };
            var comparer = new CommitComparer(sortBy);

            var expected = new[]
            {
                new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase)
                {
                    { key, "alpha" },
                    { "title", "bravo" }
                },
                new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase)
                {
                    { "title", "alpha" },
                    { key, "bravo" }
                },
                new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase)
                {
                    { key, "bravo" },
                    { "title", "delta" }
                },
                new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase)
                {
                    { "title", "charlie" },
                    { key, "charlie" }
                }
            };

            var sorted = new SortedSet<Dictionary<string, object>>(comparer)
            {
                expected[2],
                expected[3],
                expected[0],
                expected[1]
            };

            // assert
            var i = 0;

            foreach (var actual in sorted)
            {
                Assert.Same(expected[i++], actual);
            }
        }
    }
}
