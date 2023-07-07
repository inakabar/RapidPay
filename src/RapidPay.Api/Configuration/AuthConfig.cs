namespace RapidPay.Api.Configuration
{
    public static class AuthConfig
    {
        public static IServiceCollection AddAuthConfig(this IServiceCollection services, IConfiguration configuration)
        {
            string authority = configuration.GetValue<string>("IdentityAuthority");
            string apiName = configuration.GetValue<string>("IdentityAuthorityApiName");
            var policies = configuration.GetSection("SecurityPolicies").Get<List<SecurityPolicy>>();

            services.AddAuthentication("Bearer")
                .AddIdentityServerAuthentication("Bearer", options =>
                {
                    options.Authority = authority; //"https://localhost:5443";
                    options.ApiName = apiName; //"API";
                });

            services.AddAuthorization(options =>
            {
                foreach (var item in policies)
                {
                    options.AddPolicy(item.PolicyName, policy =>
                    {
                        policy.RequireAuthenticatedUser();
                        policy.RequireClaim("scope", item.Scopes);
                    });
                }
            });

            return services;
        }
    }

    public class SecurityPolicy
    {
        public string PolicyName { get; set; }
        public List<string> Scopes { get; set; }
    }
}
