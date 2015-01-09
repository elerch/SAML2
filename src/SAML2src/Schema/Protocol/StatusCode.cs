using System;
using System.Xml.Serialization;

namespace SAML2.Schema.Protocol
{
    /// <summary>
    /// The &lt;StatusCode&gt; element specifies a code or a set of nested codes representing the status of the
    /// corresponding request.
    /// </summary>
    [Serializable]
    [XmlType(Namespace = Saml20Constants.Protocol)]
    [XmlRoot(ElementName, Namespace = Saml20Constants.Protocol, IsNullable = false)]
    public class StatusCode
    {
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public const string ElementName = "StatusCode";

        #region Attributes

        /// <summary>
        /// Gets or sets the value.
        /// The status code value. This attribute contains a URI reference. The value of the topmost
        /// &lt;StatusCode&gt; element MUST be from the top-level list provided in this section.
        /// </summary>
        /// <value>The value.</value>
        [XmlAttribute("Value", DataType = "anyURI")]
        public string Value { get; set; }

        #endregion

        #region Elements

        /// <summary>
        /// Gets or sets the sub status code.
        /// A subordinate status code that provides more specific information on an error condition. Note that
        /// responders MAY omit subordinate status codes in order to prevent attacks that seek to probe for
        /// additional information by intentionally presenting erroneous requests.
        /// </summary>
        /// <value>The sub status code.</value>
        [XmlElement("StatusCode", Namespace = Saml20Constants.Protocol, Order = 1)]
        public StatusCode SubStatusCode { get; set; }

        #endregion
    }
}
