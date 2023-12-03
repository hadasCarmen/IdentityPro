using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using IdentityPro.Data;
using IdentityPro.Models;
using Newtonsoft.Json;

namespace IdentityPro.Controllers
{
    public class ProductsController : Controller
    {
        private readonly Ice_cream_shopContext _context;

        public ProductsController(Ice_cream_shopContext context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
              return _context.Product != null ? 
                          View(await _context.Product.ToListAsync()) :
                          Problem("Entity set 'Ice_cream_shopContext.Product'  is null.");
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Product == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }

        public async Task<bool> IsIceCream(string path)
        {
            var apiUrl = $"http://localhost:5041/api/image?imageUrl={path}";

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

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Price,Description,Amount,ImagePath,Flavor")] Product product)
        {
            string myPath = product.ImagePath;
            bool img = await IsIceCream(myPath);

            if (img)
            {
                if (ModelState.IsValid)
                {
                    _context.Add(product);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                return View(product);
            }
            else
            {
                ViewBag.ErrorMessage = "Doesnt contain ice cream";
                return View("Error"); // Handle error scenario
            }

        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Product == null)
            {
                return NotFound();
            }

            var product = await _context.Product.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price,Description,Amount,ImagePath,Flavor")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }
            string myPath = product.ImagePath;
            bool img = await IsIceCream(myPath);

            if (img)
            {

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(product);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!ProductExists(product.Id))
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
                return View(product);
            }
            else
            
            {
                ViewBag.ErrorMessage = "Doesnt contain ice cream";
                return View("Error"); // Handle error scenario
            }
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Product == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Product == null)
            {
                return Problem("Entity set 'Ice_cream_shopContext.Product'  is null.");
            }
            var product = await _context.Product.FindAsync(id);
            if (product != null)
            {
                _context.Product.Remove(product);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
          return (_context.Product?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
