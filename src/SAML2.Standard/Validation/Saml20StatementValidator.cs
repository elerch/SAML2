using System;
using SAML2.Schema.Core;
using SAML2.Schema.Protocol;
using SAML2.Utils;

namespace SAML2.Validation
{
    /// <summary>
    /// SAML2 Statement validator.
    /// </summary>
    public class Saml20StatementValidator : ISaml20StatementValidator
    {
        /// <summary>
        /// The attribute validator.
        /// </summary>
        private readonly ISaml20AttributeValidator _attributeValidator = new Saml20AttributeValidator();

        /// <summary>
        /// Validates the statement.
        /// </summary>
        /// <param name="statement">The statement.</param>
        public virtual void ValidateStatement(StatementAbstract statement)
        {
            if (statement == null)
            {
                throw new ArgumentNullException("statement");
            }

            // Validate all possible statements in the assertion
            if (statement is AuthnStatement)
            {
                ValidateAuthnStatement((AuthnStatement)statement);
            }
            else if (statement is AuthzDecisionStatement)
            {
                ValidateAuthzDecisionStatement((AuthzDecisionStatement)statement);
            }
            else if (statement is AttributeStatement)
            {
                ValidateAttributeStatement((AttributeStatement)statement);
            }
            else
            {
                throw new Saml20FormatException(string.Format("Unsupported Statement type {0}", statement.GetType()));
            }
        }

        /// <summary>
        /// Validate AttributeStatement.
        /// </summary>
        /// <remarks>
        /// [SAML2.0 standard] section 2.7.3
        /// </remarks>
        /// <param name="statement">The statement.</param>
        private void ValidateAttributeStatement(AttributeStatement statement)
        {
            if (statement.Items == null || statement.Items.Length == 0)
            {
                throw new Saml20FormatException("AttributeStatement MUST contain at least one Attribute or EncryptedAttribute");
            }

            foreach (var o in statement.Items)
            {
                if (o == null)
                {
                    throw new Saml20FormatException("null-Attributes are not supported");
                }

                if (o is SamlAttribute)
                {
                    _attributeValidator.ValidateAttribute((SamlAttribute)o);
                }
                else if (o is EncryptedElement)
                {
                    _attributeValidator.ValidateEncryptedAttribute((EncryptedElement)o);
                }
                else
                {
                    throw new Saml20FormatException(string.Format("Subelement {0} of AttributeStatement is not supported", o.GetType()));
                }
            }
        }
        
        /// <summary>
        /// Validate <c>AuthnStatement</c>.
        /// </summary>
        /// <remarks>
        /// [SAML2.0 standard] section 2.7.2
        /// </remarks>
        /// <param name="statement">The statement.</param>
        private void ValidateAuthnStatement(AuthnStatement statement)
        {
            if (statement.AuthnInstant == null)
            {
                throw new Saml20FormatException("AuthnStatement MUST have an AuthnInstant attribute");
            }

            if (!Saml20Utils.ValidateOptionalString(statement.SessionIndex))
            {
                throw new Saml20FormatException("SessionIndex attribute of AuthnStatement must contain at least one non-whitespace character");
            }

            if (statement.SubjectLocality != null)
            {
                if (!Saml20Utils.ValidateOptionalString(statement.SubjectLocality.Address))
                {
                    throw new Saml20FormatException("Address attribute of SubjectLocality must contain at least one non-whitespace character");
                }

                if (!Saml20Utils.ValidateOptionalString(statement.SubjectLocality.DNSName))
                {
                    throw new Saml20FormatException("DNSName attribute of SubjectLocality must contain at least one non-whitespace character");
                }
            }

            ValidateAuthnContext(statement.AuthnContext);
        }

