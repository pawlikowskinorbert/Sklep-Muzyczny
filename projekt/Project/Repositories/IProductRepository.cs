using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Project.Models;

namespace Project.Repositories
{
	public interface IProductRepository
	{
		Task<IReadOnlyList<Product>> GetProductsAsync();
		Task<Product?> GetProductByIdAsync(int id);
		void AddProduct(Product product);
		void UpdateProduct(Product product);
		void DeleteProduct(Product product);
		bool ProductExists(int id);
		
	}
}