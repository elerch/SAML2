using System;
using System.Xml.Serialization;

namespace SAML2.Schema.Core
{
    /// <summary>
    /// The &lt;Action&gt; element specifies an action on the specified resource for which permission is sought. Its
    /// string-data content provides the label for an action sought to be performed on the specified resource,
    /// </summary>
    [Serializable]
    [XmlType(Namespace = Saml20Constants.Assertion)]
    [XmlRoot(ElementName, Namespace = Saml20Constants.Assertion, IsNullable = false)]
    public class Action
    {
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public const string ElementName = "Action";

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        [XmlText]
        public string Value { get; set; }

        #region Attributes

        /// <summary>
        /// Gets or sets the namespace.
        /// A URI reference representing the namespace in which the name of the specified action is to be
        /// interpreted. If this element is absent, the namespace
        /// <c>urn:oasis:names:tc:SAML:1.0:action:rwedc-negation</c> specified in Section 8.1.2 is in
        /// effect.
        /// </summary>
        /// <value>The namespace.</value>
        [XmlAttribute("Namespace", DataType = "anyURI")]
        public string Namespace { get; set; }

        #endregion
    }
}
