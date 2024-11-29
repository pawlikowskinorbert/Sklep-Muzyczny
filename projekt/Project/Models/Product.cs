using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Models
{
	public class Product : BaseEntity
	{

		[Required(ErrorMessage = "Wymagane pole")]
		public required string Name { get; set; }
		[Required(ErrorMessage = "Wymagane pole")]
		public required string Description { get; set; }
		public required string PhotoUrl { get; set; }
		public  string? PublicId { get; set; }
		
		public decimal Price { get; set; }
		public int QuantityInStoct { get; set; }
		
		public int ProductTypeId { get; set; }
		public ProductType? ProductType { get; set; }
		
		
		
		public int BrandId { get; set; }
		public Brand? Brand { get; set; }
		
		
		public ICollection<ProductDetails>? ProductDetails { get; set; }
	}
}