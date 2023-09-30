using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Web_Api_ITI.Models;

namespace ConsumeWebApi.Models.order
{
	public class PutOrder
	{
        [DisplayName("Order No")]
        [Key] public int Id { get; set; }

        [DataType(DataType.Date)]

        [DisplayName("Created at")]
        public DateTime OrderDate { get; set; }
     
        [Required]
        public string Address { get; set; }
        [Required]
        public string Status { get; set; }
        [ValidateNever]

        [DisplayName("Customer")]
        public int CustomerId { get; set; }
        [ValidateNever]
        public List<productOrder> productOrder { get; set; }
    }
}
