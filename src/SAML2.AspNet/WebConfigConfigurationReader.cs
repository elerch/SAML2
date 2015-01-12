using SAML2.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAML2.AspNet
{
    public class WebConfigConfigurationReader : IConfigurationReader
    {
        public Saml2Configuration GetConfiguration()
        {
            //_config = ConfigurationManager.GetSection(Saml2Section.Name) as Saml2Section;

            //if (_config == null) {
            //    throw new ConfigurationErrorsException(string.Format("Configuration section \"{0}\" not found", typeof(Saml2Section).Name));
            //}

            //_config.IdentityProviders.Refresh();
        }
    }
}
