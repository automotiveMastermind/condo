namespace PulseBridge.Condo.ChangeLog
{
    using System;
    using System.IO;

    using HandlebarsDotNet;
    using PulseBridge.Condo.IO;

    /// <summary>
    /// Represents a change log writer used to generate a changelog from a git commit history.
    /// </summary>
    public class ChangeLogWriter : IChangeLogWriter
    {
        #region Private Fields
        private string template;

        private GitLog log;

        private string changelog;

        private Func<object, string> apply;
        #endregion

        #region Properties and Indexers
        /// <inheritdoc />
        string IChangeLogWriterCompiled.Template => this.template;

        /// <inheritdoc />
        string IChangeLogWriterApplied.ChangeLog => this.changelog;

        /// <inheritdoc />
        GitLog IChangeLogWriterApplied.Log => this.log;
        #endregion

        #region Methods
        /// <inheritdoc />
        IChangeLogWriterCompiled IChangeLogWriterCanCompile.Load(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (path.Length == 0)
            {
                throw new ArgumentException($"The {nameof(path)} must not be empty", nameof(path));
            }

            // attempt to load the template
            var template = File.ReadAllText(path);

            // parse the template
            return (this as IChangeLogWriterCanCompile).Template(template);
        }

        /// <inheritdoc />
        IChangeLogWriterCompiled IChangeLogWriterCanCompile.Template(string template)
        {
            if (template == null)
            {
                throw new ArgumentNullException(nameof(template));
            }

            if (template.Length == 0)
            {
                throw new ArgumentException($"The {nameof(template)} must not be empty", nameof(template));
            }

            // set the template
            this.template = template;

            // compile the delegate used to apply the template
            this.apply = Handlebars.Compile(template);

            // return self
            return this;
        }

        /// <inheritdoc />
        IChangeLogWriterApplied IChangeLogWriterCompiled.Apply(GitLog log)
        {
            // attempt to apply the template
            this.changelog = this.apply(log);

            // return self
            return this;
        }

        /// <inheritdoc />
        void IChangeLogWriterApplied.Save(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (path.Length == 0)
            {
                throw new ArgumentException($"The {nameof(path)} must not be empty", nameof(path));
            }

            // attempt to write the text
            File.WriteAllText(path, this.changelog);
        }
        #endregion
    }
}
