using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ConsumeWebApi.Models.product
{
	public class PutProduct
	{
		[Key]
		public int Id { get; set; }
		[MaxLength(50, ErrorMessage = "Enter Name at most 50 character")]
		[MinLength(6, ErrorMessage = "Enter Name At least 6 character ")]

		public DateTime ProductDate { get; set; }

		public string Name { get; set; }
		[MaxLength(50, ErrorMessage = "Enter Description at most 50 character")]
		[MinLength(6, ErrorMessage = "Enter Description At least 6 character ")]
		public string Description { get; set; }
		public decimal price { get; set; }
		[DisplayName("Category")]
		public int? CatagoryId { get; set; }

		[ValidateNever]
		public IFormFile Image { get; set; }
	}
}
