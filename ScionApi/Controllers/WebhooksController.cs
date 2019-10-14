using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Results;
using Microsoft.AspNet.WebHooks;
using Microsoft.AspNet.WebHooks.Controllers;
using Microsoft.AspNet.WebHooks.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using ScionApi.Auth0;
using ScionApi.Providers;

namespace ScionApi.Controllers
{
	[Authorize]
	[RoutePrefix("api/webhooks")]
	public class WebhooksController : ApiController
	{
		private OAuthDetails AccessDetails { get; set; }

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
		[Route("create")]
		public IHttpActionResult Create()
		{
			string baseAddress = "http://localhost:9000/";

			var webHook = new WebHook()
			{
				Description = "My Webhook",
				Secret = "12345678901234567890123456789012",
				Id = Guid.NewGuid().ToString(),
				WebHookUri = new Uri("https://localhost:44354/api/webhooks/incoming/scion")
			};

			var client = new RestClient($"{baseAddress}/api/webhooks/registrations");
			var request = new RestRequest(Method.POST);
			request.AddHeader("content-type", "application/json");

			string body = JsonConvert.SerializeObject(webHook);  

			request.AddParameter("application/json",
				body
				, ParameterType.RequestBody);

			request.AddParameter("Authorization", Request.Headers.Authorization, ParameterType.HttpHeader);

			IRestResponse response = client.Execute(request);

			if (response.StatusCode == HttpStatusCode.Created)
			{
				var webHookToReturn =  _webHookStore.LookupWebHookAsync(RequestContext.Principal.Identity.Name, webHook.Id);

				return  CreatedAtRoute("RegistrationLookupAction", new {id = webHook.Id}, webHookToReturn);
			}

			return new ResponseMessageResult(
				Request.CreateErrorResponse(        
					response.StatusCode,
					new HttpError(response.ErrorMessage)
				)
			);
		}



		[HttpGet]
		[Route("trigger")]
		public async Task<int> Trigger()
		{
			return await this.NotifyAllAsync(ScionFilterProvider.Event1, new {Message = "Event 'event1' has been triggered"});
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
