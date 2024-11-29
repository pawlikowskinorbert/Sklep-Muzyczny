using System.ComponentModel;
using Project.Models;

namespace Project.Specifications
{
	public class ProductSpecification : BaseSpecification<Product>
	{
		public ProductSpecification(ProductSpecParams specParams) : base(x =>
		(string.IsNullOrEmpty(specParams.Search) || x.Name.ToLower().Contains(specParams.Search)) &&
		(!specParams.BrandId.HasValue || x.BrandId == specParams.BrandId) &&
		(!specParams.TypeId.HasValue || x.ProductTypeId == specParams.TypeId) &&
		(!specParams.CategoryId.HasValue || x.ProductType.CategoryId == specParams.CategoryId)
		)
		{
			AddInclude(x => x.Brand!);
			AddInclude(x => x.ProductType!);
			AddInclude(x => x.ProductType!.Category!);
			AddInclude(x => x.ProductDetails!);


			ApplyPaging(specParams.PageSize * (specParams.PageIndex - 1), specParams.PageSize);

			switch (specParams.Sort?.ToLower())
			{
				case "priceasc":
					AddOrderBy(x => x.Price);
					break;
				case "pricedesc":
					AddOrderByDescending(x => x.Price);
					break;
				case "nameasc":
					AddOrderBy(x => x.Name);
					break;
				case "namedesc":
					AddOrderByDescending(x => x.Name);
					break;
				default:
					AddOrderBy(x => x.Id);
					break;
			}
		}
	}
}