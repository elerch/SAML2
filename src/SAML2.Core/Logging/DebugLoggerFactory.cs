using System;

namespace SAML2.Logging
{
    public class DebugLoggerFactory : ILoggerFactory
    {
        private readonly IInternalLogger verboseLogger = new VerboseLogger(s => {
            System.Diagnostics.Debug.WriteLine(s);
        });

        public IInternalLogger LoggerFor(Type type)
        {
            return LoggerFor((string)null);
        }

        public IInternalLogger LoggerFor(string keyName)
        {
            return verboseLogger;
        }

        private class VerboseLogger : IInternalLogger
        {
            private readonly Action<string> write;
            public VerboseLogger(Action<string> write)
            {
                if (write == null) throw new ArgumentNullException("write");
                this.write = write;
            }
            public bool IsDebugEnabled
            {
                get
                {
                    return true;
                }
            }

            public bool IsErrorEnabled
            {
                get
                {
                    return true;
                }
            }

            public bool IsFatalEnabled
            {
                get
                {
                    return true;
                }
            }

            public bool IsInfoEnabled
            {
                get
                {
                    return true;
                }
            }

            public bool IsWarnEnabled
            {
                get
                {
                    return true;
                }
            }

            public void Debug(object message)
            {
                Write("DEBUG", message);
            }


            private void WriteFormat(string verbosity, string format, params object[] args)
            {
                Write(verbosity, string.Format(format, args));
            }
            private void Write(string verbosity, object message, Exception exception = null)
            {
                if (exception != null)
                    message = message.ToString() + ", EXCEPTION: " + exception.Message;
                write(string.Format("{0}: {1}", verbosity, message));
            }

            public void Debug(object message, Exception exception)
            {
                Write("DEBUG", message, exception);
            }

            public void DebugFormat(string format, params object[] args)
            {
                WriteFormat("DEBUG", format, args);
            }

            public void Error(object message)
            {
                Write("ERROR", message);
            }

            public void Error(object message, Exception exception)
            {
                Write("ERROR", message, exception);
            }

            public void ErrorFormat(string format, params object[] args)
            {
                WriteFormat("ERROR", format, args);
            }

            public void Fatal(object message)
            {
                Write("FATAL", message);
            }

            public void Fatal(object message, Exception exception)
            {
                Write("FATAL", message, exception);
            }

            public void Info(object message)
            {
                Write("INFO", message);
            }

            public void Info(object message, Exception exception)
            {
                Write("INFO", message, exception);
            }

            public void InfoFormat(string format, params object[] args)
            {
                WriteFormat("INFO", format, args);
            }

            public void Warn(object message)
            {
                Write("WARN", message);
            }

            public void Warn(object message, Exception exception)
            {
                Write("WARN", message, exception);
            }

            public void WarnFormat(string format, params object[] args)
            {
                WriteFormat("WARN", format, args);
            }
        }
    }
}
