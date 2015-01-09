using System.Configuration;

namespace SAML2.Config
{
    /// <summary>
    /// Contact configuration element.
    /// </summary>
    public class ContactElement : WritableConfigurationElement, IConfigurationElementCollectionElement
    {
        #region Attributes

        /// <summary>
        /// Gets or sets the company.
        /// </summary>
        /// <value>The company.</value>
        [ConfigurationProperty("company")]
        public string Company
        {
            get { return (string)base["company"]; }
            set { base["company"] = value; }
        }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>The email.</value>
        [ConfigurationProperty("email")]
        public string Email
        {
            get { return (string)base["email"]; }
            set { base["email"] = value; }
        }

        /// <summary>
        /// Gets or sets the given name.
        /// </summary>
        /// <value>The given name.</value>
        [ConfigurationProperty("givenName")]
        public string GivenName
        {
            get { return (string)base["givenName"]; }
            set { base["givenName"] = value; }
        }

        /// <summary>
        /// Gets or sets the phone.
        /// </summary>
        /// <value>The phone.</value>
        [ConfigurationProperty("phone")]
        public string Phone
        {
            get { return (string)base["phone"]; }
            set { base["phone"] = value; }
        }

        /// <summary>
        /// Gets or sets the name of the sur.
        /// </summary>
        /// <value>The name of the sur.</value>
        [ConfigurationProperty("surName")]
        public string SurName
        {
            get { return (string)base["surName"]; }
            set { base["surName"] = value; }
        }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        [ConfigurationProperty("type", IsKey = true, IsRequired = true)]
        public ContactType Type
        {
            get { return (ContactType)base["type"]; }
            set { base["type"] = value; }
        }

        #endregion

        #region Implementation of IConfigurationElementCollectionElement

        /// <summary>
        /// Gets the element key.
        /// </summary>
        public object ElementKey
        {
            get { return Type; }
        }

        #endregion
    }
}