        /// <summary>
        /// Validate <c>AuthzContext</c>.
        /// </summary>
        /// <remarks>
        /// [SAML2.0 standard] section 2.7.2.2
        /// </remarks>
        /// <param name="authnContext">The authentication context.</param>
        private void ValidateAuthnContext(AuthnContext authnContext)
        {
            if (authnContext == null)
            {
                throw new Saml20FormatException("AuthnStatement MUST have an AuthnContext element");
            }

            // There must be at least one item if an authentication statement is present
            if (authnContext.Items == null || authnContext.Items.Length == 0)
            {
                throw new Saml20FormatException("AuthnContext element MUST contain at least one AuthnContextClassRef, AuthnContextDecl or AuthnContextDeclRef element");
            }

            // Cannot happen when using .NET auto-generated serializer classes, but other implementations may fail to enforce the 
            // correspondence on the size of the arrays involved
            if (authnContext.Items.Length != authnContext.ItemsElementName.Length)
            {
                throw new Saml20FormatException("AuthnContext parse error: Mismathing Items vs ItemElementNames counts");
            }

            // Validate the anyUri xsi schema type demands on context reference types
            // We do not support by-value authentication types (since Geneva does not allow it)
            if (authnContext.Items.Length > 2)
            {
                throw new Saml20FormatException("AuthnContext MUST NOT contain more than two elements.");
            }

            var authnContextDeclRefFound = false;
            for (var i = 0; i < authnContext.ItemsElementName.Length; ++i)
            {
                switch (authnContext.ItemsElementName[i])
                {
                    case Schema.Core.AuthnContextType.AuthnContextClassRef:
                        if (i > 0)
                        {
                            throw new Saml20FormatException("AuthnContextClassRef must be in the first element");
                        }

                        if (!Uri.IsWellFormedUriString((string)authnContext.Items[i], UriKind.Absolute))
                        {
                            throw new Saml20FormatException("AuthnContextClassRef has a value which is not a wellformed absolute uri");
                        }

                        break;
                    case Schema.Core.AuthnContextType.AuthnContextDeclRef:
                        if (authnContextDeclRefFound)
                        {
                            throw new Saml20FormatException("AuthnContextDeclRef MUST only be present once.");
                        }

                        authnContextDeclRefFound = true;

                        // There is some concern about this being a valid check.
                        // See: https://lists.oasis-open.org/archives/security-services/200703/msg00004.html
                        // http://saml2.codeplex.com/SourceControl/network/forks/etlerch/saml2/contribution/5740
                        if (!Uri.IsWellFormedUriString((string)authnContext.Items[i], UriKind.Absolute)) {
                            throw new Saml20FormatException("AuthnContextDeclRef has a value which is not a wellformed absolute uri");
                        }

                    break;
                    case Schema.Core.AuthnContextType.AuthnContextDecl:
                        throw new Saml20FormatException("AuthnContextDecl elements are not allowed in this implementation");
                    default:
                        throw new Saml20FormatException(string.Format("Subelement {0} of AuthnContext is not supported", authnContext.ItemsElementName[i]));
                }
            }

            // No authenticating authorities? We are done
            if (authnContext.AuthenticatingAuthority == null || authnContext.AuthenticatingAuthority.Length == 0)
            {
                return;
            }

            // Values MUST have xsi schema type anyUri:
            foreach (var authnAuthority in authnContext.AuthenticatingAuthority)
            {
                if (!Uri.IsWellFormedUriString(authnAuthority, UriKind.Absolute))
                {
                    throw new Saml20FormatException("AuthenticatingAuthority array contains a value which is not a wellformed absolute uri");
                }
            }
        }

        /// <summary>
        /// Validate <c>AuthzDecisionStatement</c>.
        /// </summary>
        /// <remarks>
        /// [SAML2.0 standard] section 2.7.4
        /// </remarks>
        /// <param name="statement">The statement.</param>
        private void ValidateAuthzDecisionStatement(AuthzDecisionStatement statement)
        {
            // This has type anyURI, and can be empty (special case in the standard), but not null.
            if (statement.Resource == null)
            {
                throw new Saml20FormatException("Resource attribute of AuthzDecisionStatement is REQUIRED");
            }

            // If it is not empty, it MUST BE a valid URI
            if (statement.Resource.Length > 0 && !Uri.IsWellFormedUriString(statement.Resource, UriKind.Absolute))
            {
                throw new Saml20FormatException("Resource attribute of AuthzDecisionStatement has a value which is not a wellformed absolute uri");
            }

            // NOTE: Decision property validation is done implicitly be the deserializer since it is represented by an enumeration
            if (statement.Action == null || statement.Action.Length == 0)
            {
                throw new Saml20FormatException("At least one Action subelement must be present for an AuthzDecisionStatement element");
            }

            foreach (var action in statement.Action)
            {
                // NOTE: [SAML2.0 standard] claims that the Namespace is [Optional], but according to the schema definition (and Geneva)
                // NOTE: it has use="required"
                if (!Saml20Utils.ValidateRequiredString(action.Namespace))
                {
                    throw new Saml20FormatException("Namespace attribute of Action element must contain at least one non-whitespace character");
                }

                if (!Uri.IsWellFormedUriString(action.Namespace, UriKind.Absolute))
                {
                    throw new Saml20FormatException("Namespace attribute of Action element has a value which is not a wellformed absolute uri");
                }
            }
        }
    }
}
