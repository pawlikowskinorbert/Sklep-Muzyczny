using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Project.Models;

namespace Project.Specifications
{
	public class CategoryListSpecification : BaseSpecification<Product, Category>
	{
		public CategoryListSpecification()
		{
			AddInclude(x => x.ProductType.Category);
			AddSelect(x => x.ProductType.Category);
			ApplyDistinct();
		}
	}
}