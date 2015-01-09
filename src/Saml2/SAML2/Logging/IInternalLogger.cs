using System;

namespace SAML2.Logging
{
    /// <summary>
    /// Interface for all internal logging implementations.
    /// </summary>
    public interface IInternalLogger
    {
        /// <summary>
        /// Gets a value indicating whether this instance is debug enabled.
        /// </summary>
        bool IsDebugEnabled { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is error enabled.
        /// </summary>
        bool IsErrorEnabled { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is fatal enabled.
        /// </summary>
        bool IsFatalEnabled { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is info enabled.
        /// </summary>
        bool IsInfoEnabled { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is warn enabled.
        /// </summary>
        bool IsWarnEnabled { get; }

        /// <summary>
        /// Logs specified debug message.
        /// </summary>
        /// <param name="message">The message.</param>
        void Debug(object message);

        /// <summary>
        /// Logs specified debug message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        void Debug(object message, Exception exception);

        /// <summary>
        /// Logs specified debug message.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        void DebugFormat(string format, params object[] args);
        
        /// <summary>
        /// Logs specified error message.
        /// </summary>
        /// <param name="message">The message.</param>
        void Error(object message);

        /// <summary>
        /// Logs specified error message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        void Error(object message, Exception exception);

        /// <summary>
        /// Logs specified error message.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        void ErrorFormat(string format, params object[] args);

        /// <summary>
        /// Logs specified fatal error message.
        /// </summary>
        /// <param name="message">The message.</param>
        void Fatal(object message);

        /// <summary>
        /// Logs specified fatal error message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        void Fatal(object message, Exception exception);

        /// <summary>
        /// Logs specified info message.
        /// </summary>
        /// <param name="message">The message.</param>
        void Info(object message);

        /// <summary>
        /// Logs specified info message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        void Info(object message, Exception exception);

        /// <summary>
        /// Logs specified info message.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        void InfoFormat(string format, params object[] args);

        /// <summary>
        /// Logs specified warn message.
        /// </summary>
        /// <param name="message">The message.</param>
        void Warn(object message);

        /// <summary>
        /// Logs specified warn message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        void Warn(object message, Exception exception);

        /// <summary>
        /// Logs specified warn message.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        void WarnFormat(string format, params object[] args);
    }
}
