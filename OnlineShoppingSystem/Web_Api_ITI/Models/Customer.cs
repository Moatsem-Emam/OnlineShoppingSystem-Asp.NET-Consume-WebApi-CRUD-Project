using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Web_Api_ITI.Models
{
	public class Customer
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Email { get; set; }
		public string Phone { get; set; }
        [ValidateNever]
        public List<Order> orders { get; set; }

    }
}
