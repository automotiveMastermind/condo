namespace PulseBridge.Condo.ChangeLog
{
    using System.Collections.Generic;

    public class NoteGroup
    {
        #region Properties and Indexers
        public string Title { get; set; }

        public ICollection<string> Notes { get; } = new HashSet<string>();
        #endregion
    }
}
