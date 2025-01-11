using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Models
{
	public class Order : BaseEntity
	{
		public string UserId { get; set; }
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public decimal TotalAmount { get; set; }
		public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
		public string Status { get; set; } = "Zatwierdzone";
	}
}