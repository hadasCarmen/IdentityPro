using IdentityPro.Data;
using IdentityPro.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdentityPro.Controllers
{
    public class DefaultController : Controller
    {
        private readonly Ice_cream_shopContext _context;
        private readonly IEnumerable<Order> userOrder;


        public DefaultController(Ice_cream_shopContext context)
        {
            _context = context;
            userOrder = context.Order;
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
            Order userOrder;
            if (orderId == -1 )
            {
                var newWeather = new Weather
                {
                    Date = DateTime.Now, // Set the date

                };

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
                   .Include(o => o.Products)
                   .Where(o => o.Id == orderId && o.DeliveryDate == null)
                   .FirstOrDefault();
                
            }
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

        // GET: DefaultController/Checkout
        public ActionResult Checkout()
        {
            string userName = User.Identity?.Name;
            if(userName == null)
            {
                TempData["RegisterMessage"] = "Please register to access this feature.";
                return RedirectToPage("/Account/Register", new { area = "Identity" });
            }
            return View();
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

    }

}
