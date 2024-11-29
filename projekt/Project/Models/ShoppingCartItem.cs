using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Models
{
	public class ShoppingCartItem : BaseEntity
	{
		public int ShoppingCartId { get; set; }
		public ShoppingCart? ShoppingCart { get; set; }
		
		public int ProductId { get; set; }
		public Product? Product { get; set; }

		[Range(1, int.MaxValue, ErrorMessage ="Ilość musi być większa niż 0!!!!!!!!!!! > __ < ")]		
		public int Quantity { get; set; }
	}
}