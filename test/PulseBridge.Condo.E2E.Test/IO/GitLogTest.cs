namespace PulseBridge.Condo.IO
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    using Xunit;

    [Purpose(PurposeType.Integration)]
    public class GitLogTest
    {
        private readonly IGitRepositoryFactory repository = new GitRepositoryFactory();

        private readonly Random random = new Random();

        private readonly IList<string> types = new List<string>
        {
            "feat",
            "fix",
            "docs",
            "style",
            "refactor",
            "perf",
            "test",
            "chore",
            "revert"
        };

        private readonly IList<string> scopes = new List<string>
        {
            "one",
            "two",
            "three",
            "four",
            "five",
            "six",
            "seven"
        };

        private readonly IList<string> subjects = new List<string>
        {
            "fix the ticktock on the clock",
            "add a new author to the contributors",
            "test all the things",
            "slap the lesser of the lesser scotts"
        };

        private readonly IList<string> bodies = new List<string>
        {
            "let the bodies hit the floor!",
            "what do visual studio say to nuget? you've got a nice package. heh",
            "somebody once told me the world is gonna roll me",
            "",
            "",
            "",
            "",
            ""
        };

        private readonly IList<string> notes = new List<string>
        {
            "BREAKING CHANGE: the world has come to an end",
            "NOTE: do this, or that... i don't really care",
            "BOOM: everything exploded",
            "",
            "",
            "",
            "",
            ""
        };

        private readonly string template = "{1}{2}: {3}{0}{0}{4}{0}{0}{5}";

        public string CreateMessage(int iteration)
        {
            var type = this.types[iteration % this.types.Count];
            var subject = this.subjects[iteration % this.subjects.Count];
            var body = this.bodies[iteration % this.bodies.Count];
            var note = this.notes[iteration % this.notes.Count];

            return string.Format
                (CultureInfo.InvariantCulture, template, Environment.NewLine, type, subject, body, note);
        }

        [Fact]
        public void GitLog_WhenSimpleHistory_Succeeds()
        {
            var random = this.random.Next(20) + 2;
            var messages = Enumerable.Range(0, random).Select(i => this.CreateMessage(i));

            var type = this.types[random % this.types.Count];
            var subject = this.subjects[random % this.subjects.Count];
            var body = this.bodies[random % this.bodies.Count];
            var note = this.notes[random % this.notes.Count];

            using (var repo = this.repository.Initialize().Commit("initial"))
            {
            }
        }

        [Fact]
        public void GitLog_WhenMixedHistory_Succeeds()
        {
        }

        [Fact]
        public void GitLog_WhenPartialHistory_Succeeds()
        {
        }
    }
}