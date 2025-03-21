using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

[assembly: OwinStartup(typeof(LegacyAuthentication.Okta.Mvc.Startup))]

namespace LegacyAuthentication.Okta.Mvc
{
    public class Startup
    {
        private readonly string _clientId = "YOUR_CLIENT_ID";
        private readonly string _clientSecret = "YOUR_CLIENT_SECRET";
        private readonly string _authority = "https://YOUR_OKTA_DOMAIN/oauth2/default";
        private readonly string _redirectUri = "https://localhost:44300/authorization-code/callback";
        private readonly string _postLogoutRedirectUri = "https://localhost:44300/";

        public void Configuration(IAppBuilder app)
        {
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            app.UseCookieAuthentication(new CookieAuthenticationOptions());

            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            {
                ClientId = _clientId,
                ClientSecret = _clientSecret,
                Authority = _authority,
                RedirectUri = _redirectUri,
                PostLogoutRedirectUri = _postLogoutRedirectUri,
                ResponseType = OpenIdConnectResponseType.Code,
                Scope = "openid profile email",
                TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "name",
                    ValidateIssuer = true
                },
                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    AuthenticationFailed = context =>
                    {
                        context.HandleResponse();
                        context.Response.Redirect("/Home/Error?message=" + context.Exception.Message);
                        return Task.FromResult(0);
                    },
                    SecurityTokenValidated = context =>
                    {
                        // Additional claims handling if needed
                        return Task.FromResult(0);
                    }
                }
            });
        }
    }
}