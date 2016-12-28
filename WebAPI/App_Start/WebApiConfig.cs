using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http;
using System.Web.Http.Cors;
using WebAPI.Models;

namespace WebAPI
{
	public static class WebApiConfig
	{
		public static void Register(HttpConfiguration config)
		{
			// Web API configuration and services
			var cors = new EnableCorsAttribute("http://localhost:8100", "*", "*");
			cors.SupportsCredentials = true;
			config.EnableCors(cors);

			// Web API routes
			config.MapHttpAttributeRoutes();

			config.Routes.MapHttpRoute(
				name: "DefaultApi",
				routeTemplate: "api/{controller}/{id}",
				defaults: new { id = RouteParameter.Optional }
			);

			//config.Formatters.Remove(config.Formatters.XmlFormatter);
			config.Formatters.Add(new TestCsvFormatter());
		}
	}

	public class TestCsvFormatter : BufferedMediaTypeFormatter
	{
		public TestCsvFormatter()
		{
			this.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/csv"));

			SupportedEncodings.Add(new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
		}

		public override bool CanReadType(Type type)
		{
			return false;
		}

		public override bool CanWriteType(System.Type type)
		{
			if (type == typeof(Test))
			{
				return true;
			}
			else
			{
				Type enumerableType = typeof(IEnumerable<Test>);
				return enumerableType.IsAssignableFrom(type);
			}
		}

		public override void WriteToStream(Type type, object value, Stream writeStream, HttpContent content)
		{
			Encoding effectiveEncoding = SelectCharacterEncoding(content.Headers);

			using (var writer = new StreamWriter(writeStream, effectiveEncoding))
			{
				var tests = value as IEnumerable<Test>;
				if (tests != null)
				{
					foreach (var test in tests)
					{
						WriteItem(test, writer);
					}
				}
				else
				{
					var singleTest = value as Test;
					if (singleTest == null)
					{
						throw new InvalidOperationException("Cannot serialize type");
					}
					WriteItem(singleTest, writer);
				}
			}
		}

		// Helper methods for serializing Products to CSV format. 
		private void WriteItem(Test test, StreamWriter writer)
		{
			writer.WriteLine("{0},{1},{2}", Escape(test.id), Escape(test.message), Escape(test.datetime));
		}

		static char[] _specialChars = new char[] { ',', '\n', '\r', '"' };

		private string Escape(object o)
		{
			if (o == null)
			{
				return "";
			}
			string field = o.ToString();
			if (field.IndexOfAny(_specialChars) != -1)
			{
				// Delimit the entire field with quotes and replace embedded quotes with "".
				return String.Format("\"{0}\"", field.Replace("\"", "\"\""));
			}
			else return field;
		}

		/*
		public override void SetDefaultContentHeaders(System.Type type, HttpContentHeaders headers, MediaTypeHeaderValue mediaType)
		{
			base.SetDefaultContentHeaders(type, headers, mediaType);
			headers.ContentType = new MediaTypeHeaderValue("application/json");
		}
		*/
	}
}
