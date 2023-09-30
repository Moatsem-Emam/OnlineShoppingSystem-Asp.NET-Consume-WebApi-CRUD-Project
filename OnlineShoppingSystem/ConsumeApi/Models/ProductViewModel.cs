using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ConsumeApi.Models
{
	public class ProductViewModel
	{

		[Key]
		public int Id { get; set; }
		[Required(ErrorMessage = "Enter Valid Name")]
		[MaxLength(20, ErrorMessage = "Enter Name at more 20 character")]
		[MinLength(6, ErrorMessage = "Enter Name At least 6 character ")]

		public string Name { get; set; }
		[Required(ErrorMessage = "Enter Valid Description")]
		[MaxLength(50, ErrorMessage = "Enter Description at more 50 character")]
		[MinLength(6, ErrorMessage = "Enter Description At least 6 character ")]
		public string Description { get; set; }
		public DateTime ProductDate { get; set; }
		public decimal price { get; set; }
		[ValidateNever]
		public DateTime CreatedAt { get; set; }
		[ValidateNever]
		public DateTime LastUpdateAt { get; set; }
		public int? CatagoryId { get; set; }

		[ValidateNever]
		public CategoryViewModel? Catagory { get; set; }
		[ValidateNever]
		public string ImageUrl { get; set; }
		[ValidateNever]
		[NotMapped]
		public IFormFile Image { get; set; }

	}
}
