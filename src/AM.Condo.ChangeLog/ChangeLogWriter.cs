namespace AM.Condo.ChangeLog
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using HandlebarsDotNet;
    using NuGet.Versioning;
    using AM.Condo.IO;

    /// <summary>
    /// Represents a change log writer used to generate a changelog from a git commit history.
    /// </summary>
    public class ChangeLogWriter : IChangeLogWriter
    {
        #region Private Fields
        private string changelog;

        private string path;

        private Func<object, string> apply;

        private readonly ChangeLogOptions options;
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
            var root = new Dictionary<string, object>();
            root.Add("commit", options.CommitName);
            root.Add("issue", options.IssueName);
            root.Add("linkReferences", options.LinkReferences);
            root.Add("repository", options.Repository);
            root.Add("repoUrl", options.RepositoryUri);
            root.Add("previousTag", previous);
            root.Add("currentTag", version.ToFullString());
            root.Add("version", version.ToNormalizedString());
            root.Add("isPatch", version.Patch > 0 || version.IsPrerelease || version.HasMetadata);
            root.Add("linkCompare", options.LinkReferences && !string.IsNullOrEmpty(previous));
            root.Add("date", first.Date.ToString("yyyy-MM-dd"));

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
                var commit = new Dictionary<string, object>();

                // add the hash
                commit.Add("hash", currentCommit.ShortHash);

                // add the header
                commit.Add("header", currentCommit.Header);

                var include = false;

                // iterate over all notes on the current commit
                foreach (var currentNote in currentCommit.Notes)
                {
                    include = true;

                    // create the note
                    var note = new Dictionary<string, object>();

                    // add the text and associated commit
                    note.Add("text", currentNote.Body);
                    note.Add("commit", commit);

                    // define a variable to retain the group
                    IDictionary<string, object> group;

                    // get the header value
                    var value = currentNote.Header;

                    // attempt to get the group
                    if (!tempNotes.TryGetValue(value, out group))
                    {
                        // create the group
                        group = new Dictionary<string, object>();
                        tempNotes.Add(value, group);

                        // add the title
                        group.Add("title", value);

                        // add the notes
                        group.Add("notes", new List<IDictionary<string, object>>());
                    }

                    // get the notes
                    var notes = group["notes"] as List<IDictionary<string, object>>;

                    // add the note
                    notes.Add(note);
                }

                // iterate over header correspondence
                foreach (var correspondence in currentCommit.HeaderCorrespondence)
                {
                    // capture the key
                    var key = correspondence.Key;
                    var value = correspondence.Value;

                    // add the key
                    commit.Add(key.ToLowerInvariant(), correspondence.Value);

                    // determine if this is the group by key
                    if (key.Equals(this.options.GroupBy, StringComparison.OrdinalIgnoreCase))
                    {
                        // capture the display name
                        string display = value;

                        // attempt to get the display name mapping
                        if (!this.options.ChangeLogTypes.TryGetValue(value, out display) && !include)
                        {
                            // discard the commit
                            continue;
                        }

                        // define a variable to retain the group
                        IDictionary<string, object> group;

                        // attempt to get the group
                        if (!tempCommits.TryGetValue(value, out group))
                        {
                            // create the group
                            group = new Dictionary<string, object>();
                            tempCommits.Add(value, group);

                            // add the title
                            group.Add("title", display);

                            // add the commits
                            group.Add("commits", new List<IDictionary<string, object>>());
                        }

                        // get the commits
                        var commits = group["commits"] as List<IDictionary<string, object>>;

                        // add the commit
                        commits.Add(commit);
                    }
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
        #endregion
    }
}
