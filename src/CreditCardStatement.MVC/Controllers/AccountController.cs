using CreditCardStatement.MVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace CreditCardStatement.MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public AccountController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var client = _httpClientFactory.CreateClient();
            var apiUrl = _configuration["ApiSettings:BaseUrl"];

            var loginDto = new
            {
                Email = model.Email,
                Password = model.Password
            };

            var content = new StringContent(
                JsonSerializer.Serialize(loginDto),
                Encoding.UTF8,
                "application/json");

            var response = await client.PostAsync($"{apiUrl}/api/auth/login", content);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<JsonElement>(json);

                if (result.GetProperty("success").GetBoolean())
                {
                    HttpContext.Session.SetString("Token", result.GetProperty("token").GetString()!);
                    HttpContext.Session.SetString("UserEmail", model.Email);
                    return RedirectToAction("Index", "CreditCard");
                }
            }

            ModelState.AddModelError(string.Empty, "Invalid email or password");
            return View(model);
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
        //end user functions or definitions
    }
    //end class
}
//end namespaces