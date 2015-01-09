using System;
using System.Xml;
using System.Xml.Serialization;

namespace SAML2.Schema.XmlDSig
{
    /// <summary>
    /// <para>
    /// CanonicalizationMethod is a required element that specifies the canonicalization algorithm applied to 
    /// the SignedInfo element prior to performing signature calculations. This element uses the general structure 
    /// for algorithms described in Algorithm Identifiers and Implementation Requirements (section 6.1). 
    /// Implementations MUST support the REQUIRED canonicalization algorithms. 
    /// </para>
    /// <para>
    /// Alternatives to the REQUIRED canonicalization algorithms (section 6.5), such as Canonical XML with 
    /// Comments (section 6.5.1) or a minimal canonicalization (such as CRLF and charset normalization), may be 
    /// explicitly specified but are NOT REQUIRED. Consequently, their use may not interoperate with other 
    /// applications that do not support the specified algorithm (see XML Canonicalization and Syntax Constraint 
    /// Considerations, section 7). Security issues may also arise in the treatment of entity processing and 
    /// comments if non-XML aware canonicalization algorithms are not properly constrained (see section 8.2: Only 
    /// What is "Seen" Should be Signed). 
    /// </para>
    /// <para>
    /// The way in which the SignedInfo element is presented to the canonicalization method is dependent on that 
    /// method. The following applies to algorithms which process XML as nodes or characters: 
    /// </para>
    /// <para>
    /// XML based canonicalization implementations MUST be provided with a [XPath] node-set originally formed from 
    /// the document containing the SignedInfo and currently indicating the SignedInfo, its descendants, and 
    /// the attribute and namespace nodes of SignedInfo and its descendant elements. 
    /// Text based canonicalization algorithms (such as CRLF and charset normalization) should be provided with 
    /// the UTF-8 octets that represent the well-formed SignedInfo element, from the first character to the last 
    /// character of the XML representation, inclusive. This includes the entire text of the start and end tags 
    /// of the SignedInfo element as well as all descendant markup and character data (i.e., the text) between 
    /// those tags. Use of text based canonicalization of SignedInfo is NOT RECOMMENDED. 
    /// </para>
    /// <para>
    /// We recommend applications that implement a text-based instead of XML-based canonicalization -- such as 
    /// resource constrained apps -- generate canonicalized XML as their output serialization so as to mitigate 
    /// interoperability and security concerns. For instance, such an implementation SHOULD (at least) generate 
    /// standalone XML instances [XML]. 
    /// </para>
    /// <para>
    /// NOTE: The signature application must exercise great care in accepting and executing an arbitrary 
    /// CanonicalizationMethod. For example, the canonicalization method could rewrite the URIs of the References 
    /// being validated. Or, the method could massively transform SignedInfo so that validation would always 
    /// succeed (i.e., converting it to a trivial signature with a known key over trivial data). Since 
    /// CanonicalizationMethod is inside SignedInfo, in the resulting canonical form it could erase itself from 
    /// SignedInfo or modify the SignedInfo element so that it appears that a different canonicalization function 
    /// was used! Thus a Signature which appears to authenticate the desired data with the desired key, 
    /// DigestMethod, and SignatureMethod, can be meaningless if a capricious CanonicalizationMethod is used. 
    /// </para>
    /// </summary>
    [Serializable]
    [XmlType(Namespace = Saml20Constants.Xmldsig)]
    [XmlRoot(ElementName, Namespace = Saml20Constants.Xmldsig, IsNullable = false)]
    public class CanonicalizationMethod
    {
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public const string ElementName = "CanonicalizationMethod";

        #region Attributes

        /// <summary>
        /// Gets or sets the algorithm.
        /// </summary>
        /// <value>The algorithm.</value>
        [XmlAttribute("Algorithm", DataType = "anyURI")]
        public string Algorithm { get; set; }

        #endregion
        
        #region Elements

        /// <summary>
        /// Gets or sets the any XML element.
        /// </summary>
        /// <value>The Any XML element.</value>
        [XmlText]
        [XmlAnyElement]
        public XmlNode[] Any { get; set; }

        #endregion
    }
}
