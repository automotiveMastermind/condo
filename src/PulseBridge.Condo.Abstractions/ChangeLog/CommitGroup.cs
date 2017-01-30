namespace PulseBridge.Condo.ChangeLog
{
    using System.Collections.Generic;

    public class CommitGroup
    {
        public string Title { get; set; }

        public ICollection<Commit> Commits { get; } = new HashSet<Commit>();
    }
}
