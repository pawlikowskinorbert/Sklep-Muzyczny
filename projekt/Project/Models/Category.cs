using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Models
{
	public class Category : BaseEntity
	{
		[Required(ErrorMessage ="Wymagane Pole")]
		public required string Name { get; set; }
		public ICollection<ProductType> ProductTypes { get; set; }
		public string? PhotoUrl { get; set; }
		public string? PublicId {get; set;}
		
		
		public Category()
		{
			ProductTypes = new HashSet<ProductType>();
		}
	}
}