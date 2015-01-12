using System.Configuration;
using System.Security.Cryptography.X509Certificates;

namespace SAML2.AspNet.Config
{
    /// <summary>
    /// Signing Certificate configuration element.
    /// </summary>
    public class CertificateElement : WritableConfigurationElement
    {
        #region Attributes

        /// <summary>
        /// Gets or sets the find value.
        /// </summary>
        /// <value>The find value.</value>
        [ConfigurationProperty("findValue", IsRequired = true)]
        public string FindValue
        {
            get { return (string)base["findValue"]; }
            set { base["findValue"] = value; }
        }

        /// <summary>
        /// Gets or sets the store location.
        /// </summary>
        /// <value>The store location.</value>
        [ConfigurationProperty("storeLocation", IsRequired = true)]
        public StoreLocation StoreLocation
        {
            get { return (StoreLocation)base["storeLocation"]; }
            set { base["storeLocation"] = value; }
        }

        /// <summary>
        /// Gets or sets the name of the store.
        /// </summary>
        /// <value>The name of the store.</value>
        [ConfigurationProperty("storeName", IsRequired = true)]
        public StoreName StoreName
        {
            get { return (StoreName)base["storeName"]; }
            set { base["storeName"] = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to only find valid certificates.
        /// </summary>
        /// <value><c>true</c> if only valid certificates should be considered; otherwise, <c>false</c>.</value>
        [ConfigurationProperty("validOnly")]
        public bool ValidOnly
        {
            get { return (bool)base["validOnly"]; }
            set { base["validOnly"] = value; }
        }

        /// <summary>
        /// Gets or sets the X509FindType.
        /// </summary>
        /// <value>The X509FindType.</value>
        [ConfigurationProperty("x509FindType", IsRequired = true)]
        public X509FindType X509FindType
        {
            get { return (X509FindType)base["x509FindType"]; }
            set { base["x509FindType"] = value; }
        }

        #endregion

        public X509Certificate2 Certificate { get; set; }

        /// <summary>
        /// Opens the certificate from its store.
        /// </summary>
        /// <returns>The <see cref="X509Certificate2"/>.</returns>
        public X509Certificate2 GetCertificate()
        {
            if (Certificate != null) return Certificate;
            var store = new X509Store(StoreName, StoreLocation);
            try
            {
                store.Open(OpenFlags.ReadOnly);
                var found = store.Certificates.Find(X509FindType, FindValue, ValidOnly);
                if (found.Count == 0)
                {
                    throw new ConfigurationErrorsException(string.Format(ErrorMessages.CertificateNotFound, FindValue));
                }

                if (found.Count > 1)
                {
                    throw new ConfigurationErrorsException(string.Format(ErrorMessages.CertificateNotUnique, FindValue));
                }
                Certificate = found[0];
                return found[0];
            }
            finally
            {
                store.Close();
            }
        }
    }
}
