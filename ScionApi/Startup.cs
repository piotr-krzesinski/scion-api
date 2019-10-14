using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks.Dataflow;
using System.Web.Http;
using Auth0.Owin;
using Autofac;
using Autofac.Integration.WebApi;
using Microsoft.AspNet.WebHooks;
using Microsoft.AspNet.WebHooks.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Jwt;
using Owin;
using ScionApi.Webhooks;

namespace ScionApi
{
	public partial class Startup
	{
		public void Configuration(IAppBuilder appBuilder)
		{
			//ConfigureAuth0(appBuilder);
			ConfigureWindowsAuth(appBuilder);

			var builder = new ContainerBuilder();
			var config = new HttpConfiguration();
			
			config.Formatters.JsonFormatter.SupportedMediaTypes
				.Add(new MediaTypeHeaderValue("text/html"));

			config.MapHttpAttributeRoutes();
			
			config.Routes.MapHttpRoute(
				name: "DefaultApi",
				routeTemplate: "api/{controller}/{id}",
				defaults: new { id = RouteParameter.Optional }
			);

			config.InitializeCustomWebHooks();
			config.InitializeCustomWebHooksSqlStorage();
			config.InitializeCustomWebHooksApis();


		   Collection<TimeSpan> retries = new Collection<TimeSpan> { TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(20) };
			

			ILogger logger = config.DependencyResolver.GetLogger();
			IWebHookStore store = config.DependencyResolver.GetStore();
			IWebHookSender sender = new ScionWebHookSender(logger, store, retries, null);
			IWebHookManager manager = new ScionWebhookManager(store, sender, logger);
			
			builder.RegisterInstance(manager).As<IWebHookManager>().SingleInstance();
			builder.RegisterInstance(sender).As<IWebHookSender>().SingleInstance();

			builder.RegisterApiControllers(Assembly.GetExecutingAssembly()).InstancePerRequest();
			
			var container = builder.Build();
			
			config.DependencyResolver = new AutofacWebApiDependencyResolver(container);

			config.EnsureInitialized();

			appBuilder.UseAutofacMiddleware(container);
			appBuilder.UseAutofacWebApi(config);

			//appBuilder.UseJwtBearerAuthentication()

			appBuilder.UseCors(CorsOptions.AllowAll);
			appBuilder.UseWebApi(config);
		}
	}
}
