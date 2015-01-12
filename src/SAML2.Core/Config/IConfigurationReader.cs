namespace SAML2.Config
{
    public interface IConfigurationProvider
    {
        Saml2Configuration GetConfiguration();
    }
}
