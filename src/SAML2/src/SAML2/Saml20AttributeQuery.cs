using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Web;
using System.Xml;
using SAML2.Bindings;
using SAML2.Config;
using SAML2.Identity;
using SAML2.Logging;
using SAML2.Protocol;
using SAML2.Schema.Core;
using SAML2.Schema.Protocol;
using SAML2.Utils;

namespace SAML2
{
    /// <summary>
    /// Performs SAML2.0 attribute queries
    /// </summary>
    public class Saml20AttributeQuery
    {
        /// <summary>
        /// Logger instance.
        /// </summary>
        protected static readonly IInternalLogger Logger = LoggerProvider.LoggerFor(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// List of attributes.
        /// </summary>
        private readonly List<SamlAttribute> _attributes;

        /// <summary>
        /// The attribute query.
        /// </summary>
        private readonly AttributeQuery _attrQuery;

        /// <summary>
        /// Prevents a default instance of the <see cref="Saml20AttributeQuery"/> class from being created.
        /// </summary>
        private Saml20AttributeQuery()
        {
            _attrQuery = new AttributeQuery
                             {
                                 Version = Saml20Constants.Version,
                                 Id = "id" + Guid.NewGuid().ToString("N"),
                                 Issuer = new NameId(),
                                 IssueInstant = DateTime.Now,
                                 Subject = new Subject()
                             };
            _attributes = new List<SamlAttribute>();
        }

        /// <summary>
        /// Gets the ID of the attribute query.
        /// </summary>
        /// <value>The ID.</value>
        public string Id
        {
            get { return _attrQuery.Id; }
        }

        /// <summary>
        /// Gets or sets the issuer of the attribute query.
        /// </summary>
        /// <value>The issuer.</value>
        public string Issuer
        {
            get { return _attrQuery.Issuer.Value; }
            set { _attrQuery.Issuer.Value = value; }
        }

        /// <summary>
        /// Gets a default instance of this class with meaningful default values set.
        /// </summary>
        /// <returns>The default <see cref="Saml20AttributeQuery"/>.</returns>
        public static Saml20AttributeQuery GetDefault()
        {
            var config = Saml2Config.GetConfig();
            var result = new Saml20AttributeQuery { Issuer = config.ServiceProvider.Id };

            return result;
        }

        /// <summary>
        /// Adds an attribute to be queried using basic name format.
        /// </summary>
        /// <param name="attrName">Name of the attribute.</param>
        public void AddAttribute(string attrName)
        {
            AddAttribute(attrName, Saml20NameFormat.Basic);
        }

        /// <summary>
        /// Adds an attribute by name using the specified name format.
        /// </summary>
        /// <param name="attrName">Name of the attribute.</param>
        /// <param name="nameFormat">The name format of the attribute.</param>
        public void AddAttribute(string attrName, Saml20NameFormat nameFormat)
        {
            if (_attributes.Any(at => at.Name == attrName && at.NameFormat == GetNameFormat(nameFormat)))
            {
                throw new InvalidOperationException(string.Format("An attribute with name \"{0}\" and name format \"{1}\" has already been added", attrName, Enum.GetName(typeof(Saml20NameFormat), nameFormat)));
            }

            var attr = new SamlAttribute
                           {
                               Name = attrName,
                               NameFormat = GetNameFormat(nameFormat)
                           };

            _attributes.Add(attr);
        }

        /// <summary>
        /// Performs the attribute query and adds the resulting attributes to <c>Saml20Identity.Current</c>.
        /// </summary>
        /// <param name="context">The http context.</param>
        public void PerformQuery(HttpContext context)
        {
            var config = Saml2Config.GetConfig();

            var endpointId = (string)context.Session[Saml20AbstractEndpointHandler.IdpLoginSessionKey];
            if (string.IsNullOrEmpty(endpointId))
            {
                Logger.Error(ErrorMessages.AttrQueryNoLogin);
                throw new InvalidOperationException(ErrorMessages.AttrQueryNoLogin);
            }

            var ep = config.IdentityProviders.FirstOrDefault(x => x.Id == endpointId);
            if (ep == null)
            {
                throw new Saml20Exception(string.Format(ErrorMessages.UnknownIdentityProvider, endpointId));
            }

            PerformQuery(context, ep);
        }

        /// <summary>
        /// Performs the attribute query against the specified IdP endpoint and adds the resulting attributes to <c>Saml20Identity.Current</c>.
        /// </summary>
        /// <param name="context">The http context.</param>
        /// <param name="endPoint">The IdP to perform the query against.</param>
        public void PerformQuery(HttpContext context, IdentityProviderElement endPoint)
        {
            var nameIdFormat = (string)context.Session[Saml20AbstractEndpointHandler.IdpNameIdFormat];
            if (string.IsNullOrEmpty(nameIdFormat))
            {
                nameIdFormat = Saml20Constants.NameIdentifierFormats.Persistent;
            }

            PerformQuery(context, endPoint, nameIdFormat);
        }

        /// <summary>
        /// Performs the attribute query against the specified IdP endpoint and adds the resulting attributes to <c>Saml20Identity.Current</c>.
        /// </summary>
        /// <param name="context">The http context.</param>
        /// <param name="endPoint">The IdP to perform the query against.</param>
        /// <param name="nameIdFormat">The name id format.</param>
        public void PerformQuery(HttpContext context, IdentityProviderElement endPoint, string nameIdFormat)
        {
            Logger.DebugFormat("{0}.{1} called", GetType(), "PerformQuery()");

            var builder = new HttpSoapBindingBuilder(context);

            var name = new NameId
                           {
                               Value = Saml20Identity.Current.Name,
                               Format = nameIdFormat
                           };
            _attrQuery.Subject.Items = new object[] { name };
            _attrQuery.SamlAttribute = _attributes.ToArray();

            var query = new XmlDocument();
            query.LoadXml(Serialization.SerializeToXmlString(_attrQuery));

            XmlSignatureUtils.SignDocument(query, Id);
            if (query.FirstChild is XmlDeclaration)
            {
                query.RemoveChild(query.FirstChild);
            }

            Logger.DebugFormat(TraceMessages.AttrQuerySent, endPoint.Metadata.GetAttributeQueryEndpointLocation(), query.OuterXml);

            Stream s;
            try
            {
                 s = builder.GetResponse(endPoint.Metadata.GetAttributeQueryEndpointLocation(), query.OuterXml, endPoint.AttributeQuery);
            }
            catch (Exception e)
            {
                Logger.Error(e.Message, e);
                throw;
            }

            var parser = new HttpSoapBindingParser(s);
            var status = parser.GetStatus();
            if (status.StatusCode.Value != Saml20Constants.StatusCodes.Success)
            {
                Logger.ErrorFormat(ErrorMessages.AttrQueryStatusNotSuccessful, Serialization.SerializeToXmlString(status));
                throw new Saml20Exception(status.StatusMessage);
            }

            bool isEncrypted;
            var xmlAssertion = Saml20SignonHandler.GetAssertion(parser.SamlMessage, out isEncrypted);
            if (isEncrypted)
            {
                var ass = new Saml20EncryptedAssertion((RSA)Saml2Config.GetConfig().ServiceProvider.SigningCertificate.GetCertificate().PrivateKey);
                ass.LoadXml(xmlAssertion);
                ass.Decrypt();
                xmlAssertion = ass.Assertion.DocumentElement;
            }

            var assertion = new Saml20Assertion(xmlAssertion, null, Saml2Config.GetConfig().AssertionProfile.AssertionValidator, endPoint.QuirksMode);
            Logger.DebugFormat(TraceMessages.AttrQueryAssertionReceived, xmlAssertion == null ? string.Empty : xmlAssertion.OuterXml);

            if (!assertion.CheckSignature(Saml20SignonHandler.GetTrustedSigners(endPoint.Metadata.Keys, endPoint)))
            {
                Logger.Error(ErrorMessages.AssertionSignatureInvalid);
                throw new Saml20Exception(ErrorMessages.AssertionSignatureInvalid);
            }
            
            foreach (var attr in assertion.Attributes)
            {
                Saml20Identity.Current.AddAttributeFromQuery(attr.Name, attr);
            }
        }

        /// <summary>
        /// Gets the name format's string representation.
        /// </summary>
        /// <param name="nameFormat">The name format.</param>
        /// <returns>The name format string.</returns>
        private static string GetNameFormat(Saml20NameFormat nameFormat)
        {
            string result;

            switch (nameFormat)
            {
                case Saml20NameFormat.Basic:
                    result = SamlAttribute.NameformatBasic;
                    break;
                case Saml20NameFormat.Uri:
                    result = SamlAttribute.NameformatUri;
                    break;
                default:
                    throw new ArgumentException(string.Format("Unsupported nameFormat: {0}", Enum.GetName(typeof(Saml20NameFormat), nameFormat)), "nameFormat");
            }

            return result;
        }
    }
}