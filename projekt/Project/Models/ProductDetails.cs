using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Models
{
	public class ProductDetails
	{
		public int Id { get; set; }
		public string? Value { get; set; }

		public int ProductId { get; set; }
		public Product? Product { get; set; }
		public int SpecificationDefinitionId { get; set; }
		public SpecificationDefinition? SpecificationDefinition { get; set; }
	}
}