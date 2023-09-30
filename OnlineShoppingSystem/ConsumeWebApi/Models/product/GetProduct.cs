using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Web_Api_ITI.Models;
using System.ComponentModel;

namespace ConsumeWebApi.Models.product
{
    public class GetProduct
    {

		[Key]
		public int Id { get; set; }
		[MaxLength(50, ErrorMessage = "Enter Name at most 50 character")]
		[MinLength(6, ErrorMessage = "Enter Name At least 6 character ")]

		public string Name { get; set; }
		[MaxLength(50, ErrorMessage = "Enter Description at most 50 character")]
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
		public Category? Catagory { get; set; }

		[ValidateNever]
		public string ImageUrl { get; set; }
		[ValidateNever]
		[NotMapped]
		public IFormFile Image { get; set; }
		public List<productOrder> productOrder { get; set; }
	}
}
