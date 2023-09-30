using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Web_Api_ITI.Models;

namespace Web_Api_ITI.Models
{
	public class Product
	{
		/// <summary>
		/// A uniqur identity(1,1) identifier
		/// </summary>
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
        [ValidateNever]
        public List<productOrder> productOrder { get; set; }
	}
}