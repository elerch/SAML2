using System;

namespace SAML2.Config
{
    internal class ConfigurationPropertyAttribute : Attribute
    {
        private string v;

        public ConfigurationPropertyAttribute(string v)
        {
            this.v = v;
        }
    }
}