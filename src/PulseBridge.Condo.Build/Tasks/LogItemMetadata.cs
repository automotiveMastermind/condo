namespace PulseBridge.Condo.Build.Tasks
{
    using System.Linq;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

    /// <summary>
    /// Represents a Microsoft Build task that logs all metadata associated with one or more items.
    /// </summary>
    public class LogItemMetadata : Task
    {
        #region Properties
        /// <summary>
        /// Gets or sets the collection of items for which to log associated metadata.
        /// </summary>
        [Required]
        public ITaskItem[] Items { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Executes the <see cref="LogItemMetadata"/> task.
        /// </summary>
        /// <returns>
        /// A value indicating whether or not the task was successfully executed.
        /// </returns>
        public override bool Execute()
        {
            // iterate over each item
            foreach (var item in this.Items)
            {
                // log the item specification
                Log.LogMessage(item.ItemSpec);

                // iterate over each metdata name
                foreach (var name in item.MetadataNames.Cast<string>())
                {
                    // get the value for the metadata
                    var value = item.GetMetadata(name);

                    // log the name of the metdata and its associated value
                    Log.LogMessage($" {name}: {value}");
                }
            }

            // should always succeed
            return true;
        }
        #endregion
    }
}