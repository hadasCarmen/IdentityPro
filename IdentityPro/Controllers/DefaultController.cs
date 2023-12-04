using IdentityPro.Data;
using IdentityPro.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using IceCreamShopGateway.Services;
using PayPal.Api;
using System.IO;
using PayPalHttp;
using Newtonsoft.Json;
using IceCreamShopGateway.Models;
using Microsoft.CodeAnalysis;
using static PayPal.BaseConstants;
using System.Security.Cryptography.X509Certificates;

namespace IdentityPro.Controllers
{
    public class DefaultController : Controller
    {
        private readonly Ice_cream_shopContext _context;
        private readonly ApplicationDbContext _user_context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly System.Net.Http.HttpClient _httpClient;

        //private readonly AddressService _addressService;
        // private readonly WeatherService _weatherService;


        public DefaultController(Ice_cream_shopContext context, ApplicationDbContext user_context, UserManager<IdentityUser> userManager, System.Net.Http.HttpClient httpClient/*, AddressService addressService, WeatherService weatherService*/)
        {
            _context = context;
            _user_context = user_context;
            _userManager = userManager;
            _httpClient = httpClient;
            //_httpClient.BaseAddress = new Uri("http://localhost:5041");
            //_addressService = addressService;
            //_weatherService = weatherService;

        }
        // GET: DefaultController
        public ActionResult Home()
        {
            var topProducts = new List<OrderItem>();
            var amount = _context.OrderItem.Count();
            if (amount > 0)
            {
                 topProducts = _context.OrderItem
                        .GroupBy(oi => oi.Name)
                        .Select(group => new OrderItem
                        {
                            Name = group.Key,
                            Amount = group.Sum(oi => oi.Amount),
                            Price = group.First().Price,
                            ImagePath = group.First().ImagePath
                        })
                        .OrderByDescending(oi => oi.Amount)
                        .Take(3)
                        .ToList();
            }
            var products = new List<Product>();
            foreach(var item in topProducts)
            {
                var product = _context.Product
                .FirstOrDefault(p => p.Name == item.Name);
                products.Add(product);
            }
            return View(products);
        }
        // GET: DefaultController/About
        public ActionResult About()
        {
            var topProducts = new List<OrderItem>();
            var amount = _context.OrderItem.Count();
            if (amount > 0)
            {
                topProducts = _context.OrderItem
                       .GroupBy(oi => oi.Name)
                       .Select(group => new OrderItem
                       {
                           Name = group.Key,
                           Amount = group.Sum(oi => oi.Amount),
                           Price = group.First().Price,
                           ImagePath = group.First().ImagePath
                       })
                       .OrderByDescending(oi => oi.Amount)
                       .Take(3)
                       .ToList();
            }
            var products = new List<Product>();
            foreach (var item in topProducts)
            {
                var product = _context.Product
                .FirstOrDefault(p => p.Name == item.Name);
                products.Add(product);
            }
            return View(products);
        }
        public IActionResult PayPal()
        {
            // Your logic to prepare data for the PayPal view
            return View("PayPal"); // Assuming "PayPal" is your view name
        }




        // GET: DefaultController/Shop
        public async Task<IActionResult> Shop()
        {
            var products = await _context.Product.ToListAsync();

            if (products != null)
            {
                return View(products); // Pass the products as the model to the view
            }
            else
            {
                return Problem("Entity set 'Ice_cream_shopContext.Product' is null.");
            }
            //return _context.Product != null ?
            //             View(await _context.Product.ToListAsync()) :
            //             Problem("Entity set 'Ice_cream_shopContext.Product'  is null.");

        }
        // GET: DefaultController/Contact

        public ActionResult Contact()
        {
            return View();
        }
        // GET: DefaultController/Cart

