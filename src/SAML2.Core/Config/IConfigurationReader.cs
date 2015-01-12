using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAML2.Config
{
    public interface IConfigurationReader
    {
        Saml2Section GetConfiguration();
    }
}
