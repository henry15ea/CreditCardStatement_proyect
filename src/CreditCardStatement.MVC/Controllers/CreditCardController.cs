using CreditCardStatement.MVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace CreditCardStatement.MVC.Controllers
{
    public class CreditCardController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private string GetApiUrl() => _configuration["ApiSettings:BaseUrl"]!;

        public CreditCardController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        private HttpClient CreateAuthorizedClient()
        {
            var client = _httpClientFactory.CreateClient();
            var token = HttpContext.Session.GetString("Token");
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
            return client;
        }

        public async Task<IActionResult> Index(int? creditCardId, int? month, int? year)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
                return RedirectToAction("Login", "Account");

            var client = CreateAuthorizedClient();
            var apiUrl = GetApiUrl();

            var cardHolderId = 1;
            var response = await client.GetAsync($"{apiUrl}/api/creditcard/cards/{cardHolderId}");
            if (!response.IsSuccessStatusCode)
                return View(new StatementViewModel());

            var cardsJson = await response.Content.ReadAsStringAsync();
            var cards = JsonSerializer.Deserialize<List<JsonElement>>(cardsJson);

            var selectorVm = new CreditCardSelectorViewModel
            {
                CreditCards = cards.Select(c => new CreditCardOptionViewModel
                {
                    Id = c.GetProperty("id").GetInt32(),
                    CardNumber = c.GetProperty("cardNumber").GetString()!,
                    CurrentBalance = c.GetProperty("currentBalance").GetDecimal()
                }).ToList()
            };

            if (creditCardId.HasValue && month.HasValue && year.HasValue)
            {
                var statementResponse = await client.GetAsync($"{apiUrl}/api/creditcard/statement/{creditCardId.Value}?month={month.Value}&year={year.Value}");
                if (statementResponse.IsSuccessStatusCode)
                {
                    var statementJson = await statementResponse.Content.ReadAsStringAsync();
                    var statement = JsonSerializer.Deserialize<JsonElement>(statementJson);

                    var vm = new StatementViewModel
                    {
                        CreditCardId = creditCardId.Value,
                        CardHolderName = statement.GetProperty("creditCard").GetProperty("cardHolderName").GetString()!,
                        CardNumber = statement.GetProperty("creditCard").GetProperty("cardNumber").GetString()!,
                        CreditLimit = statement.GetProperty("creditCard").GetProperty("creditLimit").GetDecimal(),
                        CurrentBalance = statement.GetProperty("creditCard").GetProperty("currentBalance").GetDecimal(),
                        AvailableBalance = statement.GetProperty("availableBalance").GetDecimal(),
                        TotalPurchasesCurrentMonth = statement.GetProperty("totalPurchasesCurrentMonth").GetDecimal(),
                        TotalPurchasesPreviousMonth = statement.GetProperty("totalPurchasesPreviousMonth").GetDecimal(),
                        BonifiableInterest = statement.GetProperty("bonifiableInterest").GetDecimal(),
                        MinimumPayment = statement.GetProperty("minimumPayment").GetDecimal(),
                        TotalToPay = statement.GetProperty("totalToPay").GetDecimal(),
                        CashPaymentWithInterest = statement.GetProperty("cashPaymentWithInterest").GetDecimal(),
                        SelectedMonth = month.Value,
                        SelectedYear = year.Value,
                        Transactions = statement.GetProperty("transactions").EnumerateArray().Select(t => new TransactionViewModel
                        {
                            Id = t.GetProperty("id").GetInt32(),
                            CreditCardId = t.GetProperty("creditCardId").GetInt32(),
                            Type = t.GetProperty("typeName").GetString()!,
                            Date = DateTime.Parse(t.GetProperty("date").GetString()!),
                            Description = t.GetProperty("description").GetString()!,
                            Amount = t.GetProperty("amount").GetDecimal()
                        }).ToList()
                    };

                    ViewBag.CreditCards = selectorVm;
                    return View(vm);
                }
            }

            ViewBag.CreditCards = selectorVm;
            return View(new StatementViewModel
            {
                SelectedMonth = month ?? DateTime.Now.Month,
                SelectedYear = year ?? DateTime.Now.Year
            });
        }

        [HttpPost]
        public async Task<IActionResult> AddPurchase(PurchaseViewModel model)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
                return RedirectToAction("Login", "Account");

            var client = CreateAuthorizedClient();
            var apiUrl = GetApiUrl();

            var purchaseDto = new
            {
                CreditCardId = model.CreditCardId,
                Date = model.Date,
                Description = model.Description,
                Amount = model.Amount
            };

            var content = new StringContent(
                JsonSerializer.Serialize(purchaseDto),
                Encoding.UTF8,
                "application/json");

            await client.PostAsync($"{apiUrl}/api/creditcard/purchase", content);

            return RedirectToAction("Index", new { creditCardId = model.CreditCardId, month = model.Date.Month, year = model.Date.Year });
        }

        [HttpPost]
        public async Task<IActionResult> MakePayment(PaymentViewModel model)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
                return RedirectToAction("Login", "Account");

            var client = CreateAuthorizedClient();
            var apiUrl = GetApiUrl();

            var paymentDto = new
            {
                CreditCardId = model.CreditCardId,
                Date = model.Date,
                Amount = model.Amount
            };

            var content = new StringContent(
                JsonSerializer.Serialize(paymentDto),
                Encoding.UTF8,
                "application/json");

            await client.PostAsync($"{apiUrl}/api/creditcard/payment", content);

            return RedirectToAction("Index", new { creditCardId = model.CreditCardId, month = model.Date.Month, year = model.Date.Year });
        }

        public async Task<IActionResult> History(int creditCardId, int month, int year)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
                return RedirectToAction("Login", "Account");

            var client = CreateAuthorizedClient();
            var apiUrl = GetApiUrl();

            var response = await client.GetAsync($"{apiUrl}/api/creditcard/transactions/{creditCardId}?month={month}&year={year}");
            if (!response.IsSuccessStatusCode)
                return View(new List<TransactionViewModel>());

            var json = await response.Content.ReadAsStringAsync();
            var transactions = JsonSerializer.Deserialize<List<JsonElement>>(json);

            var vm = transactions.Select(t => new TransactionViewModel
            {
                Id = t.GetProperty("id").GetInt32(),
                CreditCardId = t.GetProperty("creditCardId").GetInt32(),
                Type = t.GetProperty("typeName").GetString()!,
                Date = DateTime.Parse(t.GetProperty("date").GetString()!),
                Description = t.GetProperty("description").GetString()!,
                Amount = t.GetProperty("amount").GetDecimal()
            }).ToList();

            ViewBag.Month = month;
            ViewBag.Year = year;
            ViewBag.CreditCardId = creditCardId;

            return View(vm);
        }

        public async Task<IActionResult> ExportPdf(int creditCardId, int month, int year)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
                return RedirectToAction("Login", "Account");

            var client = CreateAuthorizedClient();
            var apiUrl = GetApiUrl();

            var response = await client.GetAsync($"{apiUrl}/api/creditcard/statement/{creditCardId}/pdf?month={month}&year={year}");

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                TempData["ErrorMessage"] = $"Error al generar el PDF: {response.StatusCode}. {errorContent}";
                return RedirectToAction("Index", new { creditCardId, month, year });
            }

            var pdfBytes = await response.Content.ReadAsByteArrayAsync();
            return File(pdfBytes, "application/pdf", $"EstadoCuenta_{creditCardId}_{month}_{year}.pdf");
        }

        public IActionResult Purchases(int creditCardId, int month, int year)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
                return RedirectToAction("Login", "Account");

            var vm = new PurchaseViewModel
            {
                CreditCardId = creditCardId,
                Date = new DateTime(year, month, DateTime.DaysInMonth(year, month))
            };

            return View(vm);
        }

        public IActionResult Payments(int creditCardId, int month, int year)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
                return RedirectToAction("Login", "Account");

            var vm = new PaymentViewModel
            {
                CreditCardId = creditCardId,
                Date = new DateTime(year, month, DateTime.DaysInMonth(year, month))
            };

            return View(vm);
        }
        //end user functions or definitions
    }
    //end class
}
//end namespaces