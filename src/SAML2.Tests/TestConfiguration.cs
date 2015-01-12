using SAML2.Config;
using SAML2.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAML2.Tests
{
    public class TestConfiguration
    {
        public static Saml2Configuration Configuration
        {
            get
            {
                var config = new Saml2Configuration
                {
                    AllowedAudienceUris = new System.Collections.Generic.List<Uri> { new Uri("urn:borger.dk:id"), new Uri("https://saml.safewhere.net") },
                    LoggingFactoryType = typeof(LoggingFactory).AssemblyQualifiedName,
                };
                Logging.LoggerProvider.Configuration = config;
                return config;
            }
        }
        public class LoggingFactory : ILoggerFactory
        {
            private static readonly IInternalLogger debugLogger = new DebugLogger();
            public IInternalLogger LoggerFor(Type type)
            {
                return debugLogger;
            }

            public IInternalLogger LoggerFor(string keyName)
            {
                return debugLogger;
            }
            private class DebugLogger : IInternalLogger
            {
                public bool IsDebugEnabled
                {
                    get
                    {
                        return false;
                    }
                }

                public bool IsErrorEnabled
                {
                    get
                    {
                        return false;
                    }
                }

                public bool IsFatalEnabled
                {
                    get
                    {
                        return false;
                    }
                }

                public bool IsInfoEnabled
                {
                    get
                    {
                        return false;
                    }
                }

                public bool IsWarnEnabled
                {
                    get
                    {
                        return false;
                    }
                }

                public void Debug(object message)
                {
                }

                public void Debug(object message, Exception exception)
                {
                }

                public void DebugFormat(string format, params object[] args)
                {
                }

                public void Error(object message)
                {
                }

                public void Error(object message, Exception exception)
                {
                }

                public void ErrorFormat(string format, params object[] args)
                {
                }

                public void Fatal(object message)
                {
                }

                public void Fatal(object message, Exception exception)
                {
                }

                public void Info(object message)
                {
                }

                public void Info(object message, Exception exception)
                {
                }

                public void InfoFormat(string format, params object[] args)
                {
                }

                public void Warn(object message)
                {
                }

                public void Warn(object message, Exception exception)
                {
                }

                public void WarnFormat(string format, params object[] args)
                {
                }
            }
        }
    }

}
