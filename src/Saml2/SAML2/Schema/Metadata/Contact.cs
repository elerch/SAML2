using System;
using System.Xml;
using System.Xml.Serialization;

namespace SAML2.Schema.Metadata
{
    /// <summary>
    /// The &lt;ContactPerson&gt; element specifies basic contact information about a person responsible in some
    /// capacity for a SAML entity or role. The use of this element is always optional. Its content is informative in
    /// nature and does not directly map to any core SAML elements or attributes.
    /// </summary>
    [Serializable]
    [XmlRoot(ElementName, IsNullable = false)]
    public class Contact
    {
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public const string ElementName = "ContactPerson";

        /// <summary>
        /// Initializes a new instance of the <see cref="Contact"/> class.
        /// </summary>
        public Contact() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Contact"/> class.
        /// </summary>
        /// <param name="contactType">Type of the contact.</param>
        public Contact(ContactType contactType)
        {
            ContactType = contactType;
        }

        #region Attributes

        /// <summary>
        /// Gets or sets XML any attribute.
        /// </summary>
        /// <value>The XML Any attribute.</value>
        [XmlAnyAttribute]
        public XmlAttribute[] AnyAttr { get; set; }

        /// <summary>
        /// Gets or sets the type of the contact.
        /// Specifies the type of contact using the ContactTypeType enumeration. The possible values are
        /// technical, support, administrative, billing, and other.
        /// </summary>
        /// <value>The type of the contact.</value>
        [XmlAttribute("contactType")]
        public ContactType ContactType { get; set; }

        #endregion

        #region Elements

        /// <summary>
        /// Gets or sets the company.
        /// Optional string element that specifies the name of the company for the contact person.
        /// </summary>
        /// <value>The company.</value>
        [XmlElement("Company", Order = 1)]
        public string Company { get; set; }

        /// <summary>
        /// Gets or sets the email address.
        /// Zero or more elements containing mailto: URIs representing e-mail addresses belonging to the
        /// contact person.
        /// </summary>
        /// <value>The email address.</value>
        [XmlElement("EmailAddress", DataType = "anyURI", Order = 4)]
        public string[] EmailAddress { get; set; }

        /// <summary>
        /// Gets or sets the extensions.
        /// This contains optional metadata extensions that are agreed upon between a metadata publisher
        /// and consumer. Extension elements MUST be namespace-qualified by a non-SAML-defined
        /// namespace.
        /// </summary>
        /// <value>The extensions.</value>
        [XmlElement("Extensions", Order = 6)]
        public ExtensionType Extensions { get; set; }

        /// <summary>
        /// Gets or sets the name of the given.
        /// Optional string element that specifies the given (first) name of the contact person.
        /// </summary>
        /// <value>The name of the given.</value>
        [XmlElement("GivenName", Order = 2)]
        public string GivenName { get; set; }

        /// <summary>
        /// Gets or sets optional string element that specifies the surname of the contact person.
        /// </summary>
        /// <value>The name of the sur.</value>
        [XmlElement("SurName", Order = 3)]
        public string SurName { get; set; }

        /// <summary>
        /// Gets or sets the telephone number.
        /// Zero or more string elements specifying a telephone number of the contact person.
        /// </summary>
        /// <value>The telephone number.</value>
        [XmlElement("TelephoneNumber", Order = 5)]
        public string[] TelephoneNumber { get; set; }

        #endregion
    }
}
