using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Web_Api_ITI.Models;
using System.Text.Json.Serialization;
using System.ComponentModel;

namespace ConsumeWebApi.Models.product
{
    public class PostProduct
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
		[Required(ErrorMessage ="You have to insert Category!")]
		[DisplayName("Category")]
		
		public int CatagoryId { get; set; }

		[ValidateNever]
        public IFormFile Image { get; set; }
    }
}
