using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Project.Specifications;

namespace Project.Models
{
	public class SpecificationDefinitionListSpecification : BaseSpecification<SpecificationDefinition>
	{
		public SpecificationDefinitionListSpecification(int productTypeId)
			: base(sd => sd.ProductTypeId == productTypeId)
		{
			AddInclude(sd => sd.ProductType);
		}
	}
}