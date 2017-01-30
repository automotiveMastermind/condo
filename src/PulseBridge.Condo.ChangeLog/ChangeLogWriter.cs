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

        private ChangeLog log = new ChangeLog();

        private string changelog;

        private Func<object, string> apply;
        #endregion

        #region Properties and Indexers
        /// <inheritdoc />
        string IChangeLogWriterCompiled.Template => this.template;

        /// <inheritdoc />
        string IChangeLogWriterApplied.ChangeLog => this.changelog;

        /// <inheritdoc />
        ChangeLog IChangeLogWriterApplied.Log => this.log;
        #endregion

        #region Methods
        /// <inheritdoc />
        public IChangeLogWriterCompiled Load(string path)
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
        public IChangeLogWriterCompiled Template(string template)
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

        /// <inheritdoc />
        public IChangeLogWriterCanCompile LoadPartial(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (path.Length == 0)
            {
                throw new ArgumentException($"The {nameof(path)} must not be empty", nameof(path));
            }

            var name = Path.GetFileNameWithoutExtension(path);
            var partial = File.ReadAllText(path);

            return this.Partial(name, partial);
        }

        /// <inheritdoc />
        public IChangeLogWriterCanCompile Partial(string name, string partial)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (partial == null)
            {
                throw new ArgumentNullException(nameof(partial));
            }

            if (name.Length == 0)
            {
                throw new ArgumentException($"The {nameof(name)} must not be empty", nameof(name));
            }

            if (partial.Length == 0)
            {
                throw new ArgumentException($"The {nameof(partial)} must not be empty", nameof(partial));
            }

            using (var reader = new StringReader(partial))
            {
                var template = Handlebars.Compile(reader);
                Handlebars.RegisterTemplate(name, template);
            }

            return this;
        }
        #endregion
    }
}
