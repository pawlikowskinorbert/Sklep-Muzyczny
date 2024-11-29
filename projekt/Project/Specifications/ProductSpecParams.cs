using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace Project.Specifications
{
	public class ProductSpecParams
	{
		private const int MaxPageSize = 50;
		public int PageIndex { get; set; } = 1;
		private int _pageSize = 6;
		public int PageSize
		{
			get => _pageSize;
			set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
		}



		private List<int> _brands = [];
		public List<int> Brands
		{
			get => _brands;
			set => _brands = value ?? new List<int>();
		}

		private List<int> _types = [];
		public List<int> Types
		{
			get => _types;
			set => _types = value ?? new List<int>();
		}

		[Display(Name = "Brand")]
		public int? BrandId { get; set; }

		[Display(Name = "Type")]
		public int? TypeId { get; set; }
		
		public int? CategoryId { get; set; }


		public string? Sort { get; set; }


		private string? _search;
		public string Search
		{
			get => _search ?? "";
			set => _search = value.ToLower();
		}


	}
}