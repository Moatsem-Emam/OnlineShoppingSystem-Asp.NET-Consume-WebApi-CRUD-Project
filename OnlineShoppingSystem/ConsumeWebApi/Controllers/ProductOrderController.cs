using ConsumeWebApi.Models.category;
using ConsumeWebApi.Models.order;
using ConsumeWebApi.Models.product;
using ConsumeWebApi.Models.ProductOrders;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using System.Drawing.Printing;
using System.Net.Http;
using System.Net.Http.Json;
using Web_Api_ITI.Models;

namespace ConsumeWebApi.Controllers
{
	public class ProductOrderController : Controller
	{
		Uri baseAddress = new Uri("https://localhost:7037/api/");
		private readonly HttpClient _httpClient;
		public IWebHostEnvironment _webHostEnvironment;
		public ProductOrderController()
		{
			_httpClient = new HttpClient();
			_httpClient.BaseAddress = baseAddress;
		}

		// GET: ProductOrders
		public ActionResult Index()
		{
			List<ProductOrdersViewModel> items = new List<ProductOrdersViewModel>();
			HttpResponseMessage response = _httpClient.GetAsync($"{_httpClient.BaseAddress}productOrders/GetproductOrder").Result;
			if (response.IsSuccessStatusCode)
			{
				string data = response.Content.ReadAsStringAsync().Result;
				items = JsonConvert.DeserializeObject<List<ProductOrdersViewModel>>(data);

			}
			return View(items);
		}

		// GET: ProductOrders/Details/5
		public ActionResult Details(int id)
		{
			ProductOrdersViewModel items = new ProductOrdersViewModel();
			HttpResponseMessage response = _httpClient.GetAsync($"{_httpClient.BaseAddress}productOrders/GetproductOrder/{id}").Result;
			if (response.IsSuccessStatusCode)
			{
				string data = response.Content.ReadAsStringAsync().Result;
				items = JsonConvert.DeserializeObject<ProductOrdersViewModel>(data);

			}
			ViewBag.items = items;
			return View("Details", items);
		}

		// GET: ProductOrders/Create
		public ActionResult Create()
		{
            List<GetOrder> orders = new List<GetOrder>();
            HttpResponseMessage Ordresponse = _httpClient.GetAsync($"{_httpClient.BaseAddress}Orders/GetOrders").Result;
            if (Ordresponse.IsSuccessStatusCode)
            {
                string data = Ordresponse.Content.ReadAsStringAsync().Result;
                orders = JsonConvert.DeserializeObject<List<GetOrder>>(data);

            }
            ViewBag.Orders = new SelectList(orders, "Id", "Id");

            List<GetProduct> products = new List<GetProduct>();
            HttpResponseMessage Prodresponse = _httpClient.GetAsync($"{_httpClient.BaseAddress}Products/GetProducts").Result;
            if (Prodresponse.IsSuccessStatusCode)
            {
                string data = Prodresponse.Content.ReadAsStringAsync().Result;
                products = JsonConvert.DeserializeObject<List<GetProduct>>(data);

            }
            ViewBag.Products = new SelectList(products, "Id", "Name");

            return View();

		}

		// POST: ProductOrders/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(ProductOrdersViewModel PO)
		{
			try
			{
				var postTask = _httpClient.PostAsJsonAsync<ProductOrdersViewModel>("productOrders/PostproductOrder", PO);
				postTask.Wait();
				var result = postTask.Result;
				if (result.IsSuccessStatusCode)
				{
					TempData["successMessage"] = "Created successfully, Add more!";
					return RedirectToAction("Create");
				}
				else
				{
					// Handle API error

					var errorMessage = result.Content.ReadAsStringAsync().Result;
					TempData["errorMessage"] = errorMessage;
				}
			}
			catch (Exception ex)
			{
				// Handle any exceptions
				ModelState.AddModelError(string.Empty, "An error occurred: " + ex.Message);
			}
			return View(PO);
		}

		// GET: ProductOrders/Edit/5
		public ActionResult Edit(int id)
		{
			ProductOrdersViewModel item = new ProductOrdersViewModel();
			HttpResponseMessage response = _httpClient.GetAsync($"{_httpClient.BaseAddress}productOrders/GetproductOrder/{id}").Result;
			if (response.IsSuccessStatusCode)
			{
				string data = response.Content.ReadAsStringAsync().Result;
				item = JsonConvert.DeserializeObject<ProductOrdersViewModel>(data);

			}
			return View(item);
		}

		// POST: ProductOrders/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(ProductOrdersViewModel productOrder)
		{
			try
			{
				var putTask = _httpClient.PutAsJsonAsync<ProductOrdersViewModel>("productOrders/PutproductOrder", productOrder);
				putTask.Wait();
				var result = putTask.Result;
				if (result.IsSuccessStatusCode)
				{
					TempData["successMessage"] = $"productOrder Id:{productOrder.Id} Updated successfully.";
					return RedirectToAction("Index");
				}
				else
				{
					// Handle API error

					var errorMessage = result.Content.ReadAsStringAsync().Result;
					TempData["errorMessage"] = errorMessage;
				}
			}
			catch (Exception ex)
			{
				// Handle any exceptions
				ModelState.AddModelError(string.Empty, "An error occurred: " + ex.Message);
			}
			return View(productOrder);
		}


		// GET: ProductOrders/Delete/5
		public ActionResult GetDelete(int id)
		{
			ProductOrdersViewModel Delitem = new ProductOrdersViewModel();
			HttpResponseMessage response = _httpClient.GetAsync($"{_httpClient.BaseAddress}productOrders/GetproductOrder/{id}").Result;
			if (response.IsSuccessStatusCode)
			{
				string data = response.Content.ReadAsStringAsync().Result;
				Delitem = JsonConvert.DeserializeObject<ProductOrdersViewModel>(data);

			}
			ViewBag.items = Delitem;
			return View("Delete",Delitem);
		}

		// POST: ProductOrders/Delete/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Delete(int id)
		{
			HttpResponseMessage DeleteResponse =  _httpClient.DeleteAsync($"{_httpClient.BaseAddress}Products/DeleteProduct/{id}").Result;
			if (DeleteResponse.IsSuccessStatusCode)
			{
				TempData["successMessage"] = $"productOrder Id:{id} Deleted successfully.";
				return RedirectToAction("Index");
			}
			TempData["errorMessage"] = "Delation Failed.";
			return View("Delete");
		}
	}

}