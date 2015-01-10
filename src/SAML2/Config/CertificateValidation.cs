using System.Configuration;

namespace SAML2.Config
{
    /// <summary>
    /// Certificate Validation configuration element.
    /// </summary>
    public class CertificateValidationElement : IConfigurationElementCollectionElement
    {

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public string Type { get; set; }

        /// <summary>
        /// Gets the element key.
        /// </summary>
        public object ElementKey
        {
            get { return Type; }
        }
    }
}
