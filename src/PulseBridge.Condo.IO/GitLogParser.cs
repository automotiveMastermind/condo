namespace PulseBridge.Condo.IO
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Text.RegularExpressions;

    using static System.FormattableString;

    /// <summary>
    /// Represents a parser for git logs.
    /// </summary>
    public class GitLogParser : IGitLogParser
    {
        private static readonly Regex NoMatchRegex = new Regex("(?!.*)");

        /// <inheritdoc />
        public GitLog Parse(IList<string> lines, IGitLogOptions options)
        {
            // determine if the output is null
            if (lines == null)
            {
                return null;
            }

            // create the git log
            var log = new GitLog();

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

            var mentionRegex = GetMentionRegex(options.MentionPrefixes);
            var noteRegex = GetNotesRegex(options.NoteKeywords);
            var referenceRegex = GetReferenceRegex(options.ActionKeywords, options.ReferencePrefixes);

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

            // create an index
            var i = 0;

            // process all lines
            while (i < lines.Count)
            {
                // get the hash
                var hash = lines[i++];

                // get the abbreviated hash
                var shortHash = lines[i++];

                // get the subject
                var header = lines[i++];

                // create the commit
                var commit = new GitCommit
                {
                    Hash = hash,
                    ShortHash = shortHash,
                    Header = header
                };

                // add the commit to the log
                log.Commits.Add(commit);

                // match for a merge header
                match = mergeRegex.Match(header);

                // add correspondence for merge
                AddCorrespondence(match, options.MergeCorrespondence, commit.MergeCorrespondence);

                // append the header to the raw commit
                raw.AppendLine(header);

                // get the next line
                var line = lines[i++];

                // determine if the merge header existed
                if (match.Success && !options.Split.Equals(line))
                {
                    // set the subject to the line
                    commit.Header = line;

                    // append the line to the raw commit message
                    raw.AppendLine(line);

                    // increment the line
                    line = lines[i++];
                }

                // add references for the header
                AddReferences(referenceRegex.Matches(commit.Header), commit.References);

                // create a variable to retain a note
                var note = default(GitNote);

                // continue processing until the split marker
                while (!options.Split.Equals(line))
                {
                    // match a note
                    match = noteRegex.Match(line);

                    // determine if the note was matched
                    if (match.Success)
                    {
                        // move to the next line
                        line = lines[i++];

                        // create a new note
                        note = new GitNote
                        {
                            Header = match.Groups[1].Value,
                            Body = match.Groups[2].Value
                        };

                        // add the note
                        commit.Notes.Add(note);

                        // break
                        break;
                    }

                    // capture the body
                    section.AppendLine(line);

                    // add references for the line
                    AddReferences(referenceRegex.Matches(line), commit.References);

                    // append the raw line
                    raw.AppendLine(line);

                    // move to the next line
                    line = lines[i++];
                }

                // set the body
                commit.Body = section.ToString().Trim(newline);

                // clear the builder
                section.Clear();

                // continue processing until the split marker
                while (!options.Split.Equals(line))
                {
                    // capture the body
                    section.AppendLine(line);

                    // add references for the line
                    AddReferences(referenceRegex.Matches(line), commit.References);

                    // append the raw line
                    raw.AppendLine(line);

                    // append the footer line
                    footer.Append(line);

                    // move to the next line
                    line = lines[i++];

                    // detect the next match
                    match = noteRegex.Match(line);

                    // determine if the note was match
                    if (match.Success)
                    {
                        // set the note body
                        note.Body += section.ToString().Trim(newline);

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
                    note.Body += section.ToString().Trim(newline);
                }

                // add header correspondence
                AddCorrespondence(headerRegex.Match(header), options.HeaderCorrespondence, commit.HeaderCorrespondence);

                // set the raw and footer
                commit.Raw = raw.ToString();
                commit.Footer = footer.ToString();

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

        private static Regex GetReferenceRegex(ICollection<string> actions, ICollection<string> prefixes)
        {
            if (actions == null || actions.Count == 0)
            {
                return new Regex("()(.+)");
            }

            var a = string.Join("|", actions);
            var p = string.Join("|", prefixes);

            return new Regex(Invariant($"({a})?:?\\s*?([\\w-\\.\\/]*?)({p})([\\w-]*\\d+)"), RegexOptions.IgnoreCase);
        }

        private static void AddReferences(MatchCollection matches, IList<GitReference> references)
        {
            foreach (Match match in matches)
            {
                if (!match.Success)
                {
                    continue;
                }

                var groups = match.Groups;

                var action = groups[1].Value;
                var owner = default(string);
                var repository = groups[2].Value;
                var prefix = groups[3].Value;
                var id = groups[4].Value;
                var raw = groups[0].Value.Trim();

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

        private static void AddCorrespondence(Match match, IList<string> source, IDictionary<string, string> target)
        {
            // determines if the match was successful
            if (!match.Success)
            {
                // move on immediately
                return;
            }

            // count the number of matches
            var matches = match.Groups.Count + 1;

            // iterate over each of the fields
            for (var j = 0; j < source.Count && j < matches; j++)
            {
                // get the group
                var group = match.Groups[j+1];

                // get the name of the header
                var name = source[j];

                // get the value from teh group
                var value = group.Value;

                // add the header
                target.Add(name, value);
            }
        }
    }
}