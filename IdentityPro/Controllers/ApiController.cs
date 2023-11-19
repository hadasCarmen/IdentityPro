using IdentityPro.Models;
//using MainApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IdentityPro.Controllers
{
    public class ApiController : Controller
    {
        // GET: ApiController
        //public async Task<IActionResult> Index()
        //{
        //    WAdapter adapter = new WAdapter();
        //    var result = await adapter.GetWeather();
        //    return View(result);
        //}

        // GET: ApiController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ApiController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ApiController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ApiController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ApiController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ApiController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ApiController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
