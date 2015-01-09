using System.Configuration;

namespace SAML2.Config
{
    /// <summary>
    /// Service Provider Endpoint configuration element.
    /// </summary>
    public class ServiceProviderEndpointElement : WritableConfigurationElement, IConfigurationElementCollectionElement
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
        /// Gets or sets the index.
        /// </summary>
        /// <value>The index.</value>
        [ConfigurationProperty("index")]
        public int Index
        {
            get { return (int)base["index"]; }
            set { base["index"] = value; }
        }

        /// <summary>
        /// Gets or sets the local path.
        /// </summary>
        /// <value>The local path.</value>
        [ConfigurationProperty("localPath", IsRequired = true)]
        public string LocalPath
        {
            get { return (string)base["localPath"]; }
            set { base["localPath"] = value; }
        }

        /// <summary>
        /// Gets or sets the redirect URL.
        /// </summary>
        /// <value>The redirect URL.</value>
        [ConfigurationProperty("redirectUrl")]
        public string RedirectUrl
        {
            get { return (string)base["redirectUrl"]; }
            set { base["redirectUrl"] = value; }
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
