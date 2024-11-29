using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Project.Models;

namespace Project.Specifications
{
	public class BrandListSpecification : BaseSpecification<Product, Brand>
	{
		public BrandListSpecification()
		{
			AddInclude(x=> x.Brand);
			AddSelect(x => x.Brand);
			ApplyDistinct();
		}
	}
}