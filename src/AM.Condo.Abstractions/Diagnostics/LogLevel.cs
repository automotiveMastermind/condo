namespace AM.Condo.Diagnostics
{
    /// <summary>
    /// Represents an enumeration of the available log levels used to describe verbosity for logging.
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// HIGH: The log message will be emitted at all verbosity levels.
        /// </summary>
        High = 0,

        /// <summary>
        /// NORMAL: The log message will be emitted at default verbosity levels.
        /// </summary>
        Normal = 1,

        /// <summary>
        /// LOW: The log message will be emitted at detailed verbosity levels.
        /// </summary>
        Low = 2
    }
}
