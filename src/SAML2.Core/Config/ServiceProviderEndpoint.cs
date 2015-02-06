using System.Configuration;

namespace SAML2.Config
{
    /// <summary>
    /// Service Provider Endpoint configuration element.
    /// </summary>
    public class ServiceProviderEndpoint
    {
        public ServiceProviderEndpoint() { }
        public ServiceProviderEndpoint(EndpointType type, string localPath, string redirectUrl = null, BindingType bindingType = BindingType.NotSet) : this()
        {
            Type = type;
            LocalPath = localPath;
            RedirectUrl = redirectUrl;
            Binding = bindingType;
        }

        /// <summary>
        /// Gets or sets the binding.
        /// </summary>
        /// <value>The binding.</value>
        public BindingType Binding { get; set; }

        /// <summary>
        /// Gets or sets the index.
        /// </summary>
        /// <value>The index.</value>
        public int Index { get; set; }

        /// <summary>
        /// Gets or sets the local path.
        /// </summary>
        /// <value>The local path.</value>
        public string LocalPath { get; set; }

        /// <summary>
        /// Gets or sets the redirect URL.
        /// </summary>
        /// <value>The redirect URL.</value>
        public string RedirectUrl { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public EndpointType Type { get; set; }

    }
}
