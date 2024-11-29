using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Project.Models;

namespace Project.Specifications
{
	public class ProductsWithFiltersForCountSpecification : BaseSpecification<Product>
	{
		public ProductsWithFiltersForCountSpecification(ProductSpecParams specParams)
		: base(x =>
		(string.IsNullOrEmpty(specParams.Search) || x.Name.ToLower().Contains(specParams.Search)) &&
		(!specParams.BrandId.HasValue || x.BrandId == specParams.BrandId) &&
		(!specParams.TypeId.HasValue || x.ProductTypeId == specParams.TypeId) &&
		(!specParams.CategoryId.HasValue || x.ProductType.CategoryId == specParams.CategoryId)
		)
		{

		}
	}
}