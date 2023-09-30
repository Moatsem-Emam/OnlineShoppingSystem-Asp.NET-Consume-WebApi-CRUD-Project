using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Diagnostics.CodeAnalysis;
using System.Drawing.Imaging;
using System.Drawing;
using Web_Api_ITI.Data;
using Web_Api_ITI.Models;

namespace Web_Api_ITI.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	public class ProductsController : ControllerBase
	{
		private readonly ApplicationDbContext _context;
		private readonly IWebHostEnvironment _webHostEnvironment;
		public ProductsController(ApplicationDbContext context, IWebHostEnvironment WebHostEnvironment)
		{
			_context = context;
			_webHostEnvironment = WebHostEnvironment;
		}
		/// <summary>
		/// Get Products Data
		/// </summary>
		/// <returns>Product Object</returns>

		[ProducesResponseType(StatusCodes.Status200OK)]
		[HttpGet]
		public ActionResult GetProducts(string? search, int catId, string? sortOrder, string? sortType, int pageSize = 20, int pageNumber = 1)
		{
			IQueryable<Product> prods = _context.Products.AsQueryable();
			if (catId != 0)
			{
				prods = prods.Where(p => p.CatagoryId == catId);
			}
			if (!string.IsNullOrEmpty(search))
			{
				prods = prods.Where(p => (p.Name.Contains(search) || p.Description.Contains(search)));
			}

			if (pageSize > 50) pageSize = 50;
			if (pageNumber < 1) pageNumber = 1;
			if (pageSize < 1) pageSize = 1;
			prods = prods.Skip(pageSize * (pageNumber - 1)).Take(pageSize);

			if (sortType == "Name" && sortOrder == "asc")
			{
				prods = prods.OrderBy(p => p.Name);
			}
			else if (sortType == "Name" && sortOrder == "desc")
			{
				prods = prods.OrderByDescending(p => p.Name);
			}
			else if (sortType == "Description" && sortOrder == "asc")
			{
				prods = prods.OrderBy(p => p.Description);
			}
			else if (sortType == "Description" && sortOrder == "desc")
			{
				prods = prods.OrderByDescending(p => p.Description);
			}

			return Ok(prods.Include(p => p.Catagory).ToList());
		}

		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[HttpGet("{id}")]
		public ActionResult GetProduct(int id)
		{
			Product? prod = _context.Products.Include(p => p.Catagory).FirstOrDefault(p => p.Id == id);
			ModelState.AddModelError("Null", "You have to insert product id! ");
			if (id == 0) return BadRequest(ModelState);
			if (prod == null) return NotFound();
			return Ok(prod);

		}

		/// <summary>
		/// Add Products
		/// </summary>
		/// <param name="prod"></param>
		/// <returns></returns>

		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[HttpPost]
		public ActionResult PostProducts([FromForm] Product prod)
		{
			ModelState.AddModelError("Null", "The Inserted object is null!");
			if (prod == null) return BadRequest(ModelState);

			ModelState.AddModelError("Duplicated", "The Inserted Name is allready exists!");
			if (_context.Products.Any(p => p.Name == prod.Name)) return BadRequest(ModelState);

			ModelState.AddModelError("Duplicated", "The Inserted Description Cannot be the same as Name!");
			if (prod.Description == prod.Name) return BadRequest(ModelState);

			ModelState.AddModelError("FutureProductionDate", "The Inserted Product Date Cannot be predicted!");
			if (prod.ProductDate > DateTime.Now) return BadRequest(ModelState);

			if (prod.Image != null)
			{
				string imgExtension = Path.GetExtension(prod.Image.FileName);
				Guid imgGuid = Guid.NewGuid();
				string imgName = imgGuid + imgExtension;
				string imgUrl = "\\Images\\" + imgName;
				prod.ImageUrl = imgUrl;

				string imgPath = _webHostEnvironment.WebRootPath + imgUrl;

				FileStream imgStream = new FileStream(imgPath, FileMode.Create);
				prod.Image.CopyTo(imgStream);
				imgStream.Dispose();
			}
			else
			{
				prod.ImageUrl = "\\Images\\No_Image.png";
			}
			prod.CreatedAt = DateTime.Now;
			_context.Products.Add(prod);
			_context.SaveChanges();
			return Ok(prod);
			//if (prod.Image != null)
			//{
			//	string imgExtension = Path.GetExtension(prod.Image.FileName);
			//	Guid imgGuid = Guid.NewGuid();
			//	string imgName = imgGuid + imgExtension;
			//	string imgUrl = "\\Images\\" + imgName;
			//	prod.ImageUrl = imgUrl;

			//	// Save the image to the MVC application's wwwroot directory
			//	string mvcWwwRootPath = Path.Combine(_webHostEnvironment.WebRootPath, "Images");
			//	Directory.CreateDirectory(mvcWwwRootPath);
			//	string imgPath = Path.Combine(mvcWwwRootPath, imgName);

			//	using (var stream = new FileStream(imgPath, FileMode.Create))
			//	{
			//		prod.Image.CopyTo(stream);
			//	}
			//}
			//else
			//{
			//	prod.ImageUrl = "\\Images\\No_Image.png";
			//}

			//prod.CreatedAt = DateTime.Now;
			//_context.Products.Add(prod);
			//_context.SaveChanges();
			////return CreatedAtAction("GetProduct",new{ id = prod.Id},prod);
			//return Ok(prod.ImageUrl);

		}
		[HttpGet("{imageName}")]
		public async Task<IActionResult> GetImage(string imageName)
		{
			try
			{
				// Construct the full path to the wwwroot folder
				var wwwrootPath = Path.Combine(_webHostEnvironment.WebRootPath, "images");

				// Construct the full path to the image file
				var imagePath = Path.Combine(wwwrootPath, imageName);

				// Check if the image file exists
				if (System.IO.File.Exists(imagePath))
				{
					// Read the image file into a byte array
					byte[] imageBytes = await System.IO.File.ReadAllBytesAsync(imagePath);

					// Determine the content type based on the file extension
					string contentType = GetContentType(imageName); // Adjust as needed

					return File(imageBytes, contentType);
				}
				else
				{
					return NotFound("Image not found");
				}
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}
		private string GetContentType(string fileName)
		{
			// Determine the content type based on the file extension
			string[] validImageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };

			string fileExtension = Path.GetExtension(fileName).ToLower();

			if (validImageExtensions.Contains(fileExtension))
			{
				switch (fileExtension)
				{
					case ".jpg":
					case ".jpeg":
						return "image/jpeg";
					case ".png":
						return "image/png";
					case ".gif":
						return "image/gif";
					case ".bmp":
						return "image/bmp";
						// Add more cases for other image types if needed
				}
			}

			// Return null for unsupported image types
			return null;
		}


		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[HttpPut]
		public ActionResult PutProducts([FromForm] Product prod)
		{
			ModelState.AddModelError("Null", "The Inserted object is null!");
			if (prod == null) return BadRequest(ModelState);


			ModelState.AddModelError("Duplicated", "The Inserted Description Cannot be the same as Name!");
			if (prod.Description == prod.Name) return BadRequest(ModelState);

			ModelState.AddModelError("FutureProductionDate", "The Inserted Product Date Cannot be predicted!");
			if (prod.ProductDate > DateTime.Now) return BadRequest(ModelState);

			if (prod.ImageUrl != "\\images\\No_Image.png")
			{
				string imgPath = _webHostEnvironment.WebRootPath + prod.ImageUrl;
				if (System.IO.File.Exists(imgPath))
				{
					System.IO.File.Delete(imgPath);
				}
			}
			if (prod.Image != null)
			{


				string imgExtension = Path.GetExtension(prod.Image.FileName);
				Guid imgGuid = Guid.NewGuid();
				string imgName = imgGuid + imgExtension;
				string imgUrl = "\\Images\\" + imgName;
				prod.ImageUrl = imgUrl;

				string imgPath = _webHostEnvironment.WebRootPath + imgUrl;

				FileStream imgStream = new FileStream(imgPath, FileMode.Create);
				prod.Image.CopyTo(imgStream);
				imgStream.Dispose();
			}
			else
			{
				prod.ImageUrl = "\\Images\\No_Image.png";
			}

			prod.LastUpdateAt = DateTime.Now;
			_context.Products.Update(prod);
			_context.SaveChanges();
			return NoContent();

		}

		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[HttpDelete("{id}")]
		public ActionResult DeleteProduct(int id)
		{
			Product? prod = _context.Products.Find(id);
			ModelState.AddModelError("Null", "You have to insert product id! ");
			if (id == 0) return BadRequest(ModelState);
			if (prod == null) return NotFound();
			if (prod.ImageUrl != "\\images\\No_Image.png")
			{
				string imgPath = _webHostEnvironment.WebRootPath + prod.ImageUrl;
				if (System.IO.File.Exists(imgPath))
				{
					System.IO.File.Delete(imgPath);
				}
			}
			_context.Products.Remove(prod);
			_context.SaveChanges();
			return NoContent();

		}
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[HttpDelete]
		public IActionResult DeleteAllProducts()
		{
			try
			{
				// Retrieve all products from the database
				var allProducts = _context.Products.ToList();

				// Check if there are any products to delete
				if (allProducts == null || allProducts.Count == 0)
				{
					return NotFound("No products found to delete.");
				}

				// Delete each product one by one
				foreach (var product in allProducts)
				{
					_context.Products.Remove(product);
				}

				// Save changes to the database
				_context.SaveChanges();

				// Return a success response
				return NoContent(); // HTTP 204 No Content
			}
			catch (Exception ex)
			{
				// Handle any exceptions that may occur during the deletion process
				return StatusCode(500, $"An error occurred: {ex.Message}");
			}
		}

	}
}
