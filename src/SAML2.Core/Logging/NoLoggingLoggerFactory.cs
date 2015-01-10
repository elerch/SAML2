namespace SAML2.Logging
{
    /// <summary>
    /// Logging factory used to create a <see cref="NoLoggingInternalLogger"/>.
    /// </summary>
    public class NoLoggingLoggerFactory : ILoggerFactory
    {
        /// <summary>
        /// No Logging internal logger instance.
        /// </summary>
        private static readonly IInternalLogger NoLogging = new NoLoggingInternalLogger();

        /// <summary>
        /// Gets a logger for the specified name.
        /// </summary>
        /// <param name="keyName">Name of the key.</param>
        /// <returns>An <see cref="IInternalLogger"/> implementation.</returns>
        public IInternalLogger LoggerFor(string keyName)
        {
            return NoLogging;
        }

        /// <summary>
        /// Gets a logger for specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>An <see cref="IInternalLogger"/> implementation.</returns>
        public IInternalLogger LoggerFor(System.Type type)
        {
            return NoLogging;
        }
    }
}
