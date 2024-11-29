using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Models
{
	public class ProductType : BaseEntity
	{
		[Required(ErrorMessage = "Wymagane pole")]
		public required string Name { get; set; }
		public required string PhotoUrl { get; set; }
		public  string? PublicId { get; set; }


		public int CategoryId { get; set; }
		public Category? Category { get; set; }

		public ICollection<Product> Products { get; set; }
		
		public ICollection<SpecificationDefinition> SpecificationDefinitions { get; set; }


		public ProductType()
		{
			Products = new HashSet<Product>();
		}
	}
}