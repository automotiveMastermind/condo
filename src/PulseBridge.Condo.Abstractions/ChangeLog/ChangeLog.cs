namespace PulseBridge.Condo.ChangeLog
{
    using System;
    using System.Collections.Generic;

    public class ChangeLog
    {
        #region Properties and Indexers
        public bool IsPatch { get; set; }

        public bool LinkReferences => !(string.IsNullOrEmpty(this.Repository) || string.IsNullOrEmpty(this.RepoUrl));

        public string Repository { get; set; }

        public string Owner { get; set; }

        public string RepoUrl { get; set; }

        public string Version { get; set; }

        public string Title { get; set; }

        public string Date { get; set; } = DateTime.UtcNow.ToString("yyyy-MM-dd");

        public string Commit { get; set; } = "commit";

        public string Issue { get; set; } = "issue";

        public ICollection<CommitGroup> CommitGroups { get; } = new HashSet<CommitGroup>();
        #endregion
    }
}
