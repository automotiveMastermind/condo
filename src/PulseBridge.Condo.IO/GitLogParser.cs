namespace PulseBridge.Condo.IO
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
            var header = string.IsNullOrEmpty(options.HeaderPattern) ? NoMatchRegex : new Regex(options.HeaderPattern);
            var revert = string.IsNullOrEmpty(options.RevertPattern) ? NoMatchRegex : new Regex(options.RevertPattern);
            var merge = string.IsNullOrEmpty(options.MergePattern) ? NoMatchRegex : new Regex(options.MergePattern);
            var field = string.IsNullOrEmpty(options.FieldPattern) ? NoMatchRegex : new Regex(options.FieldPattern);

            var mention = GetMentionRegex(options.MentionPrefixes);
            var action = GetActionsRegex(options.ActionKeywords);
            var note = GetNotesRegex(options.NoteKeywords);
            var reference = GetReferenceRegex(options.ReferencePrefixes);

            // define the match variable used to track regex matches
            var match = default(Match);

            // get newline characters
            var newline = Environment.NewLine.ToCharArray();

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
                var subject = lines[i++];

                // create the commit
                var commit = new GitCommit
                {
                    ShortHash = shortHash,
                    Hash = hash,
                    Subject = subject
                };

                // add the commit to the log
                log.Commits.Add(commit);

                // create an empty string to retain the body or notes
                var section = new StringBuilder();

                // get the next line
                var line = lines[i++];

                // continue processing until the split marker
                while (!options.Split.Equals(line))
                {
                    // capture the body
                    section.AppendLine(line);

                    // move to the next line
                    line = lines[i++];

                    // match a note
                    match = note.Match(line);

                    // determine if the note was matched
                    if (match.Success)
                    {
                        // set the body
                        commit.Body = section.ToString().Trim(newline);

                        // clear the builder
                        section.Clear();

                        // break
                        break;
                    }
                }

                // continue processing until the split marker
                while (!options.Split.Equals(line))
                {
                    // capture the body
                    section.AppendLine(line);

                    // move to the next line
                    line = lines[i++];
                }

                // set the notes
                commit.Notes = section.ToString().Trim(newline);

                // detect a merge commit in the subject
                match = merge.Match(subject);

                // add correspondence for merge
                AddCorrespondence(match, options.MergeCorrespondence, commit.MergeCorrespondence);

                // create a match for the header
                match = header.Match(subject);

                // add header correspondence
                AddCorrespondence(match, options.HeaderCorrespondence, commit.HeaderCorrespondence);
            }

            // return the log
            return log;
        }


        private static Regex GetReferenceRegex(ICollection<string> prefixes)
        {
            if (prefixes == null || prefixes.Count == 0)
            {
                return NoMatchRegex;
            }

            var joined = string.Join("|", prefixes);

            return new Regex(Invariant($"(?:.*?)??\\s*([\\w-\\.\\/]*?)??({joined})([\\w-]*\\d+)"));
        }

        private static Regex GetNotesRegex(ICollection<string> prefixes)
        {
            if (prefixes == null || prefixes.Count == 0)
            {
                return NoMatchRegex;
            }

            var joined = string.Join("|", prefixes);

            return new Regex(Invariant($"^[\\s|*]*({joined})[:\\s]+(.*)"));
        }

        private static Regex GetMentionRegex(ICollection<string> prefixes)
        {
            if (prefixes == null || prefixes.Count == 0)
            {
                return NoMatchRegex;
            }

            var joined = string.Join("|", prefixes);

            return new Regex(Invariant($"{joined}([\\w-]+)"));
        }

        private static Regex GetActionsRegex(ICollection<string> actions)
        {
            if (actions == null || actions.Count == 0)
            {
                return new Regex("()(.+)");
            }

            var joined = string.Join("|", actions);

            return new Regex(Invariant($"({joined})(?:\\s+(.*?))(?=(?:{joined})|$)"));
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