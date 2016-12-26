﻿using System.Web.Http;
using System.Web.Http.Cors;

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
		}
	}
}