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
	public class productOrdersController : ControllerBase
	{
		private readonly ApplicationDbContext _context;

		public productOrdersController(ApplicationDbContext context)
		{
			_context = context;
		}

		// GET: api/productOrders
		[HttpGet]
		public async Task<ActionResult<IEnumerable<productOrder>>> GetproductOrder()
		{
			if (_context.productOrder == null)
			{
				return NotFound();
			}
			return await _context.productOrder.ToListAsync();
		}

		// GET: api/productOrders/5
		[HttpGet("{id}")]
		public async Task<ActionResult<productOrder>> GetproductOrder(int id)
		{
			if (_context.productOrder == null)
			{
				return NotFound();
			}
			var productOrder = await _context.productOrder.FindAsync(id);

			if (productOrder == null)
			{
				return NotFound();
			}

			return productOrder;
		}

		// PUT: api/productOrders/5
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		[HttpPut("{id}")]
		public async Task<IActionResult> PutproductOrder(productOrder productOrder)
		{
			ModelState.AddModelError("Null", "The Inserted object is null!");
			if (productOrder == null) return BadRequest(ModelState);

			_context.productOrder.Update(productOrder);
			_context.SaveChanges();

			return NoContent();
		}

		// POST: api/productOrders
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		[HttpPost]
		public async Task<ActionResult<productOrder>> PostproductOrder(productOrder productOrder)
		{

			ModelState.AddModelError("Null", "The Inserted object is null!");
			if (productOrder == null) return BadRequest(ModelState);

			_context.productOrder.Add(productOrder);
			await _context.SaveChangesAsync();

			return CreatedAtAction("GetproductOrder", new { id = productOrder.Id }, productOrder);
		}

		// DELETE: api/productOrders/5
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteproductOrder(int id)
		{
			if (_context.productOrder == null)
			{
				return NotFound();
			}
			var productOrder = await _context.productOrder.FindAsync(id);
			if (productOrder == null)
			{
				return NotFound();
			}

			_context.productOrder.Remove(productOrder);
			await _context.SaveChangesAsync();

			return NoContent();
		}

		private bool productOrderExists(int id)
		{
			return (_context.productOrder?.Any(e => e.Id == id)).GetValueOrDefault();
		}
	}
}
