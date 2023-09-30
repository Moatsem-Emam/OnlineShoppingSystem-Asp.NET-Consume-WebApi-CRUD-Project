using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Web_Api_ITI.Models;

namespace ConsumeWebApi.Models.customer
{
    public class CustomerViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
		public string Phone { get; set; }
        [ValidateNever]
        public List<Order> orders { get; set; }


    }
}
