using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using IdentityPro.Data;
using IdentityPro.Models;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.AspNetCore.Cors.Infrastructure;

namespace IdentityPro.Controllers
{
    public class OrdersController : Controller
    {
        private readonly Ice_cream_shopContext _context;

        public OrdersController(Ice_cream_shopContext context)
        {
            _context = context;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            return _context.Order != null ?
                        View(await _context.Order.Include(o => o.Products).ToListAsync()) :
                        Problem("Entity set 'Ice_cream_shopContext.Order'  is null.");
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Order == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CreatedDate,DeliveryDate")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Order == null)
            {
                return NotFound();
            }

            var order = await _context.Order.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CreatedDate,DeliveryDate")] Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Order == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Order == null)
            {
                return Problem("Entity set 'Ice_cream_shopContext.Order'  is null.");
            }
            var order = await _context.Order.FindAsync(id);
            if (order != null)
            {
                _context.Order.Remove(order);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return (_context.Order?.Any(e => e.Id == id)).GetValueOrDefault();
        }


        [HttpPost]
        public IActionResult AddToOrder(int productId, string orderId = "")
        {

            int order_Id;
            Order userOrder;
            try
            {
                if (int.TryParse(orderId, out order_Id))
                {
                    //order_Id = int.Parse(orderId);
                    // Find the user's active order (if it exists)
                    userOrder = _context.Order
                       .Include(o => o.Products)
                        .FirstOrDefault(o => o.Id == order_Id && o.DeliveryDate == null);
                }
                //var userId = 8;
                else
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
                // Find the product by ID
                var productToAdd = _context.Product.Find(productId);
                if (productToAdd != null)
                {
                    OrderItem foundProduct = userOrder.Products.FirstOrDefault(p => p.Name == productToAdd.Name);
                    if (foundProduct == null)
                    {
                        var orderItem = new OrderItem
                        {
                            Name = productToAdd.Name,
                            OrderId = userOrder.Id,
                            Price = productToAdd.Price,
                            Amount = 1,
                            ImagePath = productToAdd.ImagePath
                        };
                        // Add the product to the order
                        userOrder.Products.Add(orderItem);
                        _context.OrderItem.Add(orderItem);
                    }
                    else
                    {
                        foundProduct.Amount += 1;
                    }
                    _context.SaveChanges(); // Save changes synchronously
                    order_Id = userOrder.Id;
                    // Return a response indicating success
                    return Json(new { success = true, message = "Product added to cart.", order = userOrder });
                }
                else
                {
                    // Handle the case where the product with the given ID was not found
                    return Json(new { success = false, message = "Product not found." });
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during the process
                // You can log the error, show a user-friendly message, or redirect to an error page
                return Json(new { success = false, message = "An error occurred." });
            }
        }


        [HttpGet]
        public IActionResult getOrderData([FromQuery] int orderId)
        {
            if (orderId == -1)
            {
                return NotFound();
            }

            Order? userOrder = _context.Order
           .Include(o => o.Products)
            .FirstOrDefault(o => o.Id == orderId && o.DeliveryDate == null);


            if (userOrder != null)
            {
                ViewBag.Products = userOrder.Products;
                return Json(userOrder);
            }

            return NotFound();
        }


    }
}
