using System;
using System.Collections.Generic;
using System.Linq;
using SAML2.Schema.Core;
using SAML2.Utils;

namespace SAML2.Validation
{
    /// <summary>
    /// SAML2 Assertion validator.
    /// </summary>
    public class Saml20AssertionValidator : ISaml20AssertionValidator
    {
        /// <summary>
        /// Use quirksMode.
        /// </summary>
        private readonly bool _quirksMode;

        /// <summary>
        /// The allowed audience URIs.
        /// </summary>
        private readonly List<string> _allowedAudienceUris;

        /// <summary>
        /// NameIDValidator backing field.
        /// </summary>
        private readonly ISaml20NameIdValidator _nameIdValidator = new Saml20NameIdValidator();

        /// <summary>
        /// StatementValidator backing field.
        /// </summary>
        private readonly ISaml20StatementValidator _statementValidator = new Saml20StatementValidator();

        /// <summary>
        /// SubjectValidator backing field.
        /// </summary>
        private readonly ISaml20SubjectValidator _subjectValidator = new Saml20SubjectValidator();

        /// <summary>
        /// Initializes a new instance of the <see cref="Saml20AssertionValidator"/> class.
        /// </summary>
        /// <param name="allowedAudienceUris">The allowed audience uris.</param>
        /// <param name="quirksMode">if set to <c>true</c> [quirks mode].</param>
        public Saml20AssertionValidator(List<string> allowedAudienceUris, bool quirksMode)
        {
            _allowedAudienceUris = allowedAudienceUris;
            _quirksMode = quirksMode;
        }

        #region ISaml20AssertionValidator interface

        /// <summary>
        /// Validates the assertion.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        public virtual void ValidateAssertion(Assertion assertion)
        {
            if (assertion == null)
            {
                throw new ArgumentNullException("assertion");
            }

            ValidateAssertionAttributes(assertion);
            ValidateSubject(assertion);
            ValidateConditions(assertion);
            ValidateStatements(assertion);
        }

        /// <summary>
        /// Validates the time restrictions.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="allowedClockSkew">The allowed clock skew.</param>
        public void ValidateTimeRestrictions(Assertion assertion, TimeSpan allowedClockSkew)
        {
            // Conditions are not required
            if (assertion.Conditions == null)
            {
                return;
            }

            var conditions = assertion.Conditions;
            var now = DateTime.UtcNow;

            // Negative allowed clock skew does not make sense - we are trying to relax the restriction interval, not restrict it any further
            if (allowedClockSkew < TimeSpan.Zero)
            {
                allowedClockSkew = allowedClockSkew.Negate();
            }

            // NotBefore must not be in the future
            if (!ValidateNotBefore(conditions.NotBefore, now, allowedClockSkew))
            {
                throw new Saml20FormatException("Conditions.NotBefore must not be in the future");
            }

            // NotOnOrAfter must not be in the past
            if (!ValidateNotOnOrAfter(conditions.NotOnOrAfter, now, allowedClockSkew))
            {
                throw new Saml20FormatException("Conditions.NotOnOrAfter must not be in the past");
            }

            foreach (var statement in assertion.GetAuthnStatements())
            {
                if (statement.SessionNotOnOrAfter != null && statement.SessionNotOnOrAfter <= now)
                {
                    throw new Saml20FormatException("AuthnStatement attribute SessionNotOnOrAfter MUST be in the future");
                }

                // TODO: Consider validating that authnStatement.AuthnInstant is in the past
            }

            if (assertion.Subject != null)
            {
                foreach (var subjectConfirmation in assertion.Subject.Items.OfType<SubjectConfirmation>().Where(subjectConfirmation => subjectConfirmation.SubjectConfirmationData != null))
                {
                    if (!ValidateNotBefore(subjectConfirmation.SubjectConfirmationData.NotBefore, now, allowedClockSkew))
                    {
                        throw new Saml20FormatException("SubjectConfirmationData.NotBefore must not be in the future");
                    }

                    if (!ValidateNotOnOrAfter(subjectConfirmation.SubjectConfirmationData.NotOnOrAfter, now, allowedClockSkew))
                    {
                        throw new Saml20FormatException("SubjectConfirmationData.NotOnOrAfter must not be in the past");
                    }
                }
            }
        }

        #endregion

