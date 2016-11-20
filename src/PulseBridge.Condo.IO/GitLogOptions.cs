namespace PulseBridge.Condo.IO
{
    using System.Collections.Generic;

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
        public ICollection<string> HeaderFields { get; } = new HashSet<string> { "type", "scope", "subject" };

        /// <inheritdoc />
        public ICollection<string> MergeFields { get; }

        /// <inheritdoc />
        public ICollection<string> ReferencePrefixes { get; } = new HashSet<string> { "#" };

        /// <inheritdoc />
        public ICollection<string> ActionKeywords { get; }
            = new HashSet<string> { "close", "closes", "closed", "fix", "fixed", "resolve", "resolves", "resolved" };

        /// <inheritdoc />
        public ICollection<string> NoteKeywords { get; } = new HashSet<string> { "BREAKING CHANGE" };
    }
}