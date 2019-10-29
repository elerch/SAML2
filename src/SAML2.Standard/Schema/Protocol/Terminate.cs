using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace SAML2.Schema.Protocol
{
    /// <summary>
    /// The SAML20 protocol Terminate class
    /// </summary>
    [Serializable]
    [DebuggerStepThrough]
    [XmlType(Namespace = Saml20Constants.Protocol)]
    [XmlRoot(ElementName, Namespace = Saml20Constants.Protocol, IsNullable = false)]
    public class Terminate
    {
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public const string ElementName = "Terminate";
    }
}