        public IActionResult Cart(int orderId)
        {
            try
            {
                Models.Order userOrder = CreateOrder(orderId);
                // Pass the user's order as the model to the cart view
                if (userOrder != null)
                {
                    return View(userOrder);
                }
                else
                {
                    // Return an error message or handle the case when the order is not found
                    string errorMessage = "Error accured please try again ";
                    return RedirectToAction("Error", "Home", new { errorMessage });
                }
            }
            catch
            {
                string errorMessage = "Please register first.";
                return RedirectToAction("Error", "Home", new { errorMessage });
            }
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {

            [Required]
            public string City { get; set; }

            [Required]
            public string Street { get; set; }

            [Required]
            public string Apartment { get; set; }

            [Required]
            public int ZipCode { get; set; }
           
            [Required]
            public Models.Order MyOrder { get; set; }

        }


        // GET: DefaultController/Checkout
        public ActionResult Checkout(int orderId)
        {
            string userName = User.Identity?.Name;
            Input = new InputModel();
            if (userName == null)
            {
                string errorMessage = "Please register first.";
                return RedirectToAction("Error", "Home", new { errorMessage });
            }
            // Initialize InputModel with a new instance
            try
                {
                    Input.MyOrder = CreateOrder(orderId);
                }
                catch
                {
                string errorMessage = "Error accured please try again ";
                return RedirectToAction("Error", "Home", new { errorMessage });
            }

                if (Input.MyOrder == null && orderId != -1)
                {
                // Return an error message or handle the case when the order is not found
                string errorMessage = "Error accured please try again ";
                return RedirectToAction("Error", "Home", new { errorMessage });
            }
                        return View(Input);
        }

        
        public async Task<bool> CheckAddressExistence(string city, string street)
        {
            var apiUrl = $"http://localhost:5041/api/Address/checkAddress?city={city}&street={street}";

            // Create an instance of HttpClient
            using (var httpClient = new System.Net.Http.HttpClient())
            {
                // Send a GET request to the other project's endpoint
                var response = await httpClient.GetAsync(apiUrl);
                

                if (response.IsSuccessStatusCode)
                {
                    var content = response.Content.ReadAsStringAsync().Result;

                    // Deserialize the response content manually
                    var result = JsonConvert.DeserializeObject<bool?>(content);

                    // Use the result directly in the if statement
                    return result ?? false; // If result is null, default to false
                }
                else
                {
                    // Handle the error
                    return false; // Return false or handle the error accordingly
                }
            }
        }

        public async Task<WeatherInfo> GeWeather(string location)
        {
            var apiUrl = $"http://localhost:5041/api/Weather/get?location={location}";

            // Create an instance of HttpClient
            using (var httpClient = new System.Net.Http.HttpClient())
            {
                // Send a GET request to the other project's endpoint
                var response = await httpClient.GetAsync(apiUrl);


                if (response.IsSuccessStatusCode)
                {
                    var content = response.Content.ReadAsStringAsync().Result;

                    // Deserialize the response content manually
                    var result = JsonConvert.DeserializeObject<WeatherInfo>(content);

                    // Use the result directly in the if statement
                    return result; // If result is null, default to false
                }
                else
                {
                    // Handle the error
                    return null; // Return false or handle the error accordingly
                }
            }
        }

        public async Task<bool> CheckIfHolidayWeek()
        {
            var apiUrl = $"http://localhost:5041/api/HebrewCal/get";

            // Create an instance of HttpClient
            using (var httpClient = new System.Net.Http.HttpClient())
            {
                // Send a GET request to the other project's endpoint
                var response = await httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var content = response.Content.ReadAsStringAsync().Result;
                    // Deserialize the response content manually
                    var result = JsonConvert.DeserializeObject<bool>(content);

                    // Use the result directly in the if statement
                    return result; // If result is null, default to false
                }
                else
                {
                    // Handle the error
                    return false; // Return false or handle the error accordingly
                }
            }
        }

            // POST: DefaultController/UpdateCheckout
            [HttpPost]
        public async Task<ActionResult> UpdateCheckout(InputModel model, int orderId)
        {
            if (model.ZipCode != 0 && model.Street != null && model.City != null && model.Apartment != null)
            {
                int myId = orderId;
                string userName = User.Identity?.Name;
                IdentityUser user = _userManager.FindByNameAsync(userName).Result;
                ApplicationUser applicationUser = (ApplicationUser)user;

                if (applicationUser != null)
                {
                    //var content = CheckAddressExistence(model.City.ToString(), model.Street.ToString());
                    bool isValidAddresss = await CheckAddressExistence(model.City.ToString(), model.Street.ToString());
                    bool IsHolidayWeek = await CheckIfHolidayWeek();
                    // Deserialize the response content manually
                    if (isValidAddresss)
                    {
                        WeatherInfo weather = await GeWeather(model.City.ToString());
                        // Modify the user's fields based on the form data
                        applicationUser.City = model.City.ToString();
                        applicationUser.Street = model.Street.ToString();
                        applicationUser.Apartment = model.Apartment.ToString();
                        applicationUser.ZipCode = model.ZipCode;
                        user = applicationUser;
                        // Save changes to the database
                        var result = _userManager.UpdateAsync(user).Result;
                        _user_context.SaveChanges();

                        if (result.Succeeded)
                        {
                            try
                            {
                                model.MyOrder = CreateOrder(orderId);
                            }
                            catch
                            {
                                string errorMessage = "Please register first.";
                                return RedirectToAction("Error", "Home", new { errorMessage });
                            }
                            model.MyOrder.CreatedDate = DateTime.Now;
                            model.MyOrder.DeliveryDate = DateTime.Now.AddDays(14);

                            model.MyOrder.Weather.Season = weather.Season;
                            model.MyOrder.Weather.Temp = (int)weather.Temp;
                            model.MyOrder.Weather.Humidity = weather.Humidity;
                            model.MyOrder.IsHoliday = IsHolidayWeek;
                            model.MyOrder.Day = DateTime.Now.ToString("dddd");

                            //var temp = (int)weather.Temp;
                            _context.SaveChanges();
                            // Continue with the checkout process or redirect to a success page
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            string errorMessage = "Error accured please try again ";
                            return RedirectToAction("Error", "Home", new { errorMessage });
                        }
                    }
                    else
                    {
                        string errorMessage = "Address not found";
                        return RedirectToAction("Error", "Home", new { errorMessage });
                    }

                }
                else
                {
                    string errorMessage = "Please register first.";
                    return RedirectToAction("Error", "Home", new { errorMessage });// Handle the case where the user is not found
                }
            }
            else
            {
                string errorMessage = "One or more of the details are missing";
                return RedirectToAction("Error", "Home", new { errorMessage });// Handle the case where the user is not found
            }
        }

