namespace SAML2.Logging
{
    /// <summary>
    /// Interface for all logger factory implementations.
    /// </summary>
    public interface ILoggerFactory
    {
        /// <summary>
        /// Gets a logger for the specified name.
        /// </summary>
        /// <param name="keyName">Name of the key.</param>
        /// <returns>An <see cref="IInternalLogger"/> implementation.</returns>
        IInternalLogger LoggerFor(string keyName);

        /// <summary>
        /// Gets a logger for specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>An <see cref="IInternalLogger"/> implementation.</returns>
        IInternalLogger LoggerFor(System.Type type);
    }
}
