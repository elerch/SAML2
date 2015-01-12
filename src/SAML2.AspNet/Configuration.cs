using SAML2.Config;
using System;

namespace SAML2.AspNet
{
    public class ConfigurationFactory
    {
        private Saml2Section configuration;
        public Saml2Section Configuration
        {
            get {
                return configuration ?? (configuration = GetConfiguration());
            }
        }

        private Saml2Section GetConfiguration()
        {
            return ((IConfigurationReader)Activator.CreateInstance(Type.GetType(FetcherType))).GetConfiguration();
        }

        private string fetcherType;
        public string FetcherType
        {
            get
            {
                return fetcherType ?? (fetcherType = GetFetcherType());
            }
            set
            {
                configuration = null;
                fetcherType = value;
            }
        }

        private string GetFetcherType()
        {
            return System.Configuration.ConfigurationManager.AppSettings["saml2:configurationReaderType"] ?? "SAML2.AspNet.WebConfigConfigurationReader";
        }

        private static readonly ConfigurationFactory instance = new ConfigurationFactory();
        public static ConfigurationFactory Instance { get { return instance;  } }
    }
}
