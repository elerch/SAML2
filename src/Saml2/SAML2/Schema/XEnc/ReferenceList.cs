using System;
using System.Xml.Serialization;

namespace SAML2.Schema.XEnc
{
    /// <summary>
    /// ReferenceList is an element that contains pointers from a key value of an EncryptedKey to items 
    /// encrypted by that key value (EncryptedData or EncryptedKey elements).
    /// </summary>
    [Serializable]
    [XmlType(AnonymousType = true, Namespace = Saml20Constants.Xenc)]
    [XmlRoot(Namespace = Saml20Constants.Xenc, IsNullable = false)]
    public class ReferenceList
    {
        #region Elements
        
        /// <summary>
        /// Gets or sets the items.
        /// <c>DataReferencee</c> and <c>KeyReference</c> elements
        /// </summary>
        /// <value>The items.</value>
        [XmlElement("DataReference", typeof(ReferenceType))]
        [XmlElement("KeyReference", typeof(ReferenceType))]
        [XmlChoiceIdentifier("ItemsElementName")]
        public ReferenceType[] Items { get; set; }

        /// <summary>
        /// Gets or sets the name of the items element.
        /// </summary>
        /// <value>The name of the items element.</value>
        [XmlElement("ItemsElementName")]
        [XmlIgnore]
        public ReferenceListType[] ItemsElementName { get; set; }

        #endregion
    }
}
