using System.Configuration;

namespace SAML2.Config
{
    /// <summary>
    /// Identity Provider Endpoint configuration element.
    /// </summary>
    public class IdentityProviderEndpointElement : WritableConfigurationElement, IConfigurationElementCollectionElement
    {
        #region Attributes

        /// <summary>
        /// Gets or sets the binding.
        /// </summary>
        /// <value>The binding.</value>
        [ConfigurationProperty("binding")]
        public BindingType Binding
        {
            get { return (BindingType)base["binding"]; }
            set { base["binding"] = value; }
        }

        /// <summary>
        /// Gets or sets the protocol binding to force.
        /// </summary>
        /// <value>The force protocol binding.</value>
        [ConfigurationProperty("forceProtocolBinding")]
        public string ForceProtocolBinding
        {
            get { return (string)base["forceProtocolBinding"]; }
            set { base["forceProtocolBinding"] = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating a caller to access the xml representation of an assertion before it's 
        /// translated to a strongly typed instance
        /// </summary>
        /// <value>The token accessor.</value>
        [ConfigurationProperty("tokenAccessor")]
        public string TokenAccessor
        {
            get { return (string)base["tokenAccessor"]; }
            set { base["tokenAccessor"] = value; }
        }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        [ConfigurationProperty("type", IsKey = true, IsRequired = true)]
        public EndpointType Type
        {
            get { return (EndpointType)base["type"]; }
            set { base["type"] = value; }
        }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>The URL.</value>
        [ConfigurationProperty("url", IsRequired = true)]
        public string Url
        {
            get { return (string)base["url"]; }
            set { base["url"] = value; }
        }

        #endregion

        #region Implementation of IConfigurationElementCollectionElement

        /// <summary>
        /// Gets the element key.
        /// </summary>
        public object ElementKey
        {
            get { return Type; }
        }

        #endregion
    }
}
