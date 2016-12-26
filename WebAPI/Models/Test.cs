using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.Models
{
	[Table("test", Schema="public")]
	public class Test
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		[Column("id")]
		public int id { get; set; }

		[Column("message")]
		public string message { get; set; }

		[Column("datetime")]
		public DateTime datetime { get; set; }
	}
}
