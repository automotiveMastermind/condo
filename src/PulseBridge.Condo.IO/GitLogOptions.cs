namespace PulseBridge.Condo.IO
{
    using System.Collections.Generic;

    public class GitLogOptions
    {
        public string HeaderPattern { get; set; }

        public string MergePattern { get; set; }

        public string RevertPattern { get; set; }

        public string FieldPattern { get; set; }

        public ICollection<string> HeaderFields { get; }

        public ICollection<string> MergeFields { get; }

        public ICollection<string> IssuePrefixes { get; }

        public ICollection<string> ActionKeywords { get; }

        public ICollection<string> NoteKeywords { get; }
    }
}