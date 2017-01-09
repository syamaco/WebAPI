using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Filters;
using NLog;
using WebAPI.Models;

namespace WebAPI
{
	public class GlobalExceptionFilter : IExceptionFilter
	{
		private static Logger log = LogManager.GetCurrentClassLogger();

		public GlobalExceptionFilter()
		{
		}

		public bool AllowMultiple
		{
			get { return true; }
		}

		public Task ExecuteExceptionFilterAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
		{
			return Task.Factory.StartNew(() =>
			{
				log.Error(actionExecutedContext.Exception, "web service error : {0}", actionExecutedContext.Exception.Message);

				// カスタムレスポンスを返す
				HttpResponseMessage response = actionExecutedContext.Request.CreateResponse(HttpStatusCode.OK, new Test()
				{
					id = -1,
					message = actionExecutedContext.Exception.Message,
					datetime = DateTime.Now
				}/*, new XmlMediaTypeFormatter(), new MediaTypeHeaderValue("application/xml")*/);
				actionExecutedContext.Response = response;

			}, cancellationToken);
		}
	}
}
