using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Project.Models
{
	public class AppUser : IdentityUser
	{
		[Required(ErrorMessage = "wymagane pole")]
		public required string FirstName { get; set; }
		[Required(ErrorMessage = "wymagane pole")]
		public required string LastName { get; set; }
		
		public ShoppingCart? ShoppingCart { get; set; }
		
	}
}