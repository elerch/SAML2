using System;

namespace SAML2.Logging
{
    /// <summary>
    /// Internal logger implementation that provides no logging services.
    /// </summary>
    public class NoLoggingInternalLogger : IInternalLogger
    {
        /// <summary>
        /// Gets a value indicating whether this instance is debug enabled.
        /// </summary>
        public bool IsDebugEnabled
        {
            get { return false; }
        }
        
        /// <summary>
        /// Gets a value indicating whether this instance is error enabled.
        /// </summary>
        public bool IsErrorEnabled
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is fatal enabled.
        /// </summary>
        public bool IsFatalEnabled
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is info enabled.
        /// </summary>
        public bool IsInfoEnabled
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is warn enabled.
        /// </summary>
        public bool IsWarnEnabled
        {
            get { return false; }
        }

        /// <summary>
        /// Logs specified debug message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Debug(object message)
        {
        }

        /// <summary>
        /// Logs specified debug message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public void Debug(object message, Exception exception)
        {
        }

        /// <summary>
        /// Logs specified debug message.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        public void DebugFormat(string format, params object[] args)
        {
        }
        
        /// <summary>
        /// Logs specified error message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Error(object message)
        {
        }

        /// <summary>
        /// Logs specified error message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public void Error(object message, Exception exception)
        {
        }

        /// <summary>
        /// Logs specified error message.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        public void ErrorFormat(string format, params object[] args)
        {
        }

        /// <summary>
        /// Logs specified fatal error message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Fatal(object message)
        {
        }

        /// <summary>
        /// Logs specified fatal error message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public void Fatal(object message, Exception exception)
        {
        }

        /// <summary>
        /// Logs specified info message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Info(object message)
        {
        }

        /// <summary>
        /// Logs specified info message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public void Info(object message, Exception exception)
        {
        }

        /// <summary>
        /// Logs specified info message.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        public void InfoFormat(string format, params object[] args)
        {
        }

        /// <summary>
        /// Logs specified warn message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Warn(object message)
        {
        }

        /// <summary>
        /// Logs specified warn message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public void Warn(object message, Exception exception)
        {
        }

        /// <summary>
        /// Logs specified warn message.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        public void WarnFormat(string format, params object[] args)
        {
        }
    }
}
