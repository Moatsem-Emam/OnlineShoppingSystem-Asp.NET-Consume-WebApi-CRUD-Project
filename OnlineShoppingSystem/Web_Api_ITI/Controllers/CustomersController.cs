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
    public class CustomersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CustomersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Customers
        [HttpGet]
        public ActionResult GetCustomers()
        {
            IQueryable<Customer> customer = _context.Customers.AsQueryable();
            if (customer == null)
          {
              return NotFound();
          }
            return Ok(customer);
        }

        // GET: api/Customers/5
        [HttpGet("{id}")]
        public ActionResult GetCustomer(int id)
        {
            IQueryable<Customer> customer=  _context.Customers.AsQueryable();
          if (_context.Customers == null)
          {
              return NotFound();
          }

            if (customer == null)
            {
                return NotFound();
            }
            customer.Include(c => c.orders).FirstOrDefault(c => c.Id == id);
            return Ok(customer);
        }

        // PUT: api/Customers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        public IActionResult PutCustomer(Customer customer)
        {
            ModelState.AddModelError("Null", "The Inserted object is null!");
            if (customer == null) return BadRequest(ModelState);

            _context.Customers.Update(customer);
            _context.SaveChanges();

            return NoContent();
        }

        // POST: api/Customers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Customer>> PostCustomer(Customer customer)
        {
          if (_context.Customers == null)
          {
              return Problem("Entity set 'ApplicationDbContext.customers'  is null.");
          }
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCustomer", new { id = customer.Id }, customer);
        }

        // DELETE: api/Customers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            if (_context.Customers == null)
            {
                return NotFound();
            }
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CustomerExists(int id)
        {
            return (_context.Customers?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
