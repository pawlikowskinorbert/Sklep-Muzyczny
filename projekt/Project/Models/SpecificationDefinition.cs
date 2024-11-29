using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Models
{
	public class SpecificationDefinition : BaseEntity
	{
		public int Id { get; set; }
		public string? Name { get; set; }
		public string? DisplayName { get; set; }
		public string? DataType { get; set; }
		public bool IsRequired { get; set; }
		public string? Options { get; set; }
		
		public int ProductTypeId { get; set; }
		public ProductType? ProductType { get; set; }
	}
}