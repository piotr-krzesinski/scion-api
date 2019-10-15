using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using Microsoft.AspNet.WebHooks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using ScionApi.Providers;

namespace ScionApi.Controllers
{
	[Authorize]
	[RoutePrefix("api/webhooks")]
	public class WebhooksController : ApiController
	{
		private IWebHookStore _webHookStore;
		private IWebHookFilterProvider _filterProvider;
		
		protected override void Initialize(HttpControllerContext controllerContext)
		{
			base.Initialize(controllerContext);

			_webHookStore = Configuration.DependencyResolver.GetStore();
			_filterProvider = Configuration.DependencyResolver.GetFilterProviders().FirstOrDefault();
		}

		[AllowAnonymous]
		public string Get()
		{
			return "Scion WebHooks API";
		}

		[HttpGet]
		[Route("getUserInfo")]
		public JObject UserInfo()
		{
			var url = $"https://{ConfigurationManager.AppSettings["Auth0Domain"]}/userInfo";

			var client = new RestClient(new Uri(url));
			var request = new RestRequest(Method.GET);

			//add GetToken() API method parameters
			request.Parameters.Clear();
			request.AddHeader("Authorization", Request.Headers.Authorization.ToString());
			request.AddHeader("Accept", "application/xml");
			
			//make the API request and get the response
			IRestResponse response = client.Execute<JObject>(request);

			dynamic jsonResponse = JsonConvert.DeserializeObject(response.Content);

			return jsonResponse;
		}

		[HttpPost]
		[Route("create")]
		public  async Task<IHttpActionResult> Create([FromBody]WebHook webHook)
		{
			
			await _webHookStore.InsertWebHookAsync(RequestContext.Principal.Identity.Name, webHook);

			return CreatedAtRoute("RegistrationLookupAction", new {id = webHook.Id}, webHook);
		}



		[HttpGet]
		[Route("trigger")]
		public async Task<string> Trigger()
		{
			string filter = "111111111";

			int notified = await this.NotifyAllAsync(ScionFilterProvider.Event1, new {Message = "Event 'event1' has been triggered"},
				(hook, s) =>
				{
					bool result = Equals(hook.Properties["originatorId"], filter);

					return result;
				});

			return $"Webhooks notified: {notified}";
		}
		
		[Route("registrations/{id}/filters")]
		public async Task<IHttpActionResult> Get(string id)
		{
			var webHook = await _webHookStore.LookupWebHookAsync(User.Identity.Name, id);
			if (webHook != null)
			{
				var filters = await _filterProvider.GetFiltersAsync();
				
				return Ok(filters.Where(x => webHook.Filters.Any(f => f.Equals(x.Name))));
			}

			return NotFound();
		}

	}
}
