using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using GSS.Authentication.CAS.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace M5x.Sso
{
    public static class Inject
    {
        public static IApplicationBuilder UseSso(this IApplicationBuilder app)
        {
            app.UseAuthentication();
            return app;
        }

        public static IServiceCollection AddSso(this IServiceCollection services)
        {
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCAS(options =>
                {
                    options.CallbackPath = "/signin-cas";
                    options.CasServerUrlBase = SsoConfig.SsoUrl;
                    options.SaveTokens = true;
                    options.Events = new CasEvents
                    {
                        OnCreatingTicket = context =>
                        {
                            // add claims from CasIdentity.Assertion ?
                            var assertion = context.Assertion;
                            if (assertion == null || !assertion.Attributes.Any()) return Task.CompletedTask;
                            if (!(context.Principal.Identity is ClaimsIdentity identity)) return Task.CompletedTask;
                            var email = assertion.Attributes["email"].FirstOrDefault();
                            if (!string.IsNullOrEmpty(email)) identity.AddClaim(new Claim(ClaimTypes.Email, email));

                            var name = assertion.Attributes["display_name"].FirstOrDefault();
                            if (!string.IsNullOrEmpty(name)) identity.AddClaim(new Claim(ClaimTypes.Name, name));

                            return Task.CompletedTask;
                        }
                    };
                });
            return services;
        }
    }
}