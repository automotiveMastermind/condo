namespace PulseBridge.Condo.ChangeLog
{
    using System.Collections.Generic;

    public class Commit
    {
        public string Hash { get; set; }

        public string Header { get; set; }

        public string Type { get; set; }

        public string Scope { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public ICollection<Reference> References { get; } = new HashSet<Reference>();
    }
}
