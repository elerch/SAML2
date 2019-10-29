using System;
using System.Xml.Serialization;

namespace SAML2.Schema.Protocol
{
    /// <summary>
    /// The SAML protocol status class.
    /// </summary>
    [Serializable]
    [XmlType(Namespace = Saml20Constants.Protocol)]
    [XmlRoot(ElementName, Namespace = Saml20Constants.Protocol, IsNullable = false)]
    public class Status
    {
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public const string ElementName = "Status";

        #region Elements

        /// <summary>
        /// Gets or sets the status code.
        /// A code representing the status of the activity carried out in response to the corresponding request.
        /// </summary>
        /// <value>The status code.</value>
        [XmlElement("StatusCode", Order = 1)]
        public StatusCode StatusCode { get; set; }

        /// <summary>
        /// Gets or sets the status detail.
        /// Additional information concerning the status of the request.
        /// </summary>
        /// <value>The status detail.</value>
        [XmlElement("StatusDetail", Order = 3)]
        public StatusDetail StatusDetail { get; set; }
        
        /// <summary>
        /// Gets or sets the status message.
        /// A message which MAY be returned to an operator.
        /// </summary>
        /// <value>The status message.</value>
        [XmlElement("StatusMessage", Order = 2)]
        public string StatusMessage { get; set; }

        #endregion
    }
}
