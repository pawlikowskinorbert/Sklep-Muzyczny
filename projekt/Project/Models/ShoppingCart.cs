using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Models
{
	public class ShoppingCart : BaseEntity
	{
		public string? AppUserId { get; set; }
		public AppUser? AppUser { get; set; }
		
		public ICollection<ShoppingCartItem>? ShoppingCartItems { get; set; }
	}
}