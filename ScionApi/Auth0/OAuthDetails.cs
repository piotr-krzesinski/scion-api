using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;

namespace ScionApi.Auth0
{
	public class OAuthDetails
	{
		public string access_token { get; set; }
		public string token_type { get; set; }
	}
}
