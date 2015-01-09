using System.Configuration;

namespace SAML2.Config
{
    /// <summary>
    /// Contact configuration element.
    /// </summary>
    public class ContactElement
    {

        /// <summary>
        /// Gets or sets the company.
        /// </summary>
        /// <value>The company.</value>
        public string Company { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>The email.</value>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the given name.
        /// </summary>
        /// <value>The given name.</value>
        public string GivenName { get; set; }

        /// <summary>
        /// Gets or sets the phone.
        /// </summary>
        /// <value>The phone.</value>
        public string Phone { get; set; }

        /// <summary>
        /// Gets or sets the name of the sur.
        /// </summary>
        /// <value>The name of the sur.</value>
        public string SurName { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public ContactType Type { get; set; }

        /// <summary>
        /// Gets the element key.
        /// </summary>
        public object ElementKey
        {
            get { return Type; }
        }

    }
}
