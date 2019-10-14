using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.WebHooks;
using Microsoft.AspNet.WebHooks.Diagnostics;

namespace ScionApi.Webhooks
{
	public class ScionWebhookManager : WebHookManager, IWebHookManager
	{
		public ScionWebhookManager(IWebHookStore webHookStore, IWebHookSender webHookSender, ILogger logger)
			: base(webHookStore, webHookSender, logger)
		{
		}

		public new Task VerifyWebHookAsync(WebHook webHook)
		{
			return base.VerifyWebHookAsync(webHook);
		}

		public new Task<int> NotifyAsync(string user, IEnumerable<NotificationDictionary> notifications, Func<WebHook, string, bool> predicate)
		{
			
			return base.NotifyAsync(user, notifications, predicate);
		}

		public new Task<int> NotifyAllAsync(IEnumerable<NotificationDictionary> notifications, Func<WebHook, string, bool> predicate)
		{
			return base.NotifyAllAsync(notifications, predicate);
		}
	}
}
