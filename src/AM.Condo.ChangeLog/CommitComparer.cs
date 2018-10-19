// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommitComparer.cs" company="automotiveMastermind and contributors">
//   © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AM.Condo.ChangeLog
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents a comparer that maintains the sort order of commits based on the sort options.
    /// </summary>
    public class CommitComparer : IComparer<IDictionary<string, object>>
    {
        #region Fields
        private readonly string[] sortBy;
        #endregion

        #region Properties and Indexers
        /// <summary>
        /// Initializes a new instance of the <see cref="CommitComparer"/> class.
        /// </summary>
        /// <param name="sortBy">
        /// The array of keys used for sorting.
        /// </param>
        public CommitComparer(params string[] sortBy)
            => this.sortBy = sortBy;

        /// <inheritdoc />
        public int Compare(IDictionary<string, object> left, IDictionary<string, object> right)
        {
            // iterate over the sort by columns
            foreach (var sort in this.sortBy)
            {
                // determine if the left and right value has the key
                var leftHasValue = left.TryGetValue(sort, out var leftValue)
                    && !string.IsNullOrEmpty(leftValue as string);

                var rightHasValue = right.TryGetValue(sort, out var rightValue)
                    && !string.IsNullOrEmpty(rightValue as string);

                // determine if they do not both have the value
                if (leftHasValue != rightHasValue)
                {
                    // determine if the left has a value and assumer earlier sort order; otherwise assume later sort order
                    // tricky: values that have sort columns should be sorted ahead of those that do not
                    return leftHasValue ? -1 : 1;
                }

                // compare the string values
                var compare = string.Compare(leftValue as string, rightValue as string, ignoreCase: true);

                // if the strings aren't equal
                if (compare != 0)
                {
                    // return the string comparison
                    return compare;
                }
            }

            // assume earlier sort order
            return -1;
        }
        #endregion
    }
}
