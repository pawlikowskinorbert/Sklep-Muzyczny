using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Project.Models;

namespace Project.Specifications
{
	public class ShoppingCartWithItemsSpecification : BaseSpecification<ShoppingCart>
	{
		public ShoppingCartWithItemsSpecification(string userId)
		: base(sc => sc.AppUserId == userId)
		{
			AddInclude(cart => cart.ShoppingCartItems);
			AddInclude("ShoppingCartItems.Product");
		}
	}
}