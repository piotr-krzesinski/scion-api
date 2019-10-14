using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.WebHooks;
using Microsoft.Owin.Hosting;

namespace ScionApi
{
	class Program
	{
		static void Main(string[] args)
		{
			string baseAddress = "http://localhost:9000/";

			// Start OWIN host 
			using (WebApp.Start<Startup>(url: baseAddress))
			{
				Console.WriteLine(@"Scion API on Owin Host started.");
				Console.ReadLine();
			}
		}
	}
}
