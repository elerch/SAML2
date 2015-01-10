using System.Configuration;

namespace SAML2.Config
{
    /// <summary>
    /// Logging configuration element.
    /// </summary>
    public class Logging 
    {
        /// <summary>
        /// Gets or sets the logging factory.
        /// </summary>
        /// <value>The logging factory.</value>
        public string LoggingFactory { get; set; }
    }
}
