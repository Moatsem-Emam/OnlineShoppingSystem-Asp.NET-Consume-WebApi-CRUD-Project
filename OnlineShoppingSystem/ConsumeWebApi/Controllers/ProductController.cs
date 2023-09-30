using Azure;
using ConsumeWebApi.Models.category;
using ConsumeWebApi.Models.product;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Drawing.Printing;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Web_Api_ITI.Models;

namespace ConsumeWebApi.Controllers
{
    public class ProductController : Controller
    {
        Uri baseAddress = new Uri("https://localhost:7037/api/"); 
		private readonly HttpClient _httpClient;
        public IWebHostEnvironment _webHostEnvironment;

        public ProductController(IWebHostEnvironment webHostEnvironment)
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = baseAddress;
            _webHostEnvironment = webHostEnvironment;
		}

        //------------ Get Index View - Products Data -------------
        public IActionResult Index(string search, int catId, string sortOrder, string sortType, int pageSize = 10, int pageNumber = 1)
        {
            ViewBag.CurrentSearch = search;
            ViewBag.CatId = catId;
            ViewBag.SortOrder = sortOrder;
            ViewBag.SortType = sortType;
			ViewBag.pageSize = pageSize;
            ViewBag.pageNumber = pageNumber;

			List<GetProduct> products = new List<GetProduct>();
            HttpResponseMessage response = _httpClient.GetAsync($"{_httpClient.BaseAddress}Products/GetProducts?search={search}&CatId={catId}&sortOrder={sortOrder}&sortType={sortType}&pageSize={pageSize}&pageNumber={pageNumber}").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                products = JsonConvert.DeserializeObject<List<GetProduct>>(data);

            }
			HttpResponseMessage categoryResponse = _httpClient.GetAsync($"{_httpClient.BaseAddress}Categories/GetCategories").Result;
			List<GetCategory> categories = new List<GetCategory>();

			if (categoryResponse.IsSuccessStatusCode)
			{
				string categoryData = categoryResponse.Content.ReadAsStringAsync().Result;
				categories = JsonConvert.DeserializeObject<List<GetCategory>>(categoryData);
			}
			SelectList categoryList = new SelectList(categories, "Id", "Name", ViewBag.CatId);
			ViewBag.AllCategories = categoryList;

			int totalCount = products.Count(); // Calculate the total count
			ViewBag.totalCount = totalCount; // Pass it to the view

