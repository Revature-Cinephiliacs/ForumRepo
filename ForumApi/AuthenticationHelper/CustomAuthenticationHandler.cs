using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace ForumApi.AuthenticationHelper
{
    public class CustomAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public IServiceProvider ServiceProvider { get; set; }

        public CustomAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, IServiceProvider serviceProvider)
            : base(options, logger, encoder, clock)
        {
            ServiceProvider = serviceProvider;
        }

        protected async override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var headers = Request.Headers;
            // var token = headers["Authorization"];
            var token = Helper.GetTokenFromRequest(Request);

            if (string.IsNullOrEmpty(token))
            {
                return AuthenticateResult.Fail("Token is null");
            }


            var response = await Helper.Sendrequest("", RestSharp.Method.GET, token);
            if (!response.IsSuccessful)
            {
                return AuthenticateResult.Fail($"Balancer not authorize token : for token={token}");
            }
            var respObj = JsonConvert.DeserializeObject<PermissionObj>(response.Content);


            var claims = new List<Claim>();
            respObj.permissions.ForEach(per =>
            {
                claims.Add(new Claim(per, "true"));
            });
            // context.User.Claims.Append();
            // var identity = new ClaimsIdentity(claims, "basic");
            // User = new ClaimsPrincipal(identity);

            // var claims = new[] { new Claim("token", token) };
            var identity = new ClaimsIdentity(claims, nameof(CustomAuthenticationHandler));
            var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), this.Scheme.Name);
            return AuthenticateResult.Success(ticket);
        }
    }

    public class PermissionObj
    {
        public string access { get; set; }
        public List<string> permissions { get; set; }
    }
}