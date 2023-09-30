using ConsumeApi.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ConsumeApi.Controllers
{
	public class ProductController : Controller
	{
		Uri baseAddress = new Uri("https://localhost:44398");
		private readonly HttpClient _httpClient;
        public ProductController()
        {
            _httpClient = new HttpClient();
			_httpClient.BaseAddress = baseAddress;
        }
       
        public IActionResult GetIndexView()
		{
			List<ProductViewModel> products = new List<ProductViewModel>();
			HttpResponseMessage response=_httpClient.GetAsync(_httpClient.BaseAddress+"/api/Product/Get").Result;
			if (response.IsSuccessStatusCode)
			{ 
				string data = response.Content.ReadAsStringAsync().Result;
				products=JsonConvert.DeserializeObject<List<ProductViewModel>>(data);

			}
			return View("Index",products);
		}
	}
}