        /// <summary>
        /// If both conditions.NotBefore and conditions.NotOnOrAfter are specified, NotBefore
        /// MUST BE less than NotOnOrAfter
        /// </summary>
        /// <param name="conditions">The conditions.</param>
        /// <exception cref="Saml20FormatException">If <param name="conditions"/>.NotBefore is not less than <paramref name="conditions"/>.NotOnOrAfter</exception>
        private static void ValidateConditionsInterval(Conditions conditions)
        {
            // No settings? No restrictions
            if (conditions.NotBefore == null && conditions.NotOnOrAfter == null)
            {
                return;
            }

            if (conditions.NotBefore != null && conditions.NotOnOrAfter != null && conditions.NotBefore.Value >= conditions.NotOnOrAfter.Value)
            {
                throw new Saml20FormatException(string.Format("NotBefore {0} MUST BE less than NotOnOrAfter {1} on Conditions", Saml20Utils.ToUtcString(conditions.NotBefore.Value), Saml20Utils.ToUtcString(conditions.NotOnOrAfter.Value)));
            }
        }

        /// <summary>
        /// Null fields are considered to be valid
        /// </summary>
        /// <param name="notBefore">The not before.</param>
        /// <param name="now">The now.</param>
        /// <param name="allowedClockSkew">The allowed clock skew.</param>
        /// <returns>True if the not before value is valid, else false.</returns>
        private static bool ValidateNotBefore(DateTime? notBefore, DateTime now, TimeSpan allowedClockSkew)
        {
            return notBefore == null || TimeRestrictionValidation.NotBeforeValid(notBefore.Value, now, allowedClockSkew);
        }

        /// <summary>
        /// Handle allowed clock skew by increasing notOnOrAfter with allowedClockSkew
        /// </summary>
        /// <param name="notOnOrAfter">The not on or after.</param>
        /// <param name="now">The now.</param>
        /// <param name="allowedClockSkew">The allowed clock skew.</param>
        /// <returns>True if the not on or after value is valid, else false.</returns>
        private static bool ValidateNotOnOrAfter(DateTime? notOnOrAfter, DateTime now, TimeSpan allowedClockSkew)
        {
            return notOnOrAfter == null || TimeRestrictionValidation.NotOnOrAfterValid(notOnOrAfter.Value, now, allowedClockSkew);
        }

        /// <summary>
        /// Validates that all the required attributes are present on the assertion.
        /// Furthermore it validates validity of the Issuer element.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        private void ValidateAssertionAttributes(Assertion assertion)
        {
            // There must be a Version
            if (!Saml20Utils.ValidateRequiredString(assertion.Version))
            {
                throw new Saml20FormatException("Assertion element must have the Version attribute set.");
            }

            // Version must be 2.0
            if (assertion.Version != Saml20Constants.Version)
            {
                throw new Saml20FormatException("Wrong value of version attribute on Assertion element");
            }

            // Assertion must have an ID
            if (!Saml20Utils.ValidateRequiredString(assertion.Id))
            {
                throw new Saml20FormatException("Assertion element must have the ID attribute set.");
            }

            // Make sure that the ID elements is at least 128 bits in length (SAML2.0 std section 1.3.4)
            if (!Saml20Utils.ValidateIdString(assertion.Id))
            {
                throw new Saml20FormatException("Assertion element must have an ID attribute with at least 16 characters (the equivalent of 128 bits)");
            }

            // IssueInstant must be set.
            if (!assertion.IssueInstant.HasValue)
            {
                throw new Saml20FormatException("Assertion element must have the IssueInstant attribute set.");
            }

            // There must be an Issuer
            if (assertion.Issuer == null)
            {
                throw new Saml20FormatException("Assertion element must have an issuer element.");
            }

            // The Issuer element must be valid
            _nameIdValidator.ValidateNameId(assertion.Issuer);
        }

