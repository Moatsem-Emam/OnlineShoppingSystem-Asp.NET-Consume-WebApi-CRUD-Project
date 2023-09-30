using Microsoft.EntityFrameworkCore;
using Web_Api_ITI.Models;

namespace Web_Api_ITI.Data
{
    public class ApplicationDbContext : DbContext
    {
		public DbSet<Category> Categories { get; set; }
		public DbSet<Product> Products { get; set; }
		public DbSet<Customer> Customers { get; set; }
		public DbSet<Order> Orders { get; set; }



		public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }



		public DbSet<Web_Api_ITI.Models.productOrder> productOrder { get; set; } = default!;


		//public DbSet<Web_Api_ITI.Models.Order> Order { get; set; } = default!;

    }
}
