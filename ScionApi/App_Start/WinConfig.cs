using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Owin;

namespace ScionApi
{
	public partial class Startup
	{

		private void ConfigureWindowsAuth(IAppBuilder app)
		{
			HttpListener listener = (HttpListener) app.Properties["System.Net.HttpListener"];
			listener.AuthenticationSchemes = AuthenticationSchemes.IntegratedWindowsAuthentication;
		}
	}
}
