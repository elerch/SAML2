using System;
using System.Xml;
using SAML2.Utils;

namespace SAML2.Validation
{
    /// <summary>
    /// SAML2 Xml AnyAttribute validator.
    /// </summary>
    public class Saml20XmlAnyAttributeValidator
    {
        /// <summary>
        /// Validates the XML any attributes.
        /// </summary>
        /// <param name="anyAttributes">Any attributes.</param>
        public void ValidateXmlAnyAttributes(XmlAttribute[] anyAttributes)
        {
            if (anyAttributes == null)
            {
                throw new ArgumentNullException("anyAttributes");
            }

            if (anyAttributes.Length == 0)
            {
                return;
            }

            foreach (var attr in anyAttributes)
            {
                if (!Saml20Utils.ValidateRequiredString(attr.Prefix))
                {
                    throw new Saml20FormatException("Attribute extension xml attributes MUST BE namespace qualified");
                }

                foreach (var samlns in Saml20Constants.SamlNamespaces)
                {
                    if (attr.NamespaceURI == samlns)
                    {
                        throw new Saml20FormatException("Attribute extension xml attributes MUST NOT use a namespace reserved by SAML");
                    }
                }
            }
        }
    }
}
