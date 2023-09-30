using ConsumeWebApi.Models.customer;
using ConsumeWebApi.Models.order;
using ConsumeWebApi.Models.ProductOrders;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Drawing.Printing;
using System.Net;
using System.Net.Http;
using System.Text;
using Web_Api_ITI.Models;

namespace ConsumeWebApi.Controllers
{
    public class CustomerController : Controller
	{
		Uri baseAddress = new Uri("https://localhost:7037/api/");
		private readonly HttpClient _httpClient;
		public IWebHostEnvironment _webHostEnvironment;

		public CustomerController()
		{
			_httpClient = new HttpClient();
			_httpClient.BaseAddress = baseAddress;
		}

		// GET: CustomerController
		public IActionResult Index()
		{
			List<CustomerViewModel> customers = new List<CustomerViewModel>();
			HttpResponseMessage response = _httpClient.GetAsync($"{_httpClient.BaseAddress}Customers/GetCustomers").Result;
			if (response.IsSuccessStatusCode)
			{
				string data = response.Content.ReadAsStringAsync().Result;
				customers = JsonConvert.DeserializeObject<List<CustomerViewModel>>(data);
			}
			return View(customers);

		}

		// GET: CustomerController/Details/5
		public IActionResult Details(int id)
		{
            CustomerViewModel customer = new CustomerViewModel();
            HttpResponseMessage response = _httpClient.GetAsync($"{_httpClient.BaseAddress}Customers/GetCustomer/{id}").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                var jsonArray = JArray.Parse(data);
                if (jsonArray.Count > 0)
                {
                    customer = JsonConvert.DeserializeObject<CustomerViewModel>(jsonArray[0].ToString());
                }
            }
			ViewBag.customer = customer;
            return View("Details",customer);
        }

		// GET: CustomerController/Create
		public IActionResult Create()
		{
			return View();
		}

		// POST: CustomerController/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Create(CustomerViewModel customer)
		{

			var postTask = _httpClient.PostAsJsonAsync<CustomerViewModel>("Customers/PostCustomer", customer);
			postTask.Wait();
			var result = postTask.Result;
			if (result.IsSuccessStatusCode)
			{
				return RedirectToAction("Index"); 
			}

			ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator.");
			return View(customer);

		}

		// GET: CustomerController/Edit/5
		public IActionResult Edit(int id)
		{
            CustomerViewModel customer = new CustomerViewModel();
            HttpResponseMessage response = _httpClient.GetAsync($"{_httpClient.BaseAddress}Customers/GetCustomer/{id}").Result;
            if (response.IsSuccessStatusCode)
            {
				string data = response.Content.ReadAsStringAsync().Result;
				var jsonArray = JArray.Parse(data);
				if (jsonArray.Count > 0)
				{
					customer = JsonConvert.DeserializeObject<CustomerViewModel>(jsonArray[0].ToString());
				}
			}
            return View(customer);
        }

		// POST: CustomerController/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Edit(CustomerViewModel customer)
		{
            try
            {
                var putTask = _httpClient.PutAsJsonAsync<CustomerViewModel>("Customers/PutCustomer", customer);
                putTask.Wait();
                var result = putTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = $"Customer with Id:{customer.Id} Updated successfully.";
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
            return View(customer);
        }

		// GET: CustomerController/Delete/5
		public IActionResult GetDelete(int id)
		{
            CustomerViewModel Delitem = new CustomerViewModel();
            HttpResponseMessage response = _httpClient.GetAsync($"{_httpClient.BaseAddress}Customers/GetCustomer/{id}").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                var jsonArray = JArray.Parse(data);
                if (jsonArray.Count > 0)
                {
                    Delitem = JsonConvert.DeserializeObject<CustomerViewModel>(jsonArray[0].ToString());
                }

            }
            ViewBag.customer = Delitem;
            return View("Delete", Delitem);
        }

		// POST: CustomerController/Delete/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Delete(int id)
		{
            HttpResponseMessage DeleteResponse = _httpClient.DeleteAsync($"{_httpClient.BaseAddress}Customers/DeleteCustomer/{id}").Result;
            if (DeleteResponse.IsSuccessStatusCode)
            {
                TempData["successMessage"] = $"Customer with:{id} Deleted successfully.";
                return RedirectToAction("Index");
            }
            TempData["errorMessage"] = "Delation Failed.";
            return View("Delete");
        }
	}
}
