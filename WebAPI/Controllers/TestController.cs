using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json;
using WebAPI.Models;

namespace WebAPI.Controllers
{
	public class TestController : ApiController
	{
		public Test[] Get()
		{
			List<Test> list = new List<Test>();

			using (var db = new PgDbContext())
			{
				list = db.Test.ToList();
			}

			return list.ToArray();
		}

		public Test Get(int id)
		{
			int _id = id;

			try
			{
				var body = (string)Request.Properties["body"];
				dynamic json = JsonConvert.DeserializeObject(body);
				if (json.id != null)
				{
					_id = int.Parse((string)json.id);
				}
			}
			catch (Exception ex)
			{
				return new Test() { id = -1, message = ex.Message, datetime = DateTime.Now };
			}

			using (var db = new PgDbContext())
			{
				return db.Test.SingleOrDefault(x => x.id == _id) ?? new Test() { id = -1 };
			}
		}

		public HttpResponseMessage Post([FromBody]Test value)
		{
			if (value.id > 0)
			{
				using (var db = new PgDbContext())
				{
					value.datetime = DateTime.Now;

					try
					{
						//db.Test.Attach(value);
						//db.Entry(value).State = EntityState.Added;
						db.Test.Add(value);
						db.SaveChanges();

						/*
						db.Database.ExecuteSqlCommand(@"insert into Test values(@p0, @p1, @p2)",
													  value.id,
													  value.message,
													  value.datetime);
						*/

						return Request.CreateResponse(HttpStatusCode.Created, value);
					}
					catch (DbUpdateException ex)
					{
						return Request.CreateResponse(HttpStatusCode.Conflict, new { exception = ex.GetType().Name });
					}
					catch (Exception ex)
					{
						return Request.CreateResponse(HttpStatusCode.BadRequest, new { exception = ex.GetType().Name });
					}
				}
			}

			return Request.CreateResponse(HttpStatusCode.BadRequest, new { id = value.id });
		}

		public HttpResponseMessage Put(int id, [FromBody]Test value)
		{
			using (var db = new PgDbContext())
			{
				var e = db.Test.SingleOrDefault(x => x.id == id);

				if (e == null)
				{
					return Request.CreateResponse(HttpStatusCode.NotFound, new { id = id });
				}
				else
				{
					try
					{
						e.message = value.message;

						db.Test.Attach(e);
						db.Entry(e).State = EntityState.Modified;
						db.SaveChanges();

						return Request.CreateResponse(HttpStatusCode.Accepted);
					}
					catch (Exception ex)
					{
						return Request.CreateResponse(HttpStatusCode.BadRequest, new { exception = ex.GetType().Name });
					}
				}
			}
		}

		public HttpResponseMessage Delete(int id)
		{
			using (var db = new PgDbContext())
			{
				var e = db.Test.FirstOrDefault(x => x.id == id);

				if (e == null)
				{
					return Request.CreateResponse(HttpStatusCode.NotFound, new { id = id });
				}
				else
				{
					try
					{
						db.Test.Remove(e);
						db.SaveChanges();

						return Request.CreateResponse(HttpStatusCode.OK, e);
					}
					catch (Exception ex)
					{
						return Request.CreateResponse(HttpStatusCode.BadRequest, new { exception = ex.GetType().Name });
					}
				}
			}
		}
	}
}