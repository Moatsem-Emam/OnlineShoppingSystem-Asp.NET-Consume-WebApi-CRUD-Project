using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_Api_ITI.Data;
using Web_Api_ITI.Models;

namespace Web_Api_ITI.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	public class CategoriesController : ControllerBase
	{
		private readonly ApplicationDbContext _context;
		private readonly IWebHostEnvironment _webHostEnvironment;

		//dbContext.Database.CommandTimeout = newTimeoutInSeconds;


		public CategoriesController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
		{
			_context = context;
			_webHostEnvironment = webHostEnvironment;
		}
		/// <summary>
		/// Get Categories Data
		/// </summary>
		/// <returns>Categories context</returns>

		[ProducesResponseType(StatusCodes.Status200OK)]
		[HttpGet]
		public ActionResult GetCategories() => Ok(_context.Categories.Include(c=>c.Products)); // Status Codes: 200 OK - 400 BadRequest - 404 NotFound

		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[HttpGet("{id}")]
		public ActionResult GetCategory(int id)
		{
			Category? cat = _context.Categories.Include(c => c.Products).FirstOrDefault(c=>c.Id==id);
			if (id == 0) return BadRequest();
			if (cat==null) return NotFound();
			return Ok(cat);
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
		/// <summary>
		/// Add Category
		/// </summary>
		/// <param name="cat"></param>
		/// <returns></returns>

		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[HttpPost]
		public ActionResult PostCategory([FromForm] Category cat)
		{
			if (cat == null) return NotFound(); // 400

			ModelState.AddModelError("Duplicated", "This name is registered to another category. Enter a different name.");
			_context.Database.SetCommandTimeout(120);
			if (_context.Categories.Any(c => c.Name == cat.Name)) return BadRequest(ModelState);

			ModelState.AddModelError("Duplicated", "The Inserted Description Cannot be the same as Name!");
			if (cat.Name==cat.Description) return BadRequest(ModelState);

			ModelState.AddModelError("FutureProductionDate", "The Inserted category Date Cannot be predicted!");
			if (cat.CreatedAt > DateTime.Now) return BadRequest(ModelState);

			if (cat.Image != null)
			{
				string imgExtension = Path.GetExtension(cat.Image.FileName);
				Guid imgGuid = Guid.NewGuid();
				string imgName = imgGuid + imgExtension;
				string imgUrl = "\\Images\\" + imgName;
				cat.ImageUrl = imgUrl;

				string imgPath = _webHostEnvironment.WebRootPath + imgUrl;

				FileStream imgStream = new FileStream(imgPath, FileMode.Create);
				cat.Image.CopyTo(imgStream);
				imgStream.Dispose();
			}
			else
			{
				cat.ImageUrl = "\\Images\\No_Image.png";
			}

			cat.CreatedAt = DateTime.Now;

			_context.Categories.Add(cat);
			_context.SaveChanges();

			return CreatedAtAction("GetCategory", new { id = cat.Id }, cat);
		}

		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[HttpPut]
		public ActionResult PutCategories([FromForm] Category cat)
		{
			
           
			if (cat.Name == cat.Description) return BadRequest(ModelState);

			ModelState.AddModelError("FutureProductionDate", "The Inserted category Date Cannot be predicted!");
			if (cat.CreatedAt > DateTime.Now) return BadRequest(ModelState);
            if (cat.Image != null)
            {


                string imgExtension = Path.GetExtension(cat.Image.FileName);
                Guid imgGuid = Guid.NewGuid();
                string imgName = imgGuid + imgExtension;
                string imgUrl = "\\Images\\" + imgName;
                cat.ImageUrl = imgUrl;

                string imgPath = _webHostEnvironment.WebRootPath + imgUrl;

                FileStream imgStream = new FileStream(imgPath, FileMode.Create);
                cat.Image.CopyTo(imgStream);
                imgStream.Dispose();
            }
            else
            {
                cat.ImageUrl = "\\Images\\No_Image.png";
            }
            cat.LastUpdateAt = DateTime.Now;
			_context.Categories.Update(cat);
			_context.SaveChanges();
			return NoContent();

		}

		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[HttpDelete("{id}")]
		public ActionResult DeleteCategory(int id)
		{
			Category? cat = _context.Categories.Find(id);
			ModelState.AddModelError("Null", "You have to insert category id! ");
			if (id == 0) return BadRequest(ModelState);
			if (cat == null) return NotFound();
			if (cat.ImageUrl != "\\images\\No_Image.png")
			{
				string imgPath = _webHostEnvironment.WebRootPath + cat.ImageUrl;
				if (System.IO.File.Exists(imgPath))
				{
					System.IO.File.Delete(imgPath);
				}
			}
			_context.Categories.Remove(cat);
			_context.SaveChanges();
			return NoContent();

		}
	}
}

