using System;
using System.Text;
using System.Data.Entity;
using Npgsql;
using WebAPI.Models;

namespace WebAPI
{
	public class PgDbContext : DbContext
	{
		public DbSet<Test> Test { get; set; }

		public PgDbContext() : base(nameOrConnectionString: "MobileFirst")
		{
		}
	}
}
