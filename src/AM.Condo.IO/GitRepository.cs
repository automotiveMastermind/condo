// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GitRepository.cs" company="automotiveMastermind and contributors">
//   © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AM.Condo.IO
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using AM.Condo.Diagnostics;

    /// <summary>
    /// Represents a reference to a git repository that can be manipulated programmatically.
    /// </summary>
    public class GitRepository : GitRepositoryBare, IGitRepository
    {
        #region Private Fields
        private const string Split = "------------------------ >8< ------------------------";

        private const string Format = "%H%n%h%n%ci%n%d%n%B%n" + Split;
        #endregion

        #region Constructors and Finalizers
        /// <summary>
        /// Initializes a new instance of the <see cref="GitRepository"/> class.
        /// </summary>
        /// <param name="path">
        /// The path manager that is responsible for managing the path.
        /// </param>
        public GitRepository(IPathManager path)
            : this(path, new NoOpLogger())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GitRepository"/> class.
        /// </summary>
        /// <param name="path">
        /// The path manager that is responsible for managing the path.
        /// </param>
        /// <param name="logger">
        /// The logger that is responsible for logging.
        /// </param>
        public GitRepository(IPathManager path, ILogger logger)
            : base(path, logger)
        {
        }
        #endregion

        #region Properties and Indexers
        /// <inheritdoc/>
        public string Username
        {
            get
            {
                var result = this.Execute("config --get user.name");

                // return the username
                return result.Success ? result.Output.FirstOrDefault() : null;
            }

            set
            {
                this.Execute($@"config user.name ""{value}""", throwOnError: true);
            }
        }

        /// <inheritdoc/>
        public string Email
        {
            get
            {
                var result = this.Execute("config --get user.email");

                // return the email
                return result.Success ? result.Output.FirstOrDefault() : null;
            }

            set
            {
                this.Execute($@"config user.email ""{value}""", throwOnError: true);
            }
        }

        /// <inheritdoc/>
        public string OriginUri
        {
            get
            {
                // get the result
                var result = this.Execute("ls-remote --get-url origin");

                // return the origin uri
                return result.Success ? result.Output.FirstOrDefault() : null;
            }
        }

        /// <inheritdoc/>
        public IEnumerable<string> Tags
        {
            get
            {
                // pull all tags
                this.Pull(all: true);

                // determine what command to run
                var cmd = this.ClientVersion?.Major > 1 ? "tag --sort version:refname" : "tag";

                // execute the command
                var result = this.Execute(cmd);

                // return the result
                return result.Success ? result.Output : new List<string>();
            }
        }

        /// <inheritdoc/>
        public string Authorization
        {
            get
            {
                // get the authorization header
                return this.Config($"http.{this.OriginUri}.extraheader");
            }

            set
            {
                this.Config($"http.{this.OriginUri}.extraheader", value);
            }
        }
        #endregion

        #region Methods
        /// <inheritdoc/>
        public IGitRepositoryInitialized Initialize()
        {
            // execute the init command
            this.Execute("init", throwOnError: true);

            return this;
        }

        /// <inheritdoc/>
        public IGitRepositoryInitialized Push(string remote, bool tags)
        {
            var cmd = "push";

            if (tags)
            {
                cmd += " --follow-tags";
            }

            if (!string.IsNullOrEmpty(remote))
            {
                cmd += $" --set-upstream {remote}";
            }

            this.Execute(cmd);

            return this;
        }

        /// <inheritdoc/>
        public IGitRepositoryInitialized Pull(bool all)
        {
            var cmd = "pull";

            if (all)
            {
                cmd += " --all --tags";
            }

            this.Execute(cmd);

            return this;
        }

        /// <inheritdoc/>
        public IGitRepositoryInitialized Clean()
        {
            var cmd = "clean -xdf";

            this.Execute(cmd);

            return this;
        }

        /// <inheritdoc/>
        public IGitRepositoryBare Bare()
        {
            var output = this.Execute("init --bare", throwOnError: true);

            return this;
        }

        /// <inheritdoc/>
        public IGitRepositoryInitialized Clone(string uri)
        {
            // execute the cmd
            this.Execute($"clone {uri} .", throwOnError: true);

            return this;
        }

        /// <inheritdoc/>
        public IGitRepositoryInitialized Save(string relativePath, string contents)
        {
            // save the contents using the path manager
            this.PathManager.Save(relativePath, contents);

            return this;
        }

        /// <inheritdoc/>
        public IGitRepositoryInitialized Add(string spec, bool force)
        {
            // create the command
            var cmd = force ? $@"add ""{spec}"" --force" : $@"add ""{spec}""";

            // execute the command
            this.Execute(cmd, throwOnError: true);

            // return self
            return this;
        }

        /// <inheritdoc/>
        public IGitRepositoryInitialized Remove(string spec, bool recursive)
        {
            // create the command
            var cmd = recursive ? $@"rm ""{spec}"" -r" : $@"rm ""{spec}""";

            // execute the command
            this.Execute(cmd, throwOnError: true);

            // return self
            return this;
        }

        /// <inheritdoc/>
        public IGitRepositoryInitialized Branch(string name, string source)
        {
            var cmd = $"checkout -b {name}";

            if (!string.IsNullOrEmpty(source))
            {
                cmd += $" {source}";
            }

            // execute the checkout command
            this.Execute(cmd, throwOnError: true);

            // return self
            return this;
        }

        /// <inheritdoc/>
        public IGitRepositoryInitialized Checkout(string name)
        {
            // checkout the branch
            this.Execute($"checkout {name}", throwOnError: true);

            // return self
            return this;
        }

        /// <inheritdoc/>
        public IGitRepositoryInitialized Commit(string message)
        {
            this.Execute($@"commit --allow-empty -m ""{message}""", throwOnError: true);

            return this;
        }

        /// <inheritdoc/>
        public IGitRepositoryInitialized Tag(string name, string message)
        {
            var cmd = string.IsNullOrEmpty(message)
                ? $"tag {name}"
                : $@"tag -a {name} -m ""${message}""";

            this.Execute(cmd, throwOnError: true);

            return this;
        }

        /// <inheritdoc/>
        public IGitRepositoryInitialized RestoreSubmodules(bool recursive)
        {
            var cmd = "submodule update --init";

            if (recursive)
            {
                cmd += " --recursive";
            }

            this.Execute(cmd);

            return this;
        }

        /// <inheritdoc/>
        public IGitRepositoryInitialized Condo(string root)
        {
            if (root == null)
            {
                throw new ArgumentNullException(nameof(root), $"The {nameof(root)} parameter cannot be null.");
            }

            if (string.IsNullOrEmpty(root))
            {
                throw new ArgumentException($"The {nameof(root)} parameter cannot be empty.", nameof(root));
            }

            if (!Directory.Exists(root))
            {
                throw new ArgumentException($"The {nameof(root)} path does not exist.", nameof(root));
            }

            var template = Path.Combine(root, "template");

            if (!Directory.Exists(template))
            {
                throw new ArgumentException
                    ($"The {nameof(root)} path does not contain the condo template.", nameof(root));
            }

            foreach (var source in Directory.GetFiles(template))
            {
                var destination = Path.Combine(this.RepositoryPath, Path.GetFileName(source));

                File.Copy(source, destination);
            }

            File.Copy(Path.Combine(root, "condo.build"), Path.Combine(this.RepositoryPath, "condo.build"));

            return this;
        }

        /// <inheritdoc />
        public GitLog Log(string from, string to, GitLogOptions options, IGitLogParser parser)
        {
            // create the range
            var range = from ?? (from = string.Empty);

            // determine if to is specified
            if (!string.IsNullOrEmpty(to))
            {
                if (range.Length > 0)
                {
                    range += "..";
                }

                range += to;
            }

            var cmd = $@"log {range} --format=""{Format}""";

            // create the command used to get the history of commits
            var exec = this.Execute(cmd);

            // log the output
            this.Logger.LogMessage(exec.Output, LogLevel.Low);

            // determine if we were successful
            if (!exec.Success)
            {
                throw new InvalidOperationException(string.Join(Environment.NewLine, exec.Error));
            }

            // parse the output
            var log = parser.Parse(GetCommits(exec.Output), options);

            // set the from/to ranges
            log.From = from ?? "<earliest>";
            log.To = to ?? "<latest>";

            // return the log
            return log;
        }

        /// <inheritdoc />
        public string RevParse(string reference)
        {
            var output = this.Execute($"rev-parse {reference}");

            return output.Success ? output.Output.FirstOrDefault() : string.Empty;
        }

        /// <inheritdoc />
        public IGitRepositoryInitialized AddRemote(string name, string uri)
        {
            // create the cmd
            var cmd = $"remote add {name} {uri}";

            // execute the cmd
            this.Execute(cmd);

            // return self
            return this;
        }

        /// <inheritdoc />
        public IGitRepositoryInitialized SetRemoteUrl(string name, string uri)
        {
            // create the cmd
            var cmd = $"remote set-url {name} {uri}";

            // execute the cmd
            this.Execute(cmd);

            // return self
            return this;
        }

        /// <inheritdoc />
        public IGitRepositoryInitialized GitFlow(GitFlowOptions options)
        {
            // initialize git flow
            var output = this.Execute("flow init -d");

            // determine if we were not successful
            if (!output.Success)
            {
                // return self
                return this;
            }

            // set the gitflow options
            this.Execute($"config --local gitflow.branch.master ${options.ProductionReleaseBranch}");
            this.Execute($"config --local gitflow.branch.develop ${options.NextReleaseBranch}");
            this.Execute($"config --local gitflow.prefix.feature ${options.FeatureBranchPrefix}/");
            this.Execute($"config --local gitflow.branch.bugfix ${options.BugfixBranchPrefix}/");
            this.Execute($"config --local gitflow.branch.release ${options.ReleaseBranchPrefix}/");
            this.Execute($"config --local gitflow.branch.hotfix ${options.HotfixBranchPrefix}/");
            this.Execute($"config --local gitflow.branch.support ${options.SupportBranchPrefix}/");
            this.Execute($"config --local gitflow.branch.versiontag ${options.VersionTagPrefix}");

            // return self
            return this;
        }

        /// <inheritdoc />
        public IGitRepositoryInitialized StartFlow(string type, string name, string source)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (type.Equals(string.Empty))
            {
                throw new ArgumentException($"The {nameof(type)} cannot be empty.");
            }

            if (name.Equals(string.Empty))
            {
                throw new ArgumentException($"The {nameof(name)} cannot be empty.");
            }

            var cmd = $"flow {type} start {name}";

            if (!string.IsNullOrEmpty(source))
            {
                cmd += $" {source}";
            }

            // execute the cmd
            this.Execute(cmd);

            // return self
            return this;
        }

        /// <inheritdoc />
        public IGitRepositoryInitialized RemoveTag(string tag)
        {
            if (tag == null)
            {
                throw new ArgumentNullException(nameof(tag));
            }

            if (tag.Equals(string.Empty))
            {
                throw new ArgumentException($"The {nameof(tag)} cannot be empty.");
            }

            var remote = $"push origin :{tag}";
            var local = $"tag --delete {tag}";

            var output = this.Execute(remote);

            if (output.Success)
            {
                output = this.Execute(local);
            }

            if (!output.Success)
            {
                this.Logger.LogWarning(output.Error);
            }

            return this;
        }

        /// <inheritdoc />
        public IGitRepositoryInitialized FinishFlow()
        {
            // create the git flow finish command
            var cmd = "flow finish";

            // execute the command
            this.Execute(cmd);

            // return self
            return this;
        }

        /// <inheritdoc />
        public string Config(string key)
        {
            // create the command
            var cmd = $"config --get {key}";

            // get the result
            var result = this.Execute(cmd);

            // return the result
            return result.Success ? result.Output.FirstOrDefault() : null;
        }

        /// <inheritdoc />
        public void Config(string key, string value)
        {
            // create the command
            var cmd = $@"config --replace-all {key} ""{value}""";

            // execute the command
            this.Execute(cmd, throwOnError: true);
        }

        private static IEnumerable<IList<string>> GetCommits(IEnumerable<string> lines)
        {
            var set = new List<string>();

            foreach (var line in lines)
            {
                if (Split.Equals(line))
                {
                    yield return set;

                    set = new List<string>();

                    continue;
                }

                set.Add(line);
            }
        }
        #endregion
    }
}
