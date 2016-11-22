namespace PulseBridge.Condo.IO
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents git log options that uses the AngularJS conventions for commits.
    /// </summary>
    public class AngularGitLogOptions : IGitLogOptions
    {
        /// <inheritdoc />
        public string Split { get; set; } = "------------------------ >8< ------------------------";

        /// <inheritdoc />
        public string Format { get; set; } = "%H%n%h%n%s%n%b------------------------ >8< ------------------------";

        /// <inheritdoc />
        public string HeaderPattern { get; set; } = @"^(\w*)(?:\(([\w\$\.\-\* ]*)\))?\: (.*)$";

        /// <inheritdoc />
        public string MergePattern { get; set; }

        /// <inheritdoc />
        public string RevertPattern { get; set; } = @"^Revert\s""([\s\S]*)""\s*This reverts commit (\w*)\.";

        /// <inheritdoc />
        public string FieldPattern { get; set; } = @"^-(.*?)-$";

        /// <inheritdoc />
        public IList<string> HeaderCorrespondence { get; } = new List<string> { "type", "scope", "subject" };

        /// <inheritdoc />
        public IList<string> MergeCorrespondence { get; } = new List<string>();

        /// <inheritdoc />
        public IList<string> RevertCorrespondence { get; } = new List<string>();

        /// <inheritdoc />
        public IList<string> ReferencePrefixes { get; } = new List<string> { "#" };

        /// <inheritdoc />
        public IList<string> MentionPrefixes { get; } = new List<string> { "@" };

        /// <inheritdoc />
        public IList<string> ActionKeywords { get; }
            = new List<string> { "close", "closes", "closed", "fix", "fixed", "resolve", "resolves", "resolved" };

        /// <inheritdoc />
        public IList<string> NoteKeywords { get; } = new List<string> { "BREAKING CHANGE" };
    }
}