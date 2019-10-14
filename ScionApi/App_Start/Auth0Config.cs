using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Auth0.Owin;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Jwt;
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
						IssuerSigningKeyResolver = (token, securityToken, kid, parameters) => keyResolver.GetSigningKey(kid)
					}
				});
		}
	}
}
