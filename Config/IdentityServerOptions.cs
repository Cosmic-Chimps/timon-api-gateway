namespace ApiGateway.Config
{
    public class IdentityServerOptions
    {
        public string ApiName { get; set; }
        public string ApiSecret { get; set; }
        public string Authority { get; set; }
        public string AuthenticationProviderKey { get; set; }
    }
}