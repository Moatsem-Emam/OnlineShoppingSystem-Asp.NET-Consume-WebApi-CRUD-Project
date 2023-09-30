using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Web_Api_ITI.Models;

namespace ConsumeWebApi.Models.order
{
    public class PostOrder
    {




        [Required]
        public string Address { get; set; }

        [Required]
        public string Status { get; set; }

        [ValidateNever]
        [Required(ErrorMessage = "You have to insert Customer!")]
        [DisplayName("Customer")]
        public int CustomerId { get; set; }
    }
}
