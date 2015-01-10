using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Principal;
using SAML2.Config;
using SAML2.Schema.Core;

namespace SAML2.Identity
{
    /// <summary>
    /// <para>
    /// A specialized version of GenericIdentity that contains attributes from a SAML 2 assertion. 
    /// </para>
    /// <para>
    /// The AuthenticationType property of the Identity will be "<c>urn:oasis:names:tc:SAML:2.0:assertion</c>".
    /// </para>
    /// <para>
    /// The order of the attributes is not maintained when converting from the SAML assertion to this class. 
    /// </para>
    /// </summary>
    [Serializable]
    public class Saml20Identity : GenericIdentity, ISaml20Identity 
    {
        /// <summary>
        /// The attributes.
        /// </summary>
        private readonly Dictionary<string, List<SamlAttribute>> _attributes;

        /// <summary>
        /// Initializes a new instance of the <see cref="Saml20Identity"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="attributes">The attributes.</param>
        /// <param name="persistentPseudonym">The persistent pseudonym.</param>
        public Saml20Identity(string name, ICollection<SamlAttribute> attributes, string persistentPseudonym) 
            : base(name, Saml20Constants.Assertion)
        {
            PersistentPseudonym = persistentPseudonym;

            _attributes = new Dictionary<string, List<SamlAttribute>>();
            foreach (var att in attributes)
            {
                if (!_attributes.ContainsKey(att.Name))
                {
                    _attributes.Add(att.Name, new List<SamlAttribute>());
                }

                _attributes[att.Name].Add(att);
            }
        }

        /// <summary>
        /// Gets the value of the persistent pseudonym issued by the IdP if the Service Provider connection
        /// is set up with persistent pseudonyms. Otherwise, returns null.
        /// </summary>
        /// <value>The persistent pseudonym.</value>
        public string PersistentPseudonym { get; private set; }

        /// <summary>
        /// Retrieve an SAML 20 attribute using its name. Note that this is the value contained in the 'Name' attribute, and
        /// not the 'FriendlyName' attribute.
        /// </summary>
        /// <param name="attributeName">The attribute name.</param>
        /// <returns>List of <see cref="SamlAttribute"/>.</returns>
        /// <exception cref="KeyNotFoundException">If the identity instance does not have the requested attribute.</exception>
        public List<SamlAttribute> this[string attributeName]
        {
            get { return _attributes[attributeName]; }
        }


        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable<SamlAttribute>)this).GetEnumerator();
        }

        /// <summary>
        /// Check if the identity contains a certain attribute.
        /// </summary>
        /// <param name="attributeName">The name of the attribute to look for.</param>
        /// <returns><c>true</c> if the specified attribute name has attribute; otherwise, <c>false</c>.</returns>
        public bool HasAttribute(string attributeName)
        {
            return _attributes.ContainsKey(attributeName);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        IEnumerator<SamlAttribute> IEnumerable<SamlAttribute>.GetEnumerator()
        {
            var allAttributes = new List<SamlAttribute>();
            foreach (var name in _attributes.Keys)
            {
                allAttributes.AddRange(_attributes[name]);
            }

            return allAttributes.GetEnumerator();
        }

        /// <summary>
        /// This method converts the received SAML assertion into an <see cref="IPrincipal"/>.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="point">The point.</param>
        /// <returns>The <see cref="IPrincipal"/>.</returns>
        internal static IPrincipal InitSaml20Identity(Saml20Assertion assertion, IdentityProvider point)
        {
            var isPersistentPseudonym = assertion.Subject.Format == Saml20Constants.NameIdentifierFormats.Persistent;

            // Protocol-level support for persistent pseudonyms: If a mapper has been configured, use it here before constructing the principal.
            var subjectIdentifier = assertion.Subject.Value;
            if (isPersistentPseudonym && point.PersistentPseudonym != null)
            {
                subjectIdentifier = point.PersistentPseudonym.GetMapper().MapIdentity(assertion.Subject);
            }

            // Create identity
            var identity = new Saml20Identity(subjectIdentifier, assertion.Attributes, isPersistentPseudonym ? assertion.Subject.Value : null);                        

            return new GenericPrincipal(identity, new string[] { });
        }

        /// <summary>
        /// Adds the attribute from query.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        internal void AddAttributeFromQuery(string name, SamlAttribute value)
        {
            if (!_attributes.ContainsKey(name))
            {
                _attributes.Add(name, new List<SamlAttribute>());
            }

            if (!_attributes[name].Contains(value))
            {
                _attributes[name].Add(value);
            }
        }
    }
}
