namespace PulseBridge.Condo.IO
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents git log options that uses the AngularJS conventions for commits.
    /// </summary>
    public class AngularGitLogOptions : IGitLogOptions
    {
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
        public IList<string> RevertCorrespondence { get; } = new List<string> { "header", "hash" };

        /// <inheritdoc />
        public IList<string> ReferencePrefixes { get; } = new List<string> { "#" };

        /// <inheritdoc />
        public IList<string> MentionPrefixes { get; } = new List<string> { "@" };

        /// <inheritdoc />
        public IList<string> ActionKeywords { get; }
            // = new List<string> { "close", "closes", "closed", "fix", "fixes", "fixed", "resolve", "resolves", "resolved" };
            = new List<string> { "closes" };

        /// <inheritdoc />
        public IList<string> NoteKeywords { get; } = new List<string> { "BREAKING CHANGE", "BREAKING CHANGES" };

        /// <inheritdoc />
        public bool IncludeInvalidCommits { get; set; } = false;
    }
}