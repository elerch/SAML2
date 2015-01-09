using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using SAML2.Config;

namespace SAML2
{
    /// <summary>
    /// An HTTP module that will scan the SAML2 configuration for metadata endpoints on application start and fetch
    /// identity provider metadata files.
    /// </summary>
    public class Saml20MetadataFetcherModule : IHttpModule
    {
        #region Static privates

        /// <summary>
        /// The application started
        /// </summary>
        private static volatile bool _applicationStarted;

        /// <summary>
        /// The application start lock
        /// </summary>
        private static object _applicationStartLock = new object();

        #endregion

        #region IHttpModule Members

        /// <summary>
        /// Disposes of the resources (other than memory) used by the module that implements <see cref="T:System.Web.IHttpModule" />.
        /// </summary>
        public void Dispose() { }

        /// <summary>
        /// Initializes a module and prepares it to handle requests.
        /// </summary>
        /// <param name="context">An <see cref="T:System.Web.HttpApplication" /> that provides access to the methods, properties, and events common to all application objects within an ASP.NET application</param>
        public void Init(HttpApplication context)
        {
            if (!_applicationStarted)
            {
                lock (_applicationStartLock)
                {
                    if (!_applicationStarted)
                    {
                        // this will run only once per application start
                        OnStart(context);
                        _applicationStarted = true;
                    }
                }
            }
        }

        #endregion

        /// <summary>
        /// Called when Application starts.
        /// </summary>
        /// <param name="context">The context.</param>
        public void OnStart(HttpApplication context)
        {
            var logger = Logging.LoggerProvider.LoggerFor(GetType());
            logger.Debug("Attempting to fetch SAML metadata files");

            var config = Saml2Config.GetConfig();

            var metadataLocation = config.IdentityProviders.MetadataLocation;
            var identityProviders = config.IdentityProviders;

            if (!Directory.Exists(metadataLocation))
            {
                throw new DirectoryNotFoundException("Metadata directory does not exist: " + metadataLocation);
            }

            // Get new metadata files
            foreach (var identityProvider in identityProviders)
            {
                logger.DebugFormat("Attempting to fetch SAML metadata file for identity provider {0}", identityProvider.Id);
                var metadataEndpoint = identityProvider.Endpoints.FirstOrDefault(x => x.Type == EndpointType.Metadata);
                if (metadataEndpoint == null)
                {
                    continue;
                }

                var metadataEndpointUrl = metadataEndpoint.Url;
                var metadataFile = Path.Combine(metadataLocation, identityProvider.Id + ".xml");

                // Fetch new file
                try
                {
                    var client = new WebClient();
                    client.DownloadFile(metadataEndpointUrl, metadataFile + ".new");

                    // Wipe old file
                    if (File.Exists(metadataFile))
                    {
                        File.Delete(metadataFile);
                    }

                    // Move new file into place
                    File.Move(metadataFile + ".new", metadataFile);
                    logger.DebugFormat("Successfully updated SAML metadata file for identity provider {0}", identityProvider.Id);
                }
                catch (WebException ex)
                {
                    logger.Warn(string.Format("Unable to fetch SAML metadata file for identity provider {0}", identityProvider.Id), ex);                    
                }
            }
        }
    }
}
