using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Models
{
	public class ShoppingCartItemViewModel
	{
		public int Id { get; set; }
		public int ProductId { get; set; }
		public string ProductName { get; set; }
		public decimal Price { get; set; }
		public int Quantity { get; set; }
		public int QuantityInStoct { get; set; } // Dodane pole

		public decimal TotalPrice { get; set; }


	}
}