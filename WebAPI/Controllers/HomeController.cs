using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using Npgsql;
using WebAPI.Models;
using NLog;

namespace WebAPI.Controllers
{
	public class HomeController : Controller
	{
		private static Logger log = LogManager.GetCurrentClassLogger();

		public ActionResult Index()
		{
			log.Info("Start server at " + DateTime.Now);

			var mvcName = typeof(Controller).Assembly.GetName();
			var isMono = Type.GetType("Mono.Runtime") != null;

			ViewData["Version"] = mvcName.Version.Major + "." + mvcName.Version.Minor;
			ViewData["Runtime"] = isMono ? "Mono" : ".NET";

			ViewData["List"] = string.Join(",", new List<string> { "hello", "world" });

			// データプロバイダーによるDB読み込み
			//UseDataProvider();

			// Entity FrameworkによるDB読み込み
			UseEntityFramework();

			return View();
		}

		private void UseEntityFramework()
		{
			List<string> list = new List<string>();

			using (var db = new PgDbContext())
			{
				var row = db.Test.FirstOrDefault(x => x.id == 1);

				if (row != null)
				{
					row.message = "Hello, world.";
					row.datetime = DateTime.Now;
					db.SaveChanges();
				}

				foreach (Test test in db.Test.ToList())
				{
					list.Add(test.message);
				}
			}

			ViewData["Test"] = string.Join(",", list);
		}

		private void UseDataProvider()
		{
			string connString = @"Server=localhost;Port=5432;User Id=j9;Password=;Database=mobilefirst";

			// DataReaderを利用したSELECT
			/*
			using (var conn = new NpgsqlConnection(connString))
			{
				conn.Open();

				var command = new NpgsqlCommand(@"select * from TEST", conn);

				var dataReader = command.ExecuteReader();
				while (dataReader.Read())
				{
					Console.WriteLine("value : {0},", dataReader["column_name"]);
				}
			}
			*/

			List<string> list = new List<string>();

			// DataAdapterを利用したSELECT
			using (var conn = new NpgsqlConnection(connString))
			{
				conn.Open();

				var dataAdapter = new NpgsqlDataAdapter(@"select * from TEST", conn);

				var dataSet = new DataSet();
				dataAdapter.Fill(dataSet);

				foreach (DataTable table in dataSet.Tables)
				{
					foreach (DataRow row in table.Rows)
					{
						list.Add(row["message"].ToString());
					}
				}
			}

			ViewData["Test"] = string.Join(",", list);
		}
	}
}
