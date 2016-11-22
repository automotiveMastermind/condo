namespace PulseBridge.Condo.IO
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using static System.FormattableString;

    using Xunit;

    [Purpose(PurposeType.Integration)]
    public class GitLogTest
    {
        private readonly IGitRepositoryFactory repository = new GitRepositoryFactory();

        private static readonly Random Random = new Random();

        [MemberData(nameof(CommitMessages))]
        [Theory]
        public void GitLog_WhenMixedHistory_Succeeds(CommitMessage message)
        {
            using (var repo = this.repository.Initialize().Save().Add())
            {
                // arrange
                repo.Commit(message.Type, message.Scope, message.Subject, message.Body, message.Note);

                var expected = new
                {
                    Type = message.Type,
                    Scope = message.Scope,
                    Subject = message.Subject,
                    Body = message.Body,
                    Header = Invariant($"{message.Type}({message.Scope}): {message.Subject}"),
                    References = string.Join(string.Empty, message.References.Split(' ')).Split(',')
                };

                // act
                var log = repo.Log();
                var actual = log.Commits.First();
                var references = actual.References.Select(a => a.Raw).ToList();

                // assert
                Assert.Equal(expected.Type, actual.HeaderCorrespondence["type"]);
                Assert.Equal(expected.Scope, actual.HeaderCorrespondence["scope"]);
                Assert.Equal(expected.Subject, actual.HeaderCorrespondence["subject"]);
                Assert.Equal(expected.Body, actual.Body);
                Assert.Equal(expected.Header, actual.Header);

                Assert.All
                (
                    expected.References,
                    reference =>
                    {
                        if (!string.IsNullOrEmpty(reference))
                        {
                            Assert.Contains(reference, references);
                        }
                    }
                );
            }
        }

        public static TheoryData<CommitMessage> CommitMessages
        {
            get
            {
                var random = Random.Next(20) + 2;

                var data = new TheoryData<CommitMessage>();

                var items = Enumerable.Range(0, random).Select
                (
                    i => new CommitMessage
                    {
                        Type = Types[i % Types.Count],
                        Scope = Scopes[i % Scopes.Count],
                        Subject = Headers[i % Headers.Count],
                        Body = (Bodies[i % Bodies.Count] + ' ' + References[i % References.Count]).Trim(),
                        Note = Notes[i % Notes.Count],
                        References = References[i % References.Count]
                    }
                );

                foreach (var item in items)
                {
                    data.Add(item);
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
            "five",
            "six",
            "seven",
            string.Empty,
            string.Empty,
            string.Empty
        };

        private static readonly IList<string> Headers = new List<string>
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
            string.Empty,
            string.Empty
        };

        private static readonly IList<string> Notes = new List<string>
        {
            "BREAKING CHANGE: the world has come to an end",
            "BREAKING CHANGE: do this, or that... i don't really care",
            "BREAKING CHANGE: everything exploded",
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty
        };

        private static readonly IList<string> References = new List<string>
        {
            "#34, #92, #99, #58, #293",
            "#123, #998, #578",
            "#246, #9592",
            "#84",
            "#9",
            string.Empty,
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

            public string Note { get; set; }

            public string References { get; set; }
        }
    }
}