using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Project.Models
{
	public class ProductEditViewModel
	{
		public int Id { get; set; }

		[Required(ErrorMessage = "Nazwa jest wymagana")]
		public string Name { get; set; }

		[Required(ErrorMessage = "Opis jest wymagany")]
		public string Description { get; set; }

		[Display(Name = "Zdjęcie Produktu")]
		[DataType(DataType.Upload)]
		public IFormFile? Photo { get; set; }

		[Required(ErrorMessage = "Cena jest wymagana")]
		[Range(0.01, double.MaxValue, ErrorMessage = "Cena musi być większa od zera")]
		[DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
		public decimal Price { get; set; }

		public int QuantityInStock { get; set; }

		public int ProductTypeId { get; set; }
		public SelectList? ProductTypes { get; set; }

		public int BrandId { get; set; }
		public SelectList? Brands { get; set; }

		public List<SpecificationDetail> SpecificationDetails { get; set; } = new();
	}

}