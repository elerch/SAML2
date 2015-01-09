using System;
using System.Xml.Serialization;

namespace SAML2.Schema.Protocol
{
    /// <summary>
    /// The <c>&lt;NameIDPolicy&gt;</c> element tailors the name identifier in the subjects of assertions resulting from an
    /// <c>&lt;AuthnRequest&gt;</c>.
    /// </summary>
    [Serializable]
    [XmlType(Namespace = Saml20Constants.Protocol)]
    [XmlRoot(ElementName, Namespace = Saml20Constants.Protocol, IsNullable = false)]
    public class NameIdPolicy
    {
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public const string ElementName = "NameIDPolicy";

        /// <summary>
        /// Gets or sets a value indicating whether [allow create].
        /// A Boolean value used to indicate whether the identity provider is allowed, in the course of fulfilling the
        /// request, to create a new identifier to represent the principal. Defaults to "false". When "false", the
        /// requester constrains the identity provider to only issue an assertion to it if an acceptable identifier for
        /// the principal has already been established. Note that this does not prevent the identity provider from
        /// creating such identifiers outside the context of this specific request (for example, in advance for a
        /// large number of principals).
        /// </summary>
        /// <value><c>true</c> if [allow create]; otherwise, <c>false</c>.</value>
        [XmlIgnore]
        public bool? AllowCreate { get; set; }

        #region Attributes

        /// <summary>
        /// Gets or sets the AllowCreate string.
        /// </summary>
        /// <value>The AllowCreate string.</value>
        [XmlAttribute("AllowCreate")]
        public string AllowCreateString
        {
            get { return AllowCreate.HasValue ? AllowCreate.ToString() : null; }
            set { AllowCreate = string.IsNullOrEmpty(value) ? (bool?)null : Convert.ToBoolean(value); }
        }

        /// <summary>
        /// Gets or sets the format.
        /// Specifies the URI reference corresponding to a name identifier format defined in this or another
        /// specification (see Section 8.3 for examples). The additional value of
        /// <c>urn:oasis:names:tc:SAML:2.0:nameid-format:encrypted</c> is defined specifically for use
        /// within this attribute to indicate a request that the resulting identifier be encrypted.
        /// </summary>
        /// <value>The format.</value>
        [XmlAttribute("Format", DataType = "anyURI")]
        public string Format { get; set; }
        
        /// <summary>
        /// Gets or sets the SP name qualifier.
        /// Optionally specifies that the assertion subject's identifier be returned (or created) in the namespace of
        /// a service provider other than the requester, or in the namespace of an affiliation group of service
        /// providers.
        /// </summary>
        /// <value>The SP name qualifier.</value>
        [XmlAttribute("SPNameQualifier")]
        public string SPNameQualifier { get; set; }

        #endregion
    }
}
