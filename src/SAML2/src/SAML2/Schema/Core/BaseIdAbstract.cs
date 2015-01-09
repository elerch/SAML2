using System;
using System.Xml.Serialization;

namespace SAML2.Schema.Core
{
    /// <summary>
    /// The &lt;BaseID&gt; element is an extension point that allows applications to add new kinds of identifiers. Its
    /// BaseIDAbstractType complex type is abstract and is thus usable only as the base of a derived type.
    /// </summary>
    [Serializable]
    [XmlType(Namespace = Saml20Constants.Assertion)]
    [XmlRoot(ElementName, Namespace = Saml20Constants.Assertion, IsNullable = false)]
    public abstract class BaseIdAbstract
    {
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public const string ElementName = "BaseID";

        #region Attributes

        /// <summary>
        /// Gets or sets the name qualifier.
        /// The security or administrative domain that qualifies the identifier. This attribute provides a means
        /// to federate identifiers from disparate user stores without collision.
        /// </summary>
        /// <value>The name qualifier.</value>
        [XmlAttribute("NameQualifier")]
        public string NameQualifier { get; set; }

        /// <summary>
        /// Gets or sets the SP name qualifier.
        /// Further qualifies an identifier with the name of a service provider or affiliation of providers. This
        /// attribute provides an additional means to federate identifiers on the basis of the relying party or
        /// parties.
        /// </summary>
        /// <value>The SP name qualifier.</value>
        [XmlAttribute("SPNameQualifier")]
        public string SPNameQualifier { get; set; }

        #endregion
    }
}
