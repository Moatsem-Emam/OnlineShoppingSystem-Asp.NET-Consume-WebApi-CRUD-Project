namespace Web_Api_ITI.Models
{
	public class productOrder
	{
		public int Id { get; set; }
        public int ProductId { get; set; }	
        public Product? products { get; set; }
		public int OrderId { get; set; }
		public Order? orders { get; set; }

	}
}
