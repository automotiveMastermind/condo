namespace PulseBridge.Condo.IO
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    using static System.FormattableString;

    using NuGet.Versioning;

    /// <summary>
    /// Represents a parser for git logs.
    /// </summary>
    public class GitLogParser : IGitLogParser
    {
        private static readonly Regex NoMatchRegex = new Regex("(?!.*)");

        private static readonly Regex TagRegex = new Regex("(?:tag:\\s*)(.+)");

        /// <inheritdoc />
        public GitLog Parse(IEnumerable<IList<string>> commits, IGitLogOptions options)
        {
            // create the git log
            var log = new GitLog();

            // determine if the output is null
            if (commits == null || !commits.Any())
            {
                // return the log
                return log;
            }

            // create the regex's
            var headerRegex = string.IsNullOrEmpty(options.HeaderPattern)
                ? NoMatchRegex
                : new Regex(options.HeaderPattern, RegexOptions.IgnoreCase);

            var revertRegex = string.IsNullOrEmpty(options.RevertPattern)
                ? NoMatchRegex
                : new Regex(options.RevertPattern, RegexOptions.IgnoreCase);

            var mergeRegex = string.IsNullOrEmpty(options.MergePattern)
                ? NoMatchRegex
                : new Regex(options.MergePattern, RegexOptions.IgnoreCase);

            var FieldRegex = string.IsNullOrEmpty(options.FieldPattern)
                ? NoMatchRegex
                : new Regex(options.FieldPattern, RegexOptions.IgnoreCase);

            // var mentionRegex = GetMentionRegex(options.MentionPrefixes);
            var noteRegex = GetNotesRegex(options.NoteKeywords);
            var actionRegex = GetActionsRegex(options.ActionKeywords);
            var referenceRegex = GetReferenceRegex(options.ReferencePrefixes);

            // define the match variable used to track regex matches
            var match = default(Match);

            // get newline characters
            var newline = Environment.NewLine.ToCharArray();

            // create an empty string to retain the body or notes
            var section = new StringBuilder();

            // create an empty string builder to retain the complete footer
            var footer = new StringBuilder();

            // create an empty string builder to retain the raw commit
            var raw = new StringBuilder();

            // capture the semantic version of the commits
            var version = default(SemanticVersion);

            // iterate over each commit
            foreach (var item in commits)
            {
                // create an enumerator for the commit and move to the first item
                var lines = item.GetEnumerator();
                lines.MoveNext();

                // get the hash
                var hash = lines.Current;
                lines.MoveNext();

                // get the short hash
                var shortHash = lines.Current;
                lines.MoveNext();

                // get the committer date
                DateTime date;
                DateTime.TryParse(lines.Current, out date);
                lines.MoveNext();

                // create the commit
                var commit = new GitCommit
                {
                    Hash = hash,
                    ShortHash = shortHash,
                    Date = date
                };

                // get the tags
                var tags = lines.Current.Split(',');
                lines.MoveNext();

                // iterate over each tag
                foreach (var tag in tags)
                {
                    // test for a tag
                    match = TagRegex.Match(tag);

                    // determine if tag was found
                    if (match.Success)
                    {
                        // get the label
                        var label = match.Groups[1].Value;

                        // get the current tag
                        var current = new GitTag { Name = label, Hash = hash, ShortHash = shortHash };

                        // get the version sample
                        var sample = current.Version(options.VersionTag);

                        // determine if the version is not null
                        if (sample != null)
                        {
                            // set the version
                            version = sample;
                        }

                        // add the tag
                        commit.Tags.Add(current);
                        log.Tags.Add(current);

                        // move on immediately
                        continue;
                    }

                    // add the tag as a branch reference
                    commit.Branches.Add(tag);
                }

                // get the header
                commit.Header = lines.Current;

                // append the header to the raw commit
                raw.AppendLine(commit.Header);

                // match for a merge header
                match = mergeRegex.Match(commit.Header);

                // add correspondence for merge
                var valid = AddCorrespondence(match, options.MergeCorrespondence, commit.MergeCorrespondence);

                // determine if a merge was found
                if (match.Success && lines.MoveNext())
                {
                    // set the header
                    commit.Header = lines.Current;

                    // append the header to the raw commit
                    raw.AppendLine(commit.Header);
                }

                // add header correspondence
                valid = valid || AddCorrespondence(headerRegex.Match(commit.Header), options.HeaderCorrespondence, commit.HeaderCorrespondence);

                // determine if the commit is not valid and including invalid commits is not allowed
                if (!(valid || options.IncludeInvalidCommits))
                {
                    // move to the next commit
                    continue;
                }

                // add the commit to the log
                log.Commits.Add(commit);

                // determine if the version exists
                if (version != null)
                {
                    // set the commit version
                    commit.Version = version;

                    // get or add the commit to the log versions
                    IList<GitCommit> versioned;

                    if (!log.Versions.TryGetValue(version, out versioned))
                    {
                        // create a new versioned list
                        versioned = new List<GitCommit>();

                        // add the list to the versions dictionary
                        log.Versions.Add(version, versioned);
                    }

                    // add the commit to the versioned collection
                    versioned.Add(commit);
                }
                else
                {
                    // add the commit to the unversioned commits
                    log.Unversioned.Add(commit);
                }

                // add references from the header
                AddReferences(actionRegex.Match(commit.Header), referenceRegex, commit.References);

                // create a variable to retain a note
                var note = default(GitNote);

                // continue processing lines (in the body)
                while (lines.MoveNext())
                {
                    // capture the line
                    var line = lines.Current;

                    // append the line to the raw content
                    raw.AppendLine(line);

                    // add references for the line
                    AddReferences(actionRegex.Match(line), referenceRegex, commit.References);

                    // try to detect a note
                    match = noteRegex.Match(line);

                    // determine if the line was the start of a note
                    if (match.Success)
                    {
                        // create a new note
                        note = new GitNote
                        {
                            Header = match.Groups[1].Value,
                            Body = match.Groups[2].Value
                        };

                        // add the note
                        commit.Notes.Add(note);

                        // append the footer line
                        footer.AppendLine(line);

                        // break from parsing the body
                        break;
                    }

                    // append the line to the current section
                    section.AppendLine(line);
                }

                // set the body
                commit.Body = section.ToString().Trim(newline);

                // clear the section
                section.Clear();

                // continue processing (now in the footer)
                while (lines.MoveNext())
                {
                    // capture the line
                    var line = lines.Current;

                    // capture the content
                    section.AppendLine(line);

                    // append the footer line
                    footer.AppendLine(line);

                    // append the raw line
                    raw.AppendLine(line);

                    // add references for the line
                    AddReferences(actionRegex.Match(line), referenceRegex, commit.References);

                    // detect another note
                    match = noteRegex.Match(line);

                    // determine if the note was match
                    if (match.Success)
                    {
                        // set the note body to the current section content
                        note.Body += section.ToString().TrimEnd(newline);

                        // clear the section
                        section.Clear();

                        // create a new note
                        note = new GitNote
                        {
                            Header = match.Groups[1].Value,
                            Body = match.Groups[2].Value
                        };

                        // add the current note to the commit
                        commit.Notes.Add(note);
                    }
                }

                // determine if additional body info was acquired
                if (section.Length > 0)
                {
                    // set the note body
                    note.Body += section.ToString().TrimEnd(newline);
                }

                // set the raw and footer
                commit.Raw = raw.ToString().Trim(newline);
                commit.Footer = footer.ToString().Trim(newline);

                // clear the raw, footer, and section
                raw.Clear();
                footer.Clear();
                section.Clear();
            }

            // return the log
            return log;
        }

        private static Regex GetNotesRegex(ICollection<string> prefixes)
        {
            if (prefixes == null || prefixes.Count == 0)
            {
                return NoMatchRegex;
            }

            var joined = string.Join("|", prefixes);

            return new Regex(Invariant($"^[\\s|*]*({joined})[:\\s]+(.*)"), RegexOptions.IgnoreCase);
        }

        private static Regex GetMentionRegex(ICollection<string> prefixes)
        {
            if (prefixes == null || prefixes.Count == 0)
            {
                return NoMatchRegex;
            }

            var joined = string.Join("|", prefixes);

            return new Regex(Invariant($"{joined}([\\w-]+)"), RegexOptions.IgnoreCase);
        }

        private static Regex GetActionsRegex(ICollection<string> actions)
        {
            if (actions == null || actions.Count == 0)
            {
                return new Regex("()(.+)");
            }

            var join = string.Join("|", actions);

            return new Regex(Invariant($"^({join})?(.*)$"));
        }

        private static Regex GetReferenceRegex(ICollection<string> prefixes)
        {
            var join = string.Join("|", prefixes);

            return new Regex(Invariant($"([\\w-\\.\\/]*?)({join})([\\w-]*\\d+)"), RegexOptions.IgnoreCase);
        }

        private static void AddReferences
            (Match match, Regex referenceRegex, ICollection<GitReference> references)
        {
            if (!match.Success)
            {
                return;
            }

            var action = match.Groups[1].Value;
            var parts = referenceRegex.Matches(match.Groups[2].Value);

            foreach (Match part in parts)
            {
                var groups = part.Groups;

                var repository = groups[1].Value;
                var prefix = groups[2].Value;
                var id = groups[3].Value;
                var raw = groups[0].Value;
                var owner = string.Empty;

                if (!string.IsNullOrEmpty(action))
                {
                    raw = Invariant($"{action}: {raw}");
                }

                if (!string.IsNullOrEmpty(repository))
                {
                    var split = repository.Split('/');

                    if (split.Length > 1)
                    {
                        owner = split[0];
                        repository = repository.Substring(owner.Length + 1);
                    }
                }

                // create the reference
                var reference = new GitReference
                {
                    Id = id,
                    Action = action,
                    Owner = owner,
                    Repository = repository,
                    Prefix = prefix,
                    Raw = raw
                };

                // add the reference
                references.Add(reference);
            }
        }

        private static bool AddCorrespondence(Match match, IList<string> source, IDictionary<string, string> target)
        {
            // determines if the match was successful
            if (!match.Success)
            {
                // move on immediately
                return false;
            }

            // count the number of matches
            var matches = match.Groups.Count + 1;

            // iterate over each of the fields
            for (var j = 0; j < source.Count && j < matches; j++)
            {
                // get the group
                var group = match.Groups[j + 1];

                // get the name of the header
                var name = source[j];

                // get the value from teh group
                var value = group.Value;

                // add the header
                target.Add(name, value);
            }

            return true;
        }
    }
}
