// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GitLogTest.cs" company="automotiveMastermind and contributors">
//   © automotiveMastermind and contributors. Licensed under MIT. See LICENSE and CREDITS for details.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AM.Condo.IO
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using AM.Condo.Diagnostics;
    using Xunit;
    using Xunit.Abstractions;

    [Purpose(PurposeType.Integration)]
    [Agent(AgentType.CI)]
    public class GitLogTest
    {
        private readonly IGitRepositoryFactory repository = new GitRepositoryFactory();

        private readonly ILogger logger;

        private static readonly Random Random = new Random();

        public GitLogTest(ITestOutputHelper output)
        {
            // create a logger from the output
            this.logger = new TestLogger(output);
        }

        [MemberData(nameof(CommitMessages))]
        [Theory]
        public void GitLog_WhenSimple_Succeeds(CommitMessage expected)
        {
            using (var repo = this.repository.Initialize(this.logger))
            {
                // set the username and email
                repo.Username = "condo";
                repo.Email = "open@amastermind.com";

                // commit
                repo.Commit(expected.Raw);

                // arrange
                foreach (var tag in expected.Tags)
                {
                    repo.Tag(tag);
                }

                // act
                var log = repo.Log();
                var actual = log.Commits.FirstOrDefault();

                // assert
                if (actual == null)
                {
                    // assert that the type and scope is not specified
                    Assert.Empty(expected.Type);
                    Assert.Empty(expected.Scope);

                    // move on immediately
                    return;
                }

                Assert.Equal(expected.Type, actual.HeaderCorrespondence["type"]);
                Assert.Equal(expected.Scope, actual.HeaderCorrespondence["scope"]);
                Assert.Equal(expected.Subject, actual.HeaderCorrespondence["subject"]);
                Assert.Equal(expected.Header, actual.Header);
                Assert.Equal(expected.Body, actual.Body);
                Assert.Equal(expected.Footer, actual.Footer);
                Assert.Equal(expected.Raw, actual.Raw);

                var references = actual.References.Select(a => a.Raw).ToList();
                var tags = actual.Tags.Select(t => t.Name);

                Assert.All(expected.References, reference => Assert.Contains(reference, references));
                Assert.All(expected.Tags, tag => Assert.Contains(tag, tags));
            }
        }

        public static TheoryData<CommitMessage> CommitMessages
        {
            get
            {
                var data = new TheoryData<CommitMessage>();

                for (var i = 1; i < 5; i++)
                {
                    var random = Random.Next();

                    var reference = References[random % References.Count];

                    var commit = new CommitMessage
                    {
                        Type = Types[random % Types.Count],
                        Scope = Scopes[random % Scopes.Count],
                        Subject = Subjects[random % Subjects.Count],
                        Body = (Bodies[random % Bodies.Count] + ' ' + reference).Trim(),
                        Note = Notes[random % Notes.Count],
                        Tags = Tags[random % Tags.Count].Split(',').Where(t => !string.IsNullOrEmpty(t)),
                        References = reference.Split(',').Where(r => !string.IsNullOrEmpty(r))
                    };

                    data.Add(commit);
                }

                return data;
            }
        }

        private static readonly IList<string> Types = new List<string>
        {
            "feat",
            "fix",
            "docs",
            "style",
            "refactor",
            "perf",
            "test",
            "chore",
            "revert",
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty
        };

        private static readonly IList<string> Scopes = new List<string>
        {
            "one",
            "two",
            "three",
            "four",
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty
        };

        private static readonly IList<string> Subjects = new List<string>
        {
            "fix the ticktock on the clock",
            "add a new author to the contributors",
            "test all the things",
            "slap the lesser of the lesser scotts"
        };

        private static readonly IList<string> Bodies = new List<string>
        {
            "let the bodies hit the floor!",
            "what do visual studio say to nuget? you've got a nice package. heh",
            "somebody once told me the world is gonna roll me",
            "This reverts commit abcdefghijklmnopqrstuvwxyz.",
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty
        };

        private static readonly IList<string> Notes = new List<string>
        {
            "BREAKING CHANGE: the world has come to an end",
            "BREAKING CHANGE: do this, or that... i don't really care",
            "BREAKING CHANGE: everything exploded",
            "BREAKING CHANGE: oh, my god... look at her butt",
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty
        };

        private static readonly IList<string> References = new List<string>
        {
            "#34,#92,#99,#58,#293",
            "#123,#998,#578",
            "#246,#9592",
            "#9",
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty
        };

        private static readonly IList<string> Tags = new List<string>
        {
            "1.0.0-beta.2,the-best-ever-tag",
            "2.0.0-alpha.1,what-what-what",
            "3.0.0-rc.3,didnt-do-it",
            "4.0.0",
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty
        };

        public class CommitMessage
        {
            public string Type { get; set; }

            public string Scope { get; set; }

            public string Subject { get; set; }

            public string Body { get; set; }

            public string Header
            {
                get
                {
                    var builder = new StringBuilder();

                    if (!string.IsNullOrEmpty(this.Type))
                    {
                        builder.Append(this.Type);
                    }

                    if (!string.IsNullOrEmpty(this.Scope))
                    {
                        builder.Append('(');
                        builder.Append(this.Scope);
                        builder.Append(')');
                    }

                    if (!string.IsNullOrEmpty(this.Subject))
                    {
                        if (builder.Length > 0)
                        {
                            builder.Append(": ");
                        }

                        builder.Append(this.Subject);
                    }

                    return builder.ToString();
                }
            }

            public string Footer => this.Note;

            public string Raw
            {
                get
                {
                    var builder = new StringBuilder(this.Header);

                    if (!string.IsNullOrEmpty(this.Body))
                    {
                        if (builder.Length > 0)
                        {
                            builder.AppendLine();
                            builder.AppendLine();
                        }

                        builder.Append(this.Body);
                    }

                    if (!string.IsNullOrEmpty(this.Footer))
                    {
                        if (builder.Length > 0)
                        {
                            builder.AppendLine();
                            builder.AppendLine();
                        }

                        builder.Append(this.Footer);
                    }

                    return builder.ToString();
                }
            }

            public string Note { get; set; }

            public IEnumerable<string> References { get; set; }

            public IEnumerable<string> Tags { get; set; }
        }
    }
}
