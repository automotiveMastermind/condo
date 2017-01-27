namespace PulseBridge.Condo.Tasks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

    using PulseBridge.Condo.IO;

    /// <summary>
    /// Represents a Microsoft Build task that gets information about the commits within a repository.
    /// </summary>
    public class GetCommitInfo : Task
    {
        #region Properties
        /// <summary>
        /// Gets the git log if previously retrieved.
        /// </summary>
        public static GitLog GitLog { get; private set; }

        /// <summary>
        /// Gets or sets the root of the repository.
        /// </summary>
        [Output]
        [Required]
        public string RepositoryRoot { get; set; }

        /// <summary>
        /// Gets the commit from which to retrieve commit history.
        /// </summary>
        /// <remarks>
        /// The default value is <see langword="null"/>, which will cause all commits in the entire repository to be
        /// included.
        /// </remarks>
        [Output]
        public string From { get; set; }

        /// <summary>
        /// Gets the commit to which to retrieve commit history.
        /// </summary>
        /// <remarks>
        /// The default value is HEAD, which is the most recent commit.
        /// </remarks>
        [Output]
        public string To { get; set; } = "HEAD";

        /// <summary>
        /// Gets or sets a value indicating whether or not to include invalid commits.
        /// </summary>
        public bool IncludeInvalidCommits { get; set; } = false;

        /// <summary>
        /// Gets or sets the actions used to track references that resolve an issue (work item).
        /// </summary>
        /// <remarks>
        /// The list is semi-colon delimited.
        /// </remarks>
        public string ActionKeywords { get; set; } = "Close;Closes;Closed;Fix;Fixed;Resolve;Resolves;Resolved";

        /// <summary>
        /// Gets or sets the regular expression pattern used to parse the header.
        /// </summary>
        /// <remarks>
        /// The position and number of match groups within the pattern are directly related to the
        /// <see cref="HeaderCorrespondence"/> whereby the index of the match group must match the index of the
        /// correspondence task item.
        /// </remarks>
        public string HeaderPattern { get; set; } = @"^(\w*)(?:\(([\w\$\.\-\* ]*)\))?\: (.*)$";

        /// <summary>
        /// Gets or sets the regular expression pattern used to parse a field within a commit message.
        /// </summary>
        public string FieldPattern { get; set; } = @"^-(.*?)-$";

        /// <summary>
        /// Gets or sets the regular expression pattern used to parse a reversion within a commit message.
        /// </summary>
        public string RevertPattern { get; set; } = @"^Revert\s""([\s\S]*)""\s*This reverts commit (\w*)\.";

        /// <summary>
        /// Gets or sets the regular expression pattern used to parse a merge within a commit message.
        /// </summary>
        public string MergePattern { get; set; }

        /// <summary>
        /// Gets or sets the correspondence for a header.
        /// </summary>
        /// <remarks>
        /// The list is semi-colon delimited.
        /// </remarks>
        public string HeaderCorrespondence { get; set; } = "Type;Scope;Subject";

        /// <summary>
        /// Gets or sets the correspondence for a merge.
        /// </summary>
        /// <remarks>
        /// The list is semi-colon delimited.
        /// </remarks>
        public string MergeCorrespondence { get; set; }

        /// <summary>
        /// Gets or sets the correspondence for a reversion.
        /// </summary>
        /// <remarks>
        /// The list is semi-colon delimited.
        /// </remarks>
        public string RevertCorrespondence { get; set; }

        /// <summary>
        /// Gets or sets the prefixes used to reference an issue (work item) within a commit message.
        /// </summary>
        /// <remarks>
        /// The list is semi-colon delimited.
        /// </remarks>
        public string ReferencePrefixes { get; set; } = "#";

        /// <summary>
        /// Gets or sets the prefixes used to reference an an individual or group within a commit message.
        /// </summary>
        /// <remarks>
        /// The list is semi-colon delimited.
        /// </remarks>
        public string MentionPrefixes { get; set; } = "@";

        /// <summary>
        /// Gets or sets the keywords used to reference a breaking change within a commit message.
        /// </summary>
        /// <remarks>
        /// The list is semi-colon delimited.
        /// </remarks>
        public string NoteKeywords { get; set; } = "BREAKING CHANGE;BREAKING CHANGES";

        /// <summary>
        /// Gets the commits that belong to the current release.
        /// </summary>
        /// <remarks>
        /// Will contain a list of all commits that comply with the indicated format.
        /// The SHA value will be the <see cref="ITaskItem.ItemSpec"/>. In addition, the task items will contain
        /// metadata that describes any commit that follows the conventional-changelog style guide. This metadata
        /// includes the following:
        /// <list type="bullet">
        /// <item>
        ///     <term>Type</term>
        ///     <description>
        ///     The type (of change) for the commit, which may be Features, Performance Improvements, Bug Fixes,
        ///     Documentation, Styles, Code Refactoring, Tests or Chores.
        ///     </description>
        /// </item>
        /// <item>
        ///     <term>Scope</term>
        ///     <description>
        ///     The component in which the change occurred.
        ///     </description>
        /// </item>
        /// <item>
        ///     <term>Subject</term>
        ///     <description>
        ///     The subject, or description, of the change that occurred.
        ///     </description>
        /// </item>
        /// <item>
        ///     <term>Body</term>
        ///     <description>
        ///     The body that describes additional details about the change that occurred.
        ///     </description>
        /// </item>
        /// <item>
        ///     <term>Closes</term>
        ///     <description>
        ///     A semi-colon (;) delimited list of issues (work items) that are closed (resolved) by the commit.
        ///     </description>
        /// </item>
        /// <item>
        ///     <term>References</term>
        ///     <description>
        ///     A semi-colon (;) delimited list of references (work items) that are related to the commit.
        ///     </description>
        /// </item>
        /// <item>
        ///     <term>Breaking</term>
        ///     <description>
        ///     A summary description of breaking changes associated with the commit.
        ///     </description>
        /// </item>
        /// <item>
        ///     <term>Message</term>
        ///     <description>
        ///     The entire commit message including new-lines. This is useful for commits that do not follow the
        ///     conventional changelog style.
        ///     </description>
        /// </item>
        /// </list>
        /// </remarks>
        [Output]
        public ITaskItem[] Commits { get; set; }

        /// <summary>
        /// Gets the latest release tag contained within the repository.
        /// </summary>
        [Output]
        public string LatestTag { get; private set; }

        /// <summary>
        /// Gets the commit hash of the latest tag commit.
        /// </summary>
        [Output]
        public string LatestTagCommit { get; private set; }

        /// <summary>
        /// Gets the version of the client used to access the repository.
        /// </summary>
        [Output]
        public string ClientVersion { get; private set; }
        #endregion

        #region Methods
        /// <summary>
        /// Executes the <see cref="GetRepositoryInfo"/> task.
        /// </summary>
        /// <returns>
        /// A value indicating whether or not the task executed successfully.
        /// </returns>
        public override bool Execute()
        {
            // attempt to get the repository root (walking the parent until we find it)
            var root = GetRepositoryInfo.GetRoot(this.RepositoryRoot);

            // determine if the root could be found
            if (string.IsNullOrEmpty(root))
            {
                // log a warning
                this.Log.LogWarning("The repository root was not specified.");

                // move on immediately
                return true;
            }

            // update the repository root
            this.RepositoryRoot = root;

            try
            {
                // get the commits
                this.Commits = this.GetCommits().ToArray();
            }
            catch (Exception netEx)
            {
                // log a warning
                this.Log.LogWarning(netEx.Message);

                // move on immediately
                return false;
            }

            // we were successful
            return true;
        }

        private IEnumerable<ITaskItem> GetCommits()
        {
            var factory = new GitRepositoryFactory();

            // load the repository
            var repository = factory.Load(this.RepositoryRoot);

            // set the client version
            this.ClientVersion = repository.ClientVersion;

            var headers = this.HeaderCorrespondence.PropertySplit().ToList();
            var merges = this.MergeCorrespondence.PropertySplit().ToList();
            var reversions = this.RevertCorrespondence.PropertySplit().ToList();

            var references = this.ReferencePrefixes.PropertySplit().ToList();
            var mentions = this.MentionPrefixes.PropertySplit().ToList();

            var actionKeywords = this.ActionKeywords.PropertySplit().ToList();
            var noteKeywords = this.NoteKeywords.PropertySplit().ToList();

            // get the options and parser
            var options = new GitLogOptions()
            {
                HeaderPattern = this.HeaderPattern,
                HeaderCorrespondence = headers,

                MergePattern = this.MergePattern,
                MergeCorrespondence = merges,

                RevertPattern = this.RevertPattern,
                RevertCorrespondence = reversions,

                FieldPattern = this.FieldPattern,

                ActionKeywords = actionKeywords,
                NoteKeywords = noteKeywords,

                MentionPrefixes = mentions,
                ReferencePrefixes = references,

                IncludeInvalidCommits = this.IncludeInvalidCommits
            };

            var parser = new GitLogParser();

            // get the commits
            GitLog = repository.Log(this.From, this.To, options, parser);

            // iterate over each commit
            foreach (var commit in GitLog.Commits)
            {
                // create a new task item
                var item = new TaskItem(commit.ShortHash);

                // set well-known metadata
                item.SetMetadata(nameof(commit.Hash), commit.Hash);
                item.SetMetadata(nameof(commit.ShortHash), commit.ShortHash);
                item.SetMetadata(nameof(commit.Header), commit.Header);
                item.SetMetadata(nameof(commit.Body), commit.Body);
                item.SetMetadata(nameof(commit.Raw), commit.Raw);
                item.SetMetadata(nameof(commit.Branches), commit.Branches.PropertyJoin());
                item.SetMetadata(nameof(commit.Tags), commit.Tags.PropertyJoin());
                item.SetMetadata(nameof(commit.References), commit.References.Select(reference => reference.Id).PropertyJoin());

                var count = noteKeywords.Sum(keyword => commit.Notes
                    .Count(note => note.Header.StartsWith(keyword, StringComparison.OrdinalIgnoreCase)));

                // set the metadata
                item.SetMetadata(nameof(commit.Notes), count.ToString());

                // add header correspondence
                foreach (var correspondence in commit.HeaderCorrespondence)
                {
                    item.SetMetadata(correspondence.Key, correspondence.Value);
                }

                // add merge correspondence
                foreach (var correspondence in commit.MergeCorrespondence)
                {
                    item.SetMetadata(correspondence.Key, correspondence.Value);
                }

                // iterate over actions
                foreach (var action in actionKeywords)
                {
                    item.SetMetadata
                    (
                        action,
                        commit.References
                            .Where(reference => reference.Action.Equals(action, StringComparison.OrdinalIgnoreCase))
                            .Select(reference => reference.Id)
                            .PropertyJoin()
                    );
                }

                // return the item
                yield return item;
            }

            var tag = GitLog.Tags.FirstOrDefault();

            this.LatestTag = tag.Name;
            this.LatestTagCommit = tag.Hash;
        }
        #endregion
    }
}
