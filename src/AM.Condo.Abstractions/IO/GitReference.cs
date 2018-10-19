// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GitReference.cs" company="automotiveMastermind and contributors">
// © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AM.Condo.IO
{
    using System;

    /// <summary>
    /// Represents a reference to a work item, issue, pull request, or other artifact within a git commit.
    /// </summary>
    public class GitReference : IComparable<GitReference>, IEquatable<GitReference>
    {
        #region Properties and Indexers
        /// <summary>
        /// Gets or sets the ID of the reference.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the action of the reference.
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// Gets or sets the owner of the reference.
        /// </summary>
        public string Owner { get; set; }

        /// <summary>
        /// Gets or sets the repository of the reference.
        /// </summary>
        public string Repository { get; set; }

        /// <summary>
        /// Gets or sets the raw reference.
        /// </summary>
        public string Raw { get; set; }

        /// <summary>
        /// Gets or sets the prefix of the reference.
        /// </summary>
        public string Prefix { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Determines if the specified <paramref name="left"/> and <paramref name="right"/> git references are equal.
        /// </summary>
        /// <param name="left">
        /// The left git reference to evaluate.
        /// </param>
        /// <param name="right">
        /// The right git reference to evaluate.
        /// </param>
        /// <returns>
        /// A value indicating whether or not the specified <paramref name="left"/> and <paramref name="right"/> git
        /// references are equal.
        /// </returns>
        public static bool operator ==(GitReference left, GitReference right) => Equals(left, right);

        /// <summary>
        /// Determines if the specified <paramref name="left"/> and <paramref name="right"/> git references are not
        /// equal.
        /// </summary>
        /// <param name="left">
        /// The left git reference to evaluate.
        /// </param>
        /// <param name="right">
        /// The right git reference to evaluate.
        /// </param>
        /// <returns>
        /// A value indicating whether or not the specified <paramref name="left"/> and <paramref name="right"/> git
        /// references are not equal.
        /// </returns>
        public static bool operator !=(GitReference left, GitReference right) => !Equals(left, right);

        /// <summary>
        /// Determines if the specified <paramref name="left"/> and <paramref name="right"/> git references are equal.
        /// </summary>
        /// <param name="left">
        /// The left git reference to evaluate.
        /// </param>
        /// <param name="right">
        /// The right git reference to evaluate.
        /// </param>
        /// <returns>
        /// A value indicating whether or not the specified <paramref name="left"/> and <paramref name="right"/> git
        /// references are equal.
        /// </returns>
        public static bool Equals(GitReference left, GitReference right)
        {
            if (object.ReferenceEquals(left, right))
            {
                return true;
            }

            if (object.ReferenceEquals(left, null))
            {
                return false;
            }

            return object.ReferenceEquals(left, null) ? false : left.Equals(right);
        }

        /// <inheritdoc />
        public int CompareTo(GitReference other) => string.Compare(this.Id, other?.Id, ignoreCase: true);

        /// <inheritdoc />
        public bool Equals(GitReference other)
            => string.Equals(this.Id, other?.Id, StringComparison.InvariantCultureIgnoreCase);

        /// <inheritdoc />
        public override bool Equals(object obj) => this.Equals(obj as GitReference);

        /// <inheritdoc />
        public override int GetHashCode() => this.Id?.GetHashCode() ?? base.GetHashCode();

        /// <inheritdoc />
        public override string ToString() => this.Raw ?? "<unknown>";
        #endregion
    }
}