        /// <summary>
        /// Validates the Assertion's conditions
        /// Audience restrictions processing rules are:
        /// - Within a single audience restriction condition in the assertion, the service must be configured
        /// with an audience-list that contains at least one of the restrictions in the assertion ("OR" filter)
        /// - When multiple audience restrictions are present within the same assertion, all individual audience
        /// restriction conditions must be met ("AND" filter)
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        private void ValidateConditions(Assertion assertion)
        {
            // Conditions are not required
            if (assertion.Conditions == null)
            {
                return;
            }

            var oneTimeUseSeen = false;
            var proxyRestrictionsSeen = false;
            
            ValidateConditionsInterval(assertion.Conditions);

            foreach (var cat in assertion.Conditions.Items)
            {
                if (cat is OneTimeUse)
                {
                    if (oneTimeUseSeen)
                    {
                        throw new Saml20FormatException("Assertion contained more than one condition of type OneTimeUse");
                    }

                    oneTimeUseSeen = true;
                    continue;
                }

                if (cat is ProxyRestriction)
                {
                    if (proxyRestrictionsSeen)
                    {
                        throw new Saml20FormatException("Assertion contained more than one condition of type ProxyRestriction");
                    }

                    proxyRestrictionsSeen = true;

                    var proxyRestriction = (ProxyRestriction)cat;
                    if (!string.IsNullOrEmpty(proxyRestriction.Count))
                    {
                        uint res;
                        if (!uint.TryParse(proxyRestriction.Count, out res))
                        {
                            throw new Saml20FormatException("Count attribute of ProxyRestriction MUST BE a non-negative integer");
                        }
                    }

                    if (proxyRestriction.Audience != null)
                    {
                        foreach (var audience in proxyRestriction.Audience)
                        {
                            if (!Uri.IsWellFormedUriString(audience, UriKind.Absolute))
                            {
                                throw new Saml20FormatException("ProxyRestriction Audience MUST BE a wellformed uri");
                            }
                        }
                    }
                }

                // AudienceRestriction processing goes here (section 2.5.1.4 of [SAML2.0 standard])
                if (cat is AudienceRestriction)
                {
                    // No audience restrictions? No problems...
                    var audienceRestriction = (AudienceRestriction)cat;
                    if (audienceRestriction.Audience == null || audienceRestriction.Audience.Count == 0)
                    {
                        continue;
                    }

                    // If there are no allowed audience uris configured for the service, the assertion is not
                    // valid for this service
                    if (_allowedAudienceUris == null || _allowedAudienceUris.Count < 1)
                    {
                        throw new Saml20FormatException("The service is not configured to meet any audience restrictions");
                    }

                    string match = null;
                    foreach (var audience in audienceRestriction.Audience)
                    {
                        // In QuirksMode this validation is omitted
                        if (!_quirksMode)
                        {
                            // The given audience value MUST BE a valid URI
                            if (!Uri.IsWellFormedUriString(audience, UriKind.Absolute))
                            {
                                throw new Saml20FormatException("Audience element has value which is not a wellformed absolute uri");
                            }
                        }

                        match = _allowedAudienceUris.Find(allowedUri => allowedUri.Equals(audience));
                        if (match != null)
                        {
                            break;
                        }
                    }

                    var logger = Logging.LoggerProvider.LoggerFor(GetType());
                    if (logger.IsDebugEnabled)
                    {
                        var intended = string.Join(", ", audienceRestriction.Audience.ToArray());
                        var allowed = string.Join(", ", _allowedAudienceUris.ToArray());
                        logger.DebugFormat(TraceMessages.AudienceRestrictionValidated, intended, allowed);
                    }

                    if (match == null)
                    {
                        throw new Saml20FormatException("The service is not configured to meet the given audience restrictions");
                    }
                }
            }
        }
        
        /// <summary>
        /// Validates the details of the Statements present in the assertion ([SAML2.0 standard] section 2.7)
        /// NOTE: the rules relating to the enforcement of a Subject element are handled during Subject validation
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        private void ValidateStatements(Assertion assertion)
        {
            // Statements are not required
            if (assertion.Items == null)
            {
                return;
            }

            foreach (var o in assertion.Items)
            {
                _statementValidator.ValidateStatement(o);
            }
        }

        /// <summary>
        /// Validates the subject of an Assertion
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        private void ValidateSubject(Assertion assertion)
        {
            if (assertion.Subject == null)
            {
                // If there is no statements there must be a subject
                // as specified in [SAML2.0 standard] section 2.3.3
                if (assertion.Items == null || assertion.Items.Length == 0)
                {
                    throw new Saml20FormatException("Assertion with no Statements must have a subject.");
                }

                foreach (var o in assertion.Items)
                {
                    // If any of the below types are present there must be a subject.
                    if (o is AuthnStatement || o is AuthzDecisionStatement || o is AttributeStatement)
                    {
                        throw new Saml20FormatException("AuthnStatement, AuthzDecisionStatement and AttributeStatement require a subject.");
                    }
                }
            }
            else
            {
                // If a subject is present, validate it
                _subjectValidator.ValidateSubject(assertion.Subject);
            }
        }
    }
}
