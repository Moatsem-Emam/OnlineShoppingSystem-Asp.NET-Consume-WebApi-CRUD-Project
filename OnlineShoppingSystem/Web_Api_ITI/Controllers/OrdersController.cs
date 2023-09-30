using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_Api_ITI.Data;
using Web_Api_ITI.Models;

namespace Web_Api_ITI.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	public class OrdersController : ControllerBase
	{
		private readonly ApplicationDbContext _context;

		public OrdersController(ApplicationDbContext context)
		{
			_context = context;
		}

		// GET: api/Orders
		[HttpGet]
		public async Task<ActionResult<IEnumerable<Order>>> GetOrders(string? search, string? status, string? sortOrder, string? sortType, int pageSize = 10, int pageNumber = 1)
		{
			IQueryable<Order> orders = _context.Orders.AsQueryable();

			if (!string.IsNullOrEmpty(search))
			{
				orders = orders.Where(o => (o.Address.Contains(search)));
			}
			if (status != null && (status == "Pending" || status == "Shipped" || status == "Arrived"))
			{
				orders = orders.Where(o => o.Status == status);
			}
			if (pageSize > 50) pageSize = 50;
			if (pageNumber < 1) pageNumber = 1;
			if (pageSize < 1) pageSize = 1;
			orders = orders.Skip(pageSize * (pageNumber - 1)).Take(pageSize);

			return Ok(orders.Include(o => o.customer).ToList());

		}

		// GET: api/Orders/5
		[HttpGet("{id}")]
		public ActionResult<Order> GetOrder(int id)
		{
			if (_context.Orders == null)
			{
				return NotFound();
			}
			IQueryable< Order> order =  _context.Orders.AsQueryable();

			if (order == null)
			{
				return NotFound();
			}

			order.Include(o => o.customer).Include(o => o.productOrder).ThenInclude(po => po.products).FirstOrDefault(o => o.Id == id);
			return Ok(order);

		}

		// PUT: api/Orders/5
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		[HttpPut]
		public IActionResult PutOrder(Order order)
		{
			ModelState.AddModelError("Null", "The Inserted object is null!");
			if (order == null) return BadRequest(ModelState);

			ModelState.AddModelError("FutureOrderDate", "The Inserted Order Date Cannot be predicted!");
			if (order.OrderDate > DateTime.Now) return BadRequest(ModelState);

			order.UpdatedOrderDate = DateTime.Now;
			_context.Orders.Update(order);
			_context.SaveChanges();

			return NoContent();
		}

		// POST: api/Orders
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		[HttpPost]
		public IActionResult PostOrder(Order order)
		{

			ModelState.AddModelError("FutureOrderDate", "The Inserted Order Date Cannot be predicted!");
			if (order.OrderDate > DateTime.Now) return BadRequest(ModelState);
			
			if (_context.Orders == null)
			{
				return Problem("Entity set 'ApplicationDbContext.Order'  is null.");
			}
			order.OrderDate = DateTime.Now;
			_context.Orders.Add(order);
		    _context.SaveChanges();

			return CreatedAtAction("GetOrder", new { id = order.Id }, order);
		}

		// DELETE: api/Orders/5
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteOrder(int id)
		{
			if (_context.Orders == null)
			{
				return NotFound();
			}
			var order = await _context.Orders.FindAsync(id);
			if (order == null)
			{
				return NotFound();
			}

			_context.Orders.Remove(order);
			await _context.SaveChangesAsync();

			return NoContent();
		}
		[HttpPost]
		public async Task<IActionResult> ChangeStatus(int id, string newStatus)
		{
			// Find the order in the database by its ID
			var order = await _context.Orders.FindAsync(id);

			if (order == null)
			{
				return NotFound(); // order not found
			}

			// Update the status based on the new status received from the button click
			order.Status = newStatus;

			try
			{
				// Save the changes to the database
				await _context.SaveChangesAsync();
				return Ok(); // Status updated successfully
			}
			catch (DbUpdateException)
			{
				// Handle any database update errors here
				return BadRequest("An error occurred while updating the status.");
			}
		}

		private bool OrderExists(int id)
		{
			return (_context.Orders?.Any(e => e.Id == id)).GetValueOrDefault();
		}
	}
}
