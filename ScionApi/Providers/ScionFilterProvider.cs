using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.WebHooks;

namespace ScionApi.Providers
{

	public class ScionFilterProvider : IWebHookFilterProvider
	{
		public const string Event1 = "event1";
		public const string Event2 = "event2";

		private readonly Collection<WebHookFilter> filters = new Collection<WebHookFilter>
		{
			new WebHookFilter {Name = Event1, Description = "This event 1 happened foo."},
			new WebHookFilter {Name = Event2, Description = "This event 2 happened boo."},
		};

		public Task<Collection<WebHookFilter>> GetFiltersAsync()
		{
			return Task.FromResult(this.filters);
		}
	}


}
