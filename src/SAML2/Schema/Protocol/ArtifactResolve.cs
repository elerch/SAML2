using System;
using System.Xml.Serialization;

namespace SAML2.Schema.Protocol
{
    /// <summary>
    /// The &lt;ArtifactResolve&gt; message is used to request that a SAML protocol message be returned in an
    /// &lt;ArtifactResponse&gt; message by specifying an artifact that represents the SAML protocol message.
    /// The original transmission of the artifact is governed by the specific protocol binding that is being used; see
    /// [SAMLBind] for more information on the use of artifacts in bindings
    /// </summary>
    [Serializable]
    [XmlType(Namespace = Saml20Constants.Protocol)]
    [XmlRoot(ElementName, Namespace = Saml20Constants.Protocol, IsNullable = false)]
    public class ArtifactResolve : RequestAbstract
    {
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public const string ElementName = "ArtifactResolve";

        #region Elements

        /// <summary>
        /// Gets or sets the artifact.
        /// The artifact value that the requester received and now wishes to translate into the protocol message it
        /// represents. See [SAMLBind] for specific artifact format information.
        /// </summary>
        /// <value>The artifact.</value>
        [XmlElement("Artifact", Order = 1)]
        public string Artifact { get; set; }

        #endregion
    }
}
