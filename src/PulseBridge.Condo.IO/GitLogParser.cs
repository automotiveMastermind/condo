using System;

namespace PulseBridge.Condo.IO
{
    using System.Collections.Generic;

    public class GitLogParser : IGitLogParser
    {
        /// <inheritdoc />
        public IGitLog Parse(IEnumerable<string> lines, IGitLogOptions options)
        {
            throw new NotImplementedException();
        }
    }
}