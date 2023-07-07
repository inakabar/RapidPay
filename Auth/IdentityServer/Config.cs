using IdentityServer4.Models;

namespace IdentityServer
{
    public class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResource
                {
                    Name= "role",
                    UserClaims= new List<string> { "role" },
                }
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new[]
            {
                new ApiScope("API.read"),
                new ApiScope("API.write"),
            };

        public static IEnumerable<ApiResource> ApiResources =>
            new[]
            {
                new ApiResource("API")
                {
                    Scopes = new List<string> { "API.read", "API.write"},
                    ApiSecrets = new List<Secret> { new Secret("ScopeSecret".Sha256())},
                    UserClaims = new List<string> { "role" }
                },
            };

        public static IEnumerable<Client> Clients =>
        new[]
        {
            // m2m client credentials flow client
            new Client
            {
              ClientId = "m2m.client-admin",
              ClientName = "Client Credentials Client",

              AllowedGrantTypes = GrantTypes.ClientCredentials,
              ClientSecrets = {new Secret("SuperSecretPassword".Sha256())},
              AllowedScopes = {"API.read", "API.write" }
            },
            new Client
            {
              ClientId = "m2m.client.readonly",
              ClientName = "Client Credentials Client",

              AllowedGrantTypes = GrantTypes.ClientCredentials,
              ClientSecrets = {new Secret("SuperSecretPassword".Sha256())},
              AllowedScopes = { "API.read" }
            },

            // interactive client using code flow + pkce -> Usage for client apps
            //new Client
            //{
            //  ClientId = "interactive",
            //  ClientSecrets = {new Secret("SuperSecretPassword".Sha256())},

            //  AllowedGrantTypes = GrantTypes.Code,

            //  RedirectUris = {"https://localhost:5444/signin-oidc"},
            //  FrontChannelLogoutUri = "https://localhost:5444/signout-oidc",
            //  PostLogoutRedirectUris = {"https://localhost:5444/signout-callback-oidc"},

            //  AllowOfflineAccess = true,
            //  AllowedScopes = {"openid", "profile", "API.read"},
            //  RequirePkce = true,
            //  RequireConsent = true,
            //  AllowPlainTextPkce = false
            //},
        };
    }
}
