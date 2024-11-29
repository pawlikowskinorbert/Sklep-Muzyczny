using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Models
{
	public class RegisterViewModel
	{
		
		[Required]
		public required string UserName { get; set; }
		
		[Required]
		[EmailAddress]
		public required string Email { get; set; }
		
		[Required]
		public required string FirstName { get; set; }
		
		[Required]
		public required string LastName { get; set; }
		
		
		
		[Required]
		[DataType(DataType.Password)]
		public required string Password { get; set; }
		
		[Required]
		[DataType(DataType.Password)]
		[Compare("Password", ErrorMessage = "Passwords doesn not match")]
		public required string ConfirmPassword { get; set; }
		
		
	}
}