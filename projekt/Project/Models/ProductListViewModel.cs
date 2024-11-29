using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Project.Helpers;
using Project.Specifications;

namespace Project.Models
{
	public class ProductListViewModel
	{
		public Pagination<Product>? Pagination { get; set; }
		public SelectList? Brands { get; set; }
		public SelectList? Types { get; set; }
		public SelectList? Categories { get; set; }
		public ProductSpecParams? SpecParams { get; set; }
	}
}