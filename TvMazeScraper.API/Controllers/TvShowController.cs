using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TvMazeScraper.API.Controllers
{
    public class TvShowController : Controller
    {
        // GET: TvShowController
        public ActionResult Index()
        {
            return View();
        }

        // GET: TvShowController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: TvShowController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TvShowController/Create
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

        // GET: TvShowController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: TvShowController/Edit/5
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

        // GET: TvShowController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: TvShowController/Delete/5
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
