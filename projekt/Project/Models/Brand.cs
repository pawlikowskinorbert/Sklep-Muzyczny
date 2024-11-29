using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Models
{
	public class Brand : BaseEntity
	{
		public required string Name { get; set; }
		public required string PhotoUrl { get; set; }
		public  string? PublicId { get; set; }
		public ICollection<Product>? Products { get; set; }
		
		
	}
}