using ConsumeWebApi.Models.category;
using ConsumeWebApi.Models.product;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http;

namespace ConsumeWebApi.Controllers
{
    public class CategoryController : Controller
    {
        Uri baseAddress = new Uri("https://localhost:7037/api/");
        private readonly HttpClient _httpClient;
        public IWebHostEnvironment _webHostEnvironment;

        public CategoryController(IWebHostEnvironment webHostEnvironment)
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = baseAddress;
            _webHostEnvironment = webHostEnvironment;
        }
        // GET: CategoryController
        public IActionResult Index(string search, string sortOrder, string sortType, int pageSize = 10, int pageNumber = 1)
        {
            ViewBag.CurrentSearch = search;
            ViewBag.SortOrder = sortOrder;
            ViewBag.SortType = sortType;
            ViewBag.pageSize = pageSize;
            ViewBag.pageNumber = pageNumber;

            
            HttpResponseMessage response = _httpClient.GetAsync($"{_httpClient.BaseAddress}Categories/GetCategories").Result;
            List<GetCategory> categories = new List<GetCategory>();

            if (response.IsSuccessStatusCode)
            {
                string Data = response.Content.ReadAsStringAsync().Result;
                categories = JsonConvert.DeserializeObject<List<GetCategory>>(Data);
            }
            SelectList categoryList = new SelectList(categories, "Id", "Name", ViewBag.CatId);
            ViewBag.AllCategories = categoryList;

            int totalCount = categories.Count(); // Calculate the total count
            ViewBag.totalCount = totalCount; // Pass it to the view

            return View(categories);
        }

        // GET: CategoryController/Details/5
        public ActionResult Details(int id)
        {
            GetCategory category = new GetCategory();
            HttpResponseMessage response = _httpClient.GetAsync($"{_httpClient.BaseAddress}Categories/GetCategory/{id}").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                category = JsonConvert.DeserializeObject<GetCategory>(data);

            }
            ViewBag.SpecificCat = category;
            return View("Details", category);
        }

        // GET: CategoryController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CategoryController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PostCategory model)
        {
            try
            {
                // Create a new instance of MultipartFormDataContent
                MultipartFormDataContent multipartFormDataContent = new MultipartFormDataContent();
                // Add properties to the multipartFormDataContent
                multipartFormDataContent.Add(new StringContent(model.Name, Encoding.UTF8), "Name");
                multipartFormDataContent.Add(new StringContent(model.Description, Encoding.UTF8), "Description");
               
                if (model.Image != null)
                {

                    var imageContent = new StreamContent(model.Image.OpenReadStream());
                    imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse(model.Image.ContentType);
                    multipartFormDataContent.Add(imageContent, "Image", model.Image.FileName);

                }

                var response =  _httpClient.PostAsync("Categories/PostCategory", multipartFormDataContent).Result;
                if (response.IsSuccessStatusCode)
                {

                    // Product created successfully
                    TempData["successMessage"] = "Category created successfully";
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
    

        // GET: CategoryController/Edit/5
        public ActionResult Edit(int id)
        {
        PutCategory category = new PutCategory();
        HttpResponseMessage response = _httpClient.GetAsync($"{_httpClient.BaseAddress}Categories/GetCategory/{id}").Result;
        if (response.IsSuccessStatusCode)
        {
            string data = response.Content.ReadAsStringAsync().Result;
            category = JsonConvert.DeserializeObject<PutCategory>(data);

        }
        return View(category);
    }

        // POST: CategoryController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PutCategory model)
        {
            try
            {
                // Create a new instance of MultipartFormDataContent
                MultipartFormDataContent multipartFormDataContent = new MultipartFormDataContent();
                // Add properties to the multipartFormDataContent
                multipartFormDataContent.Add(new StringContent(model.Id.ToString(), Encoding.UTF8), "Id");
                multipartFormDataContent.Add(new StringContent(model.Name, Encoding.UTF8), "Name");
                multipartFormDataContent.Add(new StringContent(model.Description, Encoding.UTF8), "Description");
                multipartFormDataContent.Add(new StringContent(model.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"), Encoding.UTF8), "CreatedAt");

                if (model.Image != null)
                {

                    var imageContent = new StreamContent(model.Image.OpenReadStream());
                    imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse(model.Image.ContentType);
                    multipartFormDataContent.Add(imageContent, "Image", model.Image.FileName);

                    // Send the POST request to the API

                }

                var response = _httpClient.PutAsync("Categories/PutCategories", multipartFormDataContent).Result;
                if (response.IsSuccessStatusCode)
                {

                    // Product created successfully
                    TempData["successMessage"] = "Category Updated successfully";
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

        // GET: CategoryController/Delete/5
        public ActionResult GetDelete(int id)
        {
            GetCategory category = new GetCategory();
            HttpResponseMessage response = _httpClient.GetAsync($"{_httpClient.BaseAddress}Categories/GetCategory/{id}").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                category = JsonConvert.DeserializeObject<GetCategory>(data);

            }
            ViewBag.SpecificCat = category;
            return View("Delete", category);
        }

        // POST: CategoryController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            HttpResponseMessage DeleteResponse =  _httpClient.DeleteAsync($"{_httpClient.BaseAddress}Categories/DeleteCategory/{id}").Result;
            if (DeleteResponse.IsSuccessStatusCode)
            {
                TempData["successMessage"] = "Category Deleted successfully.";
                return RedirectToAction("Index");
            }
            TempData["errorMessage"] = "Delation Failed.";
            return View("Delete");
        }

    }
}