        // Action method to retrieve product data by productId
        [HttpGet]
        public IActionResult GetProductData(int productId)
        {
            // Query your database to retrieve the product data
            var product = _context.Product
                .Where(p => p.Id == productId)
                .FirstOrDefault();

            if (product == null)
            {
                return NotFound(); // Handle the case where the product is not found
            }

            // Serialize the product data to JSON and return it
            return Json(product);
        }

        [HttpPost]
        public IActionResult UpdateCart(int id, [Bind("Id,Products")] Models.Order order)
        {
            if (order != null && order.Products != null)
            {
                foreach (var orderitem in order.Products)
                {
                    // You can access the updated quantity for each product like this:
                    int updatedQuantity = orderitem.Amount;

                    // Retrieve the current product from the database based on its ID.
                    var currentProduct = _context.OrderItem.Where(x=> x.Id == orderitem.Id).FirstOrDefault();  

                    if (currentProduct != null)
                    {
                            // Update the quantity of the product.
                            currentProduct.Amount = updatedQuantity;

                        // You can perform additional updates or validations here if needed.

                        // Save changes to the database.
                        _context.SaveChanges();
                    }
                }
            }

            // After processing the updates, you can redirect back to the cart page or perform other actions.
            //return RedirectToAction("Cart");
            return RedirectToAction("Cart", new { orderId = order.Id });

        }

        [HttpPost]
        public IActionResult RemoveProduct(int orderId, int productId)
        {
            var currentOrder = _context.Order.Include(o => o.Products)
                .Where(x => x.Id == orderId).FirstOrDefault();

            if (currentOrder != null && currentOrder.Products != null)
            {
                OrderItem currentProduct = _context.OrderItem.Where(x => x.Id == productId).FirstOrDefault();
                if (currentProduct != null)
                {

                    // Update the quantity of the product.
                    currentProduct.Amount = 0;
                    currentOrder.Products.Remove(currentProduct);
                    _context.OrderItem.Remove(currentProduct);
                     // Save changes to the database.
                    _context.SaveChanges();
                    
                }
            }

            // After processing the updates, you can redirect back to the cart page or perform other actions.
            return RedirectToAction("Cart", new { orderId = orderId });
        }

        public Models.Order CreateOrder(int orderId)
        {
            string userName = User.Identity?.Name;
            var newWeather = new Models.Weather
            {
                Date = DateTime.Now, // Set the date
                Season = "winter"
            };
            if (userName == null)
            {
                throw new ArgumentNullException(nameof(userName));
            }
            else
            {
                IdentityUser user = _userManager.FindByNameAsync(userName).Result;
                ApplicationUser applicationUser = (ApplicationUser)user;
                Models.Order userOrder;

                if (orderId == -1)
                {
                    // Check if an active order already exists for the user
                    userOrder = _context.Order
                        .Include(o => o.User)
                        .Where(o => o.User.Id == applicationUser.Id && o.DeliveryDate == null)
                        .FirstOrDefault();

                    if (userOrder == null)
                    {
                        // If no active order exists, create a new one

                        // Associate the Weather entity with the Order
                        userOrder = new Models.Order
                        {
                            CreatedDate = DateTime.Now,
                            DeliveryDate = null,       // Set the delivery date (if applicable)
                            Products = new List<OrderItem>(),
                            Weather = newWeather,
                            User = applicationUser,  // Associate the ApplicationUser with the Order
                            IsHoliday = false
                        };

                        _context.Order.Add(userOrder);
                    }
                    // else: An active order already exists, no need to create a new one
                }
                else
                {
                    // Update existing order's user (check if it's necessary)
                    userOrder = _context.Order
                        .Include(o => o.User)
                        .Include(o => o.Products)
                        .Where(o => o.Id == orderId && o.DeliveryDate == null)
                        .FirstOrDefault();

                    if (userOrder != null)
                    {
                        userOrder.User = applicationUser;
                        userOrder.Weather = newWeather;
                    }
                    // else: The order with orderId doesn't exist or is already delivered
                }

                return userOrder;
            }
        }
    }



}
