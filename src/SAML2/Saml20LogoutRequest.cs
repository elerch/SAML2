using System;
using System.Xml;
using SAML2.Config;
using SAML2.Schema.Core;
using SAML2.Schema.Protocol;
using SAML2.Utils;

namespace SAML2
{
    /// <summary>
    /// Encapsulates the LogoutRequest schema class
    /// </summary>
    public class Saml20LogoutRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Saml20LogoutRequest"/> class.
        /// </summary>
        public Saml20LogoutRequest()
        {
            Request = new LogoutRequest
                           {
                               Version = Saml20Constants.Version,
                               Id = "id" + Guid.NewGuid().ToString("N"),
                               Issuer = new NameId(),
                               IssueInstant = DateTime.Now
                           };
        }

        #region Properties

        /// <summary>
        /// Gets or sets <c>NotOnOrAfter</c>.
        /// </summary>
        /// <value>The Not On Or After date.</value>
        public DateTime? NotOnOrAfter
        {
            get { return Request.NotOnOrAfter; }
            set { Request.NotOnOrAfter = value; }
        }

        /// <summary>
        /// Gets or sets the destination.
        /// </summary>
        /// <value>The destination.</value>
        public string Destination
        {
            get { return Request.Destination; }
            set { Request.Destination = value; }
        }

        /// <summary>
        /// Gets the id of the logout request.
        /// </summary>
        public string Id
        {
            get { return Request.Id; }
        }

        /// <summary>
        /// Gets or sets the issuer value.
        /// </summary>
        /// <value>The issuer value.</value>
        public string Issuer
        {
            get { return Request.Issuer.Value; }
            set { Request.Issuer.Value = value; }
        }

        /// <summary>
        /// Gets or sets the reason for this logout request.
        /// Defined values should be on uri form.
        /// </summary>
        /// <value>The reason.</value>
        public string Reason
        {
            get { return Request.Reason; }
            set { Request.Reason = value; }
        }

        /// <summary>
        /// Gets the underlying <c>LogoutRequest</c> schema class instance.
        /// </summary>
        /// <value>The request.</value>
        public LogoutRequest Request { get; private set; }

        /// <summary>
        /// Gets or sets the <c>SessionIndex</c>.
        /// </summary>
        /// <value>The SessionIndex.</value>
        public string SessionIndex
        {
            get { return Request.SessionIndex[0]; }
            set { Request.SessionIndex = new[] { value }; }
        }

        /// <summary>
        /// Gets or sets <c>SubjectToLogOut</c>.
        /// </summary>
        /// <value>The Subject To LogOut.</value>
        public NameId SubjectToLogOut
        {
            get { return Request.Item as NameId; }
            set { Request.Item = value; }
        }

        #endregion

        /// <summary>
        /// Returns an instance of the class with meaningful default values set.
        /// </summary>
        /// <returns>The <see cref="Saml20LogoutRequest"/>.</returns>
        public static Saml20LogoutRequest GetDefault()
        {
            var config = Saml2Config.GetConfig();
            var result = new Saml20LogoutRequest
                             {
                                 SubjectToLogOut = new NameId(),
                                 Issuer = config.ServiceProvider.Id
                             };

            return result;
        }

        /// <summary>
        /// Returns the <c>AuthnRequest</c> as an XML document.
        /// </summary>
        /// <returns>The request XML.</returns>
        public XmlDocument GetXml()
        {
            var doc = new XmlDocument { PreserveWhitespace = true };
            doc.LoadXml(Serialization.SerializeToXmlString(Request));

            return doc;
        }
    }
}