			return View(products);
        }

		//------------ Specific Product Data -------------
		public async Task<IActionResult> Details(int id)
        {
			GetProduct products = new GetProduct();
			HttpResponseMessage response = _httpClient.GetAsync($"{_httpClient.BaseAddress}Products/GetProduct/{id}").Result;
			if (response.IsSuccessStatusCode)
			{
				string data = response.Content.ReadAsStringAsync().Result;
				products = JsonConvert.DeserializeObject<GetProduct>(data);

			}
			ViewBag.SpecificProd= products;
			return View("Details",products);
		}
		//------------ Delete Product View -------------
		public IActionResult GetDelete(int id) 
		{
			GetProduct products = new GetProduct();
			HttpResponseMessage response = _httpClient.GetAsync($"{_httpClient.BaseAddress}Products/GetProduct/{id}").Result;
			if (response.IsSuccessStatusCode)
			{
				string data = response.Content.ReadAsStringAsync().Result;
				products = JsonConvert.DeserializeObject<GetProduct>(data);

			}
			ViewBag.SpecificProd = products;
			return View("Delete", products);
		}
		[HttpPost]
		public async Task<IActionResult> Delete(int id)
		{
			HttpResponseMessage DeleteResponse = await _httpClient.DeleteAsync($"{_httpClient.BaseAddress}Products/DeleteProduct/{id}");
			if (DeleteResponse.IsSuccessStatusCode)
			{
				TempData["successMessage"] = "Product Deleted successfully.";
                return RedirectToAction("Index");
            }
            TempData["errorMessage"] = "Delation Failed.";
			return View("Delete");
        }
		[HttpPost]
		public async Task<IActionResult> DeleteAll()
		{
			HttpResponseMessage DeleteAllResponse = await _httpClient.DeleteAsync($"{_httpClient.BaseAddress}Products/DeleteAllProducts/");
			if (DeleteAllResponse.IsSuccessStatusCode)
			{
				TempData["successMessage"] = "All Products Deleted successfully.";
				return RedirectToAction("Index");
			}
			TempData["errorMessage"] = "Delation Failed.";
			return View("Index");
		}
		//------------ Get Create view -------------
		public IActionResult Create()
        {
            HttpResponseMessage categoryResponse = _httpClient.GetAsync($"{_httpClient.BaseAddress}Categories/GetCategories").Result;
            List<GetCategory> categories = new List<GetCategory>();

            if (categoryResponse.IsSuccessStatusCode)
            {
                string categoryData = categoryResponse.Content.ReadAsStringAsync().Result;
                categories = JsonConvert.DeserializeObject<List<GetCategory>>(categoryData);
            }
            SelectList categoryList = new SelectList(categories, "Id", "Name");
            ViewBag.Categories = categoryList;
            return View();

        }
		//------------ Action create -------------
		[HttpPost]
        public async Task<IActionResult> Create(PostProduct model)
        {

            try
            {
                // Create a new instance of MultipartFormDataContent
                MultipartFormDataContent multipartFormDataContent = new MultipartFormDataContent();
                // Add properties to the multipartFormDataContent
                multipartFormDataContent.Add(new StringContent(model.Name, Encoding.UTF8), "Name");
                multipartFormDataContent.Add(new StringContent(model.Description, Encoding.UTF8), "Description");
                multipartFormDataContent.Add(new StringContent(model.ProductDate.ToString("yyyy-MM-dd HH:mm:ss"), Encoding.UTF8), "ProductDate");
                multipartFormDataContent.Add(new StringContent(model.price.ToString(), Encoding.UTF8), "price");
                multipartFormDataContent.Add(new StringContent(model.CatagoryId.ToString(), Encoding.UTF8), "CatagoryId");
                if (model.Image != null)
                {

                    var imageContent = new StreamContent(model.Image.OpenReadStream());
                    imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse(model.Image.ContentType);
                    multipartFormDataContent.Add(imageContent, "Image", model.Image.FileName);

                    // Send the POST request to the API

                }

                var response = await _httpClient.PostAsync("Products/PostProducts", multipartFormDataContent);
                if (response.IsSuccessStatusCode)
                {

					// Product created successfully
					TempData["successMessage"] = "Product created successfully";
					return RedirectToAction("Index"); // Redirect to a success page
                }
                else
                {
                    // Handle API error

                    var errorMessage = response.Content.ReadAsStringAsync().Result;
                    TempData["errorMessage"] = errorMessage;
                }
               
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                ModelState.AddModelError(string.Empty, "An error occurred: " + ex.Message);
            }


            // If we get here, there was a validation error or an API error
            return View(model);
        }
        
        //------------ Get Edit View -------------
        public IActionResult Edit(int id)
        {
            PostProduct product = new PostProduct();
			HttpResponseMessage response = _httpClient.GetAsync($"{_httpClient.BaseAddress}Products/GetProduct/{id}").Result;
			if (response.IsSuccessStatusCode)
			{
				string data = response.Content.ReadAsStringAsync().Result;
				product = JsonConvert.DeserializeObject<PostProduct>(data);

			}
            ViewBag.image = product.Image;

            HttpResponseMessage categoryResponse = _httpClient.GetAsync($"{_httpClient.BaseAddress}Categories/GetCategories").Result;
            List<GetCategory> categories = new List<GetCategory>();

            if (categoryResponse.IsSuccessStatusCode)
            {
                string categoryData = categoryResponse.Content.ReadAsStringAsync().Result;
                categories = JsonConvert.DeserializeObject<List<GetCategory>>(categoryData);
            }
            SelectList categoryList = new SelectList(categories, "Id", "Name");
            ViewBag.Categories = categoryList;
            return View(product);
		}
		//------------ Action Edit -------------
		[HttpPost]
        public IActionResult Edit(PostProduct model)
        {
			try
			{
				// Create a new instance of MultipartFormDataContent
				MultipartFormDataContent multipartFormDataContent = new MultipartFormDataContent();
                // Add properties to the multipartFormDataContent
                multipartFormDataContent.Add(new StringContent(model.Id.ToString(), Encoding.UTF8), "Id");
                multipartFormDataContent.Add(new StringContent(model.Name, Encoding.UTF8), "Name");
				multipartFormDataContent.Add(new StringContent(model.Description, Encoding.UTF8), "Description");
				multipartFormDataContent.Add(new StringContent(model.ProductDate.ToString("yyyy-MM-dd"), Encoding.UTF8), "ProductDate");
				multipartFormDataContent.Add(new StringContent(model.price.ToString(), Encoding.UTF8), "price");
				multipartFormDataContent.Add(new StringContent(model.CatagoryId.ToString(), Encoding.UTF8), "CatagoryId");
				if (model.Image != null)
				{

					var imageContent = new StreamContent(model.Image.OpenReadStream());
					imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse(model.Image.ContentType);
					multipartFormDataContent.Add(imageContent, "Image", model.Image.FileName);

					// Send the PUT request to the API

				}

				var response =  _httpClient.PutAsync("Products/PutProducts", multipartFormDataContent).Result;
				if (response.IsSuccessStatusCode)
				{

					// Product Updated successfully
					return RedirectToAction("Index"); // Redirect to a success page
				}
				else
				{
					// Handle API error

					var errorMessage = response.Content.ReadAsStringAsync().Result;
					TempData["errorMessage"] = errorMessage;
				}

			}
			catch (Exception ex)
			{
				// Handle any exceptions
				ModelState.AddModelError(string.Empty, "An error occurred: " + ex.Message);
			}


			// If we get here, there was a validation error or an API error
			return View(model);
		}
     
    }

}
