using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace WebAPI
{
	public class HttpRequestHandler : DelegatingHandler
	{
		protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			if (request.Content != null)
			{
				string body = await request.Content.ReadAsStringAsync();
				request.Properties["body"] = body;
			}

			return await base.SendAsync(request, cancellationToken);
		}
	}
}
