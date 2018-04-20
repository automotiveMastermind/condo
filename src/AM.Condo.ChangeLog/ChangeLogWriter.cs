// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChangeLogWriter.cs" company="automotiveMastermind and contributors">
//   © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AM.Condo.ChangeLog
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using AM.Condo.IO;

    using HandlebarsDotNet;
    using NuGet.Versioning;

    /// <summary>
    /// Represents a change log writer used to generate a changelog from a git commit history.
    /// </summary>
    public class ChangeLogWriter : IChangeLogWriter
    {
        #region Private Fields
        private readonly ChangeLogOptions options;

        private readonly CommitComparer comparer;

        private string changelog;

        private string path;

        private Func<object, string> apply;
        #endregion

        #region Constructors and Finalizers
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeLogWriter"/> class.
        /// </summary>
        public ChangeLogWriter()
            : this(new ChangeLogOptions())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeLogWriter"/> class.
        /// </summary>
        /// <param name="options">
        /// The options used to configure the change log.
        /// </param>
        public ChangeLogWriter(ChangeLogOptions options)
        {
            // set the options
            this.options = options;

            // set the changelog to empty
            this.changelog = string.Empty;

            // no encoding
            Handlebars.Configuration.TextEncoder = null;

            // create the commit comparer
            this.comparer = new CommitComparer(options.SortBy);

            // remove repository uri if not from github
            if (!(options.RepositoryUri?.Contains("//github.com")).GetValueOrDefault())
            {
                // clear out the repository uri
                options.RepositoryUri = null;
            }
        }
        #endregion

        #region Properties and Indexers
        /// <inheritdoc />
        string IChangeLogWriterApplied.ChangeLog => this.changelog;
        #endregion

        #region Methods
        /// <inheritdoc />
        IChangeLogWriterCompiled IChangeLogWriterCanCompile.Load(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (path.Length == 0)
            {
                throw new ArgumentException($"The {nameof(path)} must not be empty", nameof(path));
            }

            // attempt to load the template
            var template = File.ReadAllText(path);

            // parse the template
            return (this as IChangeLogWriterCanCompile).Template(template);
        }

        /// <inheritdoc />
        IChangeLogWriterCompiled IChangeLogWriterCanCompile.Template(string template)
        {
            if (template == null)
            {
                throw new ArgumentNullException(nameof(template));
            }

            if (template.Length == 0)
            {
                throw new ArgumentException($"The {nameof(template)} must not be empty", nameof(template));
            }

            // compile the delegate used to apply the template
            this.apply = Handlebars.Compile(template);

            // return self
            return this;
        }

        /// <inheritdoc />
        IChangeLogWriterApplied IChangeLogWriterCompiled.Apply(GitLog log)
        {
            // determine if the options contains a version
            if (this.options.Version != null)
            {
                // iterate over each commit in the unversioned history
                foreach (var commit in log.Unversioned)
                {
                    // set the version of the commit
                    commit.Version = this.options.Version;
                }

                // add the unversioned commits as a new version
                log.Versions.Add(this.options.Version, log.Unversioned);
            }

            // convert the versions to a list
            var versions = log.Versions.ToList();

            // get the first version
            var current = versions[0];

            // apply the first version
            this.ApplyVersion(current.Key, null, current.Value);

            // iterate over each version
            for (var i = 1; i < versions.Count; i++)
            {
                // capture the previous
                var previous = current;

                // set the current
                current = versions[i];

                // apply the current version
                this.ApplyVersion(current.Key, previous.Key.ToFullString(), current.Value);
            }

            // return self
            return this;
        }

        /// <inheritdoc />
        void IChangeLogWriterApplied.Save()
        {
            // attempt to write the text
            File.AppendAllText(this.path, this.changelog);
        }

        /// <inheritdoc />
        IChangeLogWriterCanCompile IChangeLogWriterCanCompile.LoadPartial(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (path.Length == 0)
            {
                throw new ArgumentException($"The {nameof(path)} must not be empty", nameof(path));
            }

            var name = Path.GetFileNameWithoutExtension(path);
            var partial = File.ReadAllText(path);

            return (this as IChangeLogWriterCanCompile).Partial(name, partial);
        }

        /// <inheritdoc />
        IChangeLogWriterCanCompile IChangeLogWriterCanCompile.Partial(string name, string partial)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (partial == null)
            {
                throw new ArgumentNullException(nameof(partial));
            }

            if (name.Length == 0)
            {
                throw new ArgumentException($"The {nameof(name)} must not be empty", nameof(name));
            }

            if (partial.Length == 0)
            {
                throw new ArgumentException($"The {nameof(partial)} must not be empty", nameof(partial));
            }

            using (var reader = new StringReader(partial))
            {
                var template = Handlebars.Compile(reader);
                Handlebars.RegisterTemplate(name, template);
            }

            return this;
        }

        /// <inheritdoc />
        public IChangeLogWriterCanCompile Initialize(string path, string content)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (path.Length == 0)
            {
                throw new ArgumentException($"The {nameof(path)} must not be empty", nameof(path));
            }

            // set the path
            this.path = path;

            // write all the text
            File.WriteAllText(path, content);

            // return self
            return this;
        }

        private void ApplyVersion(SemanticVersion version, string previous, IList<GitCommit> log)
        {
            // determine if the log does not contain any commits
            if (log.Count == 0)
            {
                // move on immediately
                return;
            }

            var first = log[0];

            // setup the root dictionary
            var root = new Dictionary<string, object>
            {
                { "commit", this.options.CommitName },
                { "issue", this.options.IssueName },
                { "linkReferences", this.options.LinkReferences },
                { "repository", this.options.Repository },
                { "repoUrl", this.options.RepositoryUri },
                { "previousTag", previous },
                { "currentTag", version.ToFullString() },
                { "version", version.ToNormalizedString() },
                { "isPatch", version.Patch > 0 || version.IsPrerelease || version.HasMetadata },
                { "linkCompare", this.options.LinkReferences && !string.IsNullOrEmpty(previous) },
                { "date", first.Date.ToString("yyyy-MM-dd") }
            };

            var commitGroups = new List<IDictionary<string, object>>();
            root.Add("commitGroups", commitGroups);

            var noteGroups = new List<IDictionary<string, object>>();
            root.Add("noteGroups", noteGroups);

            var tempCommits = new SortedDictionary<string, IDictionary<string, object>>();
            var tempNotes = new SortedDictionary<string, IDictionary<string, object>>();

            // iterate over each commit
            foreach (var currentCommit in log)
            {
                // create the commit
                var commit = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase)
                {
                    // add the hash
                    { "hash", currentCommit.ShortHash },

                    // add the header
                    { "header", currentCommit.Header }
                };

                // iterate over all notes on the current commit
                foreach (var currentNote in currentCommit.Notes)
                {
                    // create the note
                    var note = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase)
                    {
                        // add the text and associated commit
                        { "text", currentNote.Body },
                        { "commit", commit }
                    };

                    // get the header value
                    var value = currentNote.Header;

                    // determine if we should include the note type
                    if (!this.options.ChangeLogTypes.TryGetValue(value, out string display))
                    {
                        // move on immediately
                        continue;
                    }

                    // attempt to get the group
                    if (!tempNotes.TryGetValue(display, out IDictionary<string, object> group))
                    {
                        // create the group
                        group = new Dictionary<string, object>();
                        tempNotes.Add(display, group);

                        // add the title
                        group.Add("title", display);

                        // add the notes
                        group.Add("notes", new SortedSet<IDictionary<string, object>>(this.comparer));
                    }

                    // get the notes
                    var notes = group["notes"] as SortedSet<IDictionary<string, object>>;

                    // add the note
                    notes.Add(note);
                }

                // iterate over header correspondence
                foreach (var correspondence in currentCommit.HeaderCorrespondence)
                {
                    // add the header info to the commit
                    commit.Add(correspondence.Key.ToLowerInvariant(), correspondence.Value);
                }

                // attempt to get the group by header
                if (currentCommit.HeaderCorrespondence.TryGetValue(this.options.GroupBy, out var groupBy))
                {
                    // attempt to get the display name mapping
                    if (!this.options.ChangeLogTypes.TryGetValue(groupBy, out string display))
                    {
                        // discard the commit
                        continue;
                    }

                    // attempt to get the group
                    if (!tempCommits.TryGetValue(display, out IDictionary<string, object> group))
                    {
                        // create the group
                        group = new Dictionary<string, object>();
                        tempCommits.Add(display, group);

                        // add the title
                        group.Add("title", display);

                        // add the commits
                        group.Add("commits", new SortedSet<IDictionary<string, object>>(this.comparer));
                    }

                    // get the commits
                    var commits = group["commits"] as SortedSet<IDictionary<string, object>>;

                    // add the commit
                    commits.Add(commit);
                }

                // create the references
                var references = new HashSet<IDictionary<string, object>>();
                commit.Add("references", references);

                // iterate over each reference
                foreach (var currentReference in currentCommit.References)
                {
                    // create the reference
                    var reference = new Dictionary<string, object>();
                    references.Add(reference);

                    // add the owner, issue id, and repository
                    reference.Add("owner", currentReference.Owner);
                    reference.Add("issue", currentReference.Id);
                    reference.Add("repository", currentReference.Repository);
                }
            }

            // iterate over the sorted groups
            foreach (var item in tempCommits)
            {
                // add the groups
                commitGroups.Add(item.Value);
            }

            // iterate over sorted notes
            foreach (var item in tempNotes)
            {
                // add the notes
                noteGroups.Add(item.Value);
            }

            // apply the log
            this.changelog = this.apply(root) + Environment.NewLine + this.changelog;
        }
        #endregion
    }
}
