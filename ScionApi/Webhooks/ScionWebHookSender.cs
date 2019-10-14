using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Web.Http.Tracing;
using Microsoft.AspNet.WebHooks;
using Microsoft.AspNet.WebHooks.Diagnostics;

namespace ScionApi.Webhooks
{
	public class ScionWebHookSender : DataflowWebHookSender
	{
		private readonly IWebHookStore _store;

		public override Task SendWebHookWorkItemsAsync(IEnumerable<WebHookWorkItem> workItems)
		{
			this.Logger.Log(TraceLevel.Info, $"[SCION] Sending {workItems.Count()} webhooks.", null);

			return base.SendWebHookWorkItemsAsync(workItems);
		}

		protected override Task OnWebHookSuccess(WebHookWorkItem workItem)
		{
			this.Logger.Log(TraceLevel.Info, $"[SCION] Webhook '{workItem.WebHook.Id}' sent.", null);

			return Task.FromResult(true);
		}

		protected override Task OnWebHookFailure(WebHookWorkItem workItem)
		{
			this.Logger.Log(TraceLevel.Error, $"[SCION] Webhook '{workItem.WebHook.Id}' sent failed.", null);

			return DeleteWebHook(workItem.WebHook.Id);
		}

		protected override Task OnWebHookRetry(WebHookWorkItem workItem)
		{
			this.Logger.Log(TraceLevel.Warn, $"[SCION] Webhook '{workItem.WebHook.Id}' in retry ${workItem.Offset}.", null);

			return Task.FromResult(true);
		}

		protected override Task OnWebHookGone(WebHookWorkItem workItem)
		{
			this.Logger.Log(TraceLevel.Error, $"[SCION] Webhook '{workItem.WebHook.Id}' sent failed.", null);

			return DeleteWebHook(workItem.WebHook.Id);
		}

		private async Task DeleteWebHook(string webHookId)
		{
			string webHookUser = null;

			await _store.QueryWebHooksAcrossAllUsersAsync(new[] { "*" },
				(webHook, user) => {
					if (webHook.Id == webHookId)
					{
						webHookUser = user;
					}

					return false;
				});
			
			if (webHookUser != null)
			{
				await _store.DeleteWebHookAsync(webHookUser, webHookId);

				this.Logger.Log(TraceLevel.Error, $"[SCION] Webhook '{webHookId}' deleted.", null);
			}
		}

		public ScionWebHookSender(ILogger logger, IWebHookStore store) : base(logger)
		{
			_store = store;
		}

		public ScionWebHookSender(ILogger logger, IWebHookStore store, IEnumerable<TimeSpan> retryDelays, ExecutionDataflowBlockOptions options) : base(logger, retryDelays, options)
		{
			_store = store;
		}
	}
}
