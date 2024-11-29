using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Project.Models;

namespace Project.Specifications
{
	public class TypeListSpecification : BaseSpecification<Product, ProductType>
	{
		public TypeListSpecification()
		{
			AddInclude(x=> x.ProductType!);
			AddSelect(x => x.ProductType!);
			ApplyDistinct();
		}
	}
}