using System;
using SAML2.Config;

namespace SAML2.Logging
{
    /// <summary>
    /// Logger provider.
    /// </summary>
    public class LoggerProvider
    {
        /// <summary>
        /// Logger provider static instance.
        /// </summary>
        private static LoggerProvider _instance;

        /// <summary>
        /// The logger factory.
        /// </summary>
        private readonly ILoggerFactory _loggerFactory;

        public static Saml2Configuration Configuration { get; set; }

        static LoggerProvider()
        {
            SetLoggerFactory(new LazyLoggerFactory());
        }
        private class LazyLoggerFactory : ILoggerFactory
        {
            private ILoggerFactory loggerFactory;
            private ILoggerFactory LoggerFactory
            {
                get { return loggerFactory = (loggerFactory ?? LocateLoggerFactory()); }
            }
            private ILoggerFactory LocateLoggerFactory()
            {
                string loggerClass = Configuration.Logging.LoggingFactory;
                return string.IsNullOrEmpty(loggerClass) ? new NoLoggingLoggerFactory() : GetLoggerFactory(loggerClass);
            }
            public IInternalLogger LoggerFor(string keyName)
            {
                return LoggerFactory.LoggerFor(keyName);
            }

            public IInternalLogger LoggerFor(Type type)
            {
                return LoggerFactory.LoggerFor(type);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggerProvider"/> class.
        /// </summary>
        /// <param name="loggerFactory">The logger factory.</param>
        private LoggerProvider(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        /// <summary>
        /// Gets a logger for the specified key.
        /// </summary>
        /// <param name="keyName">Name of the key.</param>
        /// <returns>An instance of <see cref="IInternalLogger"/>.</returns>
        public static IInternalLogger LoggerFor(string keyName)
        {
            return _instance._loggerFactory.LoggerFor(keyName);
        }

        /// <summary>
        /// Gets a logger for the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>An instance of <see cref="IInternalLogger"/>.</returns>
        public static IInternalLogger LoggerFor(Type type)
        {
            return _instance._loggerFactory.LoggerFor(type);
        }

        /// <summary>
        /// Sets the logger factory.
        /// </summary>
        /// <param name="loggerFactory">The logger factory.</param>
        public static void SetLoggerFactory(ILoggerFactory loggerFactory)
        {
            _instance = new LoggerProvider(loggerFactory);
        }
        
        /// <summary>
        /// Gets the logger factory.
        /// </summary>
        /// <param name="saml2LoggerClass">The SAML2 logger class.</param>
        /// <returns>The implementation of <see cref="ILoggerFactory"/>.</returns>
        private static ILoggerFactory GetLoggerFactory(string saml2LoggerClass)
        {
            ILoggerFactory loggerFactory;
            var loggerFactoryType = Type.GetType(saml2LoggerClass);
            try
            {
                loggerFactory = (ILoggerFactory)Activator.CreateInstance(loggerFactoryType);
            }
            catch (MissingMethodException ex)
            {
                throw new ApplicationException("Public constructor was not found for " + loggerFactoryType, ex);
            }
            catch (InvalidCastException ex)
            {
                throw new ApplicationException(loggerFactoryType + "Type does not implement " + typeof(ILoggerFactory), ex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Unable to instantiate: " + loggerFactoryType, ex);
            }

            return loggerFactory;
        }
    }
}
