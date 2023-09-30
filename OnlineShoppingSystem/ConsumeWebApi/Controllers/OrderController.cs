using Azure;
using ConsumeWebApi.Models.customer;
using ConsumeWebApi.Models.order;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Drawing.Printing;
using Web_Api_ITI.Models;

namespace ConsumeWebApi.Controllers
{
    public class OrderController : Controller
    {
        Uri baseAddress = new Uri("https://localhost:7037/api/");
        private readonly HttpClient _httpClient;
        public OrderController()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = baseAddress;
        }
        // GET: OrderController
        public ActionResult Index(string search, string? status, string sortOrder, string sortType, int pageSize = 5, int pageNumber = 1)
        {
            ViewBag.CurrentSearch = search;
            ViewBag.SortOrder = sortOrder;
            ViewBag.status = status;
            ViewBag.SortType = sortType;
            ViewBag.pageSize = pageSize;
            ViewBag.pageNumber = pageNumber;

            List<GetOrder> orders = new List<GetOrder>();
            HttpResponseMessage response = _httpClient.GetAsync($"{_httpClient.BaseAddress}Orders/GetOrders?search={search}&status={status}&sortOrder={sortOrder}&sortType={sortType}&pageSize={pageSize}&pageNumber={pageNumber}").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                orders = JsonConvert.DeserializeObject<List<GetOrder>>(data);

            }

            ViewBag.totalCount = orders.Count; 

			List<string> statusList = new List<string> { "Pending", "Shipped", "Arrived" };
            SelectList SelectStatusList = new SelectList(statusList);
            ViewBag.AllStatus = SelectStatusList;
            return View(orders);
        }

        // GET: OrderController/Details/5
        public ActionResult Details(int id)
        {
            GetOrder order = new GetOrder();
            HttpResponseMessage response = _httpClient.GetAsync($"{_httpClient.BaseAddress}Orders/GetOrder/{id}").Result;
            if (response.IsSuccessStatusCode)
            {
				//string data = response.Content.ReadAsStringAsync().Result;
				//order = JsonConvert.DeserializeObject<GetOrder>(data);
				string data = response.Content.ReadAsStringAsync().Result;
				var jsonArray = JArray.Parse(data);
				if (jsonArray.Count > 0)
				{
					order = JsonConvert.DeserializeObject<GetOrder>(jsonArray[0].ToString());
				}


			}
			ViewBag.SpecificOrder = order;
            return View("Details", order);
        }

        // GET: OrderController/Create
        public ActionResult Create()
        {
            List<CustomerViewModel> customers = new List<CustomerViewModel>();
            HttpResponseMessage response = _httpClient.GetAsync($"{_httpClient.BaseAddress}Customers/GetCustomers").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                customers = JsonConvert.DeserializeObject<List<CustomerViewModel>>(data);
            }
			List<string> statusList = new List<string> { "Pending", "Shipped", "Arrived" };
			SelectList SelectStatusList = new SelectList(statusList);
			ViewBag.AllStatus = SelectStatusList;
			ViewBag.Customers = new SelectList(customers, "Id", "Name");
            return View();
        }

        // POST: OrderController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PostOrder Order)
        {
            try
            {

				var postTask = _httpClient.PostAsJsonAsync<PostOrder>("Orders/PostOrder", Order);
                postTask.Wait();
                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "Order created successfully";
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
            return View(Order);
           
        }

        // GET: OrderController/Edit/5
        public ActionResult Edit(int id)
        {
            PutOrder order = new PutOrder();
            HttpResponseMessage response = _httpClient.GetAsync($"{_httpClient.BaseAddress}Orders/GetOrder/{id}").Result;
            if (response.IsSuccessStatusCode)
            {
                //string data = response.Content.ReadAsStringAsync().Result;
                //order = JsonConvert.DeserializeObject<GetOrder>(data);
                string data = response.Content.ReadAsStringAsync().Result;
                var jsonArray = JArray.Parse(data);
                if (jsonArray.Count > 0)
                {
                    order = JsonConvert.DeserializeObject<PutOrder>(jsonArray[0].ToString());
                }
            }
			List<string> statusList = new List<string> { "Pending", "Shipped", "Arrived" };
			SelectList SelectStatusList = new SelectList(statusList);
			ViewBag.AllStatus = SelectStatusList;
			return View(order);
        }
        // POST: OrderController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PutOrder order)
        {
			try
			{
				var putTask = _httpClient.PutAsJsonAsync<PutOrder>("Orders/PutOrder", order);
				putTask.Wait();
				var result = putTask.Result;
				if (result.IsSuccessStatusCode)
				{
					TempData["successMessage"] = $"Order with Id:{order.Id} Updated successfully.";
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
			return View(order);
		}

        // GET: OrderController/Delete/5
        public ActionResult GetDelete(int id)
        {
			GetOrder order = new GetOrder();
			HttpResponseMessage response = _httpClient.GetAsync($"{_httpClient.BaseAddress}Orders/GetOrder/{id}").Result;
			if (response.IsSuccessStatusCode)
			{
				//string data = response.Content.ReadAsStringAsync().Result;
				//order = JsonConvert.DeserializeObject<GetOrder>(data);
				string data = response.Content.ReadAsStringAsync().Result;
				var jsonArray = JArray.Parse(data);
				if (jsonArray.Count > 0)
				{
					order = JsonConvert.DeserializeObject<GetOrder>(jsonArray[0].ToString());
				}
			}
            ViewBag.SpecificOrder=order;
            return View("Delete",order);
		}

        // POST: OrderController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
			HttpResponseMessage DeleteResponse = _httpClient.DeleteAsync($"{_httpClient.BaseAddress}Orders/DeleteOrder/{id}").Result;
			if (DeleteResponse.IsSuccessStatusCode)
			{
				TempData["successMessage"] = $"Order with:{id} Deleted successfully.";
				return RedirectToAction("Index");
			}
			TempData["errorMessage"] = "Delation Failed.";
			return View("Delete");
		}
    }
}
