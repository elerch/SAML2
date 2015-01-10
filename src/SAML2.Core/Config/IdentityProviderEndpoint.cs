using System.Configuration;

namespace SAML2.Config
{
    /// <summary>
    /// Identity Provider Endpoint configuration element.
    /// </summary>
    public class IdentityProviderEndpoint
    {
        /// <summary>
        /// Gets or sets the binding.
        /// </summary>
        /// <value>The binding.</value>
        public BindingType Binding { get; set; }

        /// <summary>
        /// Gets or sets the protocol binding to force.
        /// </summary>
        /// <value>The force protocol binding.</value>
        public string ForceProtocolBinding { get; set; }

        /// <summary>
        /// Gets or sets a value indicating a caller to access the xml representation of an assertion before it's 
        /// translated to a strongly typed instance
        /// </summary>
        /// <value>The token accessor.</value>
        public string TokenAccessor { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public EndpointType Type { get; set; }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>The URL.</value>
        public string Url { get; set; }

    }
}
