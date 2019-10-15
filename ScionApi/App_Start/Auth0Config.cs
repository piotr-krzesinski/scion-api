using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Auth0.Owin;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security.OAuth;
using Owin;

namespace ScionApi
{
	public partial class Startup
	{
		private void ConfigureAuth0(IAppBuilder app)
		{
			var domain = $"https://{ConfigurationManager.AppSettings["Auth0Domain"]}/";
			var apiIdentifier = ConfigurationManager.AppSettings["Auth0ApiIdentifier"];

			var keyResolver = new OpenIdConnectSigningKeyResolver(domain);
			
			app.UseJwtBearerAuthentication(
				new JwtBearerAuthenticationOptions
				{
					AuthenticationMode = AuthenticationMode.Active,
					TokenValidationParameters = new TokenValidationParameters()
					{
						ValidAudience = apiIdentifier,
						ValidIssuer = domain,
						IssuerSigningKeyResolver = (token, securityToken, kid, parameters) => keyResolver.GetSigningKey(kid),
						
					},
					Provider = new OAuthBearerAuthenticationProvider()
					{
						OnValidateIdentity = AddClaim
					}
				}
				);

			

		}

		private Task AddClaim(OAuthValidateIdentityContext context)
		{
			var userName = context.Ticket.Identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

			context.Ticket.Identity.AddClaim(new Claim(ClaimTypes.Name, userName));
			return Task.CompletedTask;
		}

	}
}
