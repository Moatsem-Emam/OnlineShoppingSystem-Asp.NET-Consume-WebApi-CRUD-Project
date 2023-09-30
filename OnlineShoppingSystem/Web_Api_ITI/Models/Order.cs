using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Web_Api_ITI.Models
{
	public class Order
	{
		[DisplayName("Order No")]
		[Key] public int Id { get; set; }
      
		[DataType(DataType.Date)]

        [DisplayName("Created At")]
        public DateTime OrderDate { get; set; }
		[DataType(DataType.Date)]

        [DisplayName("Last Updated")]
        public DateTime UpdatedOrderDate { get; set; }
        [Required]
        public string Address { get; set; }
		[Required]
		public string Status  { get; set; }
        [ValidateNever]

        [DisplayName("Customer")]
        public int CustomerId { get; set; }
        public Customer? customer { get; set; }
		[ValidateNever]
		public List<productOrder> productOrder { get; set; }
    }
}
