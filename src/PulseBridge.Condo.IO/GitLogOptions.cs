namespace PulseBridge.Condo.IO
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents git log options that uses the AngularJS conventions for commits.
    /// </summary>
    public class GitLogOptions : IGitLogOptions
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
        public List<string> HeaderCorrespondence { get; set; } = new List<string> { "Type", "Scope", "Subject" };

        /// <inheritdoc />
        public List<string> MergeCorrespondence { get; set; } = new List<string>();

        /// <inheritdoc />
        public List<string> RevertCorrespondence { get; set; } = new List<string> { "Header", "Hash" };

        /// <inheritdoc />
        public List<string> ReferencePrefixes { get; set; } = new List<string> { "#" };

        /// <inheritdoc />
        public List<string> MentionPrefixes { get; set; } = new List<string> { "@" };

        /// <inheritdoc />
        public List<string> ActionKeywords { get; set; }
            = new List<string> { "Close", "Closes", "Closed", "Fix", "Fixes", "Fixed", "Resolve", "Resolves", "Resolved" };

        /// <inheritdoc />
        public List<string> NoteKeywords { get; set; } = new List<string> { "BREAKING CHANGE", "BREAKING CHANGES" };

        /// <inheritdoc />
        public bool IncludeInvalidCommits { get; set; } = false;
    }
}
