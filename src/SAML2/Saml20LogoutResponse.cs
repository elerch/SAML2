using System;
using System.Xml;
using SAML2.Schema.Core;
using SAML2.Schema.Protocol;
using SAML2.Utils;

namespace SAML2
{
    /// <summary>
    /// Encapsulates the LogoutResponse schema class
    /// </summary>
    public class Saml20LogoutResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Saml20LogoutResponse"/> class.
        /// </summary>
        public Saml20LogoutResponse()
        {
            Response = new LogoutResponse
                            {
                                Version = Saml20Constants.Version,
                                ID = "id" + Guid.NewGuid().ToString("N"),
                                Issuer = new NameId(),
                                IssueInstant = DateTime.Now,
                                Status = new Status { StatusCode = new StatusCode() }
                            };
        }

        /// <summary>
        /// Gets the ID.
        /// </summary>
        /// <value>The ID.</value>
        public string Id
        {
            get { return Response.ID; }
        }

        /// <summary>
        /// Gets or sets the destination.
        /// </summary>
        /// <value>The destination.</value>
        public string Destination
        {
            get { return Response.Destination; }
            set { Response.Destination = value; }
        }

        /// <summary>
        /// Gets or sets the id of the LogoutRequest to which this LogoutResponse corresponds.
        /// </summary>
        /// <value>The id of the LogoutRequest to which this LogoutResponse corresponds.</value>
        public string InResponseTo
        {
            get { return Response.InResponseTo; }
            set { Response.InResponseTo = value; }
        }

        /// <summary>
        /// Gets or sets the issuer.
        /// </summary>
        /// <value>The issuer.</value>
        public string Issuer
        {
            get { return Response.Issuer.Value; }
            set { Response.Issuer.Value = value; }
        }

        /// <summary>
        /// Gets the underlying Response schema class instance.
        /// </summary>
        /// <value>The response.</value>
        public LogoutResponse Response { get; private set; }

        /// <summary>
        /// Gets or sets the status code.
        /// </summary>
        /// <value>The status code.</value>
        public string StatusCode
        {
            get { return Response.Status.StatusCode.Value; }
            set { Response.Status.StatusCode.Value = value; }
        }

        /// <summary>
        /// Gets LogoutResponse as an XmlDocument
        /// </summary>
        /// <returns>The XML document.</returns>
        public XmlDocument GetXml()
        {
            var doc = new XmlDocument { PreserveWhitespace = true };
            doc.LoadXml(Serialization.SerializeToXmlString(Response));
            return doc;
        }
    }
}
