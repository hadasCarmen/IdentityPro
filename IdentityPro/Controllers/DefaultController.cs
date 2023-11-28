using IdentityPro.Data;
using IdentityPro.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace IdentityPro.Controllers
{
    public class DefaultController : Controller
    {
        private readonly Ice_cream_shopContext _context;
        private readonly ApplicationDbContext _user_context;
        private readonly UserManager<IdentityUser> _userManager;

        public DefaultController(Ice_cream_shopContext context, ApplicationDbContext user_context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _user_context = user_context;
            _userManager = userManager;
        }
        // GET: DefaultController
        public ActionResult Home()
        {
            return View();
        }
        // GET: DefaultController/About
        public ActionResult About()
        {
            return View();
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
            //int userId = int.Parse(orderId);
            //string myId = orderId;
            Order userOrder = CreateOrder(orderId);
            // Pass the user's order as the model to the cart view
            if (userOrder != null)
            {
                return View(userOrder);
            }
            else
            {
                // Return an error message or handle the case when the order is not found
                return Problem($"Order with ID {orderId} not found.");
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
            public Order MyOrder { get; set; }

        }


        // GET: DefaultController/Checkout
        public ActionResult Checkout(int orderId)
        {
            string userName = User.Identity?.Name;
            if (userName == null)
            {
                ViewBag.ErrorMessage = "Please register first.";
            }
            // Initialize InputModel with a new instance
            Input = new InputModel();

            Input.MyOrder = CreateOrder(orderId);

            if (Input.MyOrder == null && orderId != -1)
            {
                // Return an error message or handle the case when the order is not found
                return Problem($"Order with ID {orderId} not found.");
            }

            // Pass the InputModel as the model to the view
            return View(Input);
        }
        // POST: DefaultController/UpdateCheckout
        [HttpPost]
        public ActionResult UpdateCheckout(InputModel model, int orderId)
        {
            int myId = orderId;
            string userName = User.Identity?.Name;
            IdentityUser user = _userManager.FindByNameAsync(userName).Result;
            ApplicationUser applicationUser = (ApplicationUser)user;

            if (applicationUser != null)
            {
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
                    model.MyOrder = CreateOrder(orderId);
                    model.MyOrder.CreatedDate = DateTime.Now;
                    model.MyOrder.DeliveryDate = DateTime.Now.AddDays(14);
                    _context.SaveChanges();
                    // Continue with the checkout process or redirect to a success page
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.ErrorMessage = "Failed to update user information.";
                    return View("Error"); // Handle error scenario
                }
            }
            else
            {
                ViewBag.ErrorMessage = "User not found.";
                return View("Error"); // Handle the case where the user is not found
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
        public IActionResult UpdateCart(int id, [Bind("Id,Products")] Order order)
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

        public Order CreateOrder(int orderId)
        {
            string userName = User.Identity?.Name;
            IdentityUser user = _userManager.FindByNameAsync(userName).Result;
            ApplicationUser applicationUser = (ApplicationUser)user;
            Order userOrder;
            if (orderId == -1)
            {
                var newWeather = new Weather
                {
                    Date = DateTime.Now, // Set the date

                };
                var orderUser =  applicationUser;

                // If no active order exists, create a new one
                userOrder = new Order
                {
                    CreatedDate = DateTime.Now,
                    DeliveryDate = null,       // Set the delivery date (if applicable)
                    Products = new List<OrderItem>(),
                    Weather = newWeather      // Associate the Weather entity with the Order

                };
                _context.Order.Add(userOrder);
            }
            else
            {
                userOrder = _context.Order
                .Include(o=>o.User)
               .Include(o => o.Products)
               .Where(o => o.Id == orderId && o.DeliveryDate == null)
               .FirstOrDefault();
                userOrder.User = applicationUser;
            }
            return userOrder;
        }

    }

    

}
