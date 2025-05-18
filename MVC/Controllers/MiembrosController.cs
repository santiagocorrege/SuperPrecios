using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MVC.Controllers
{
    public class MiembrosController : Controller
    {
        // GET: MiembrosController
        public ActionResult Index()
        {
            return View();
        }

        // GET: MiembrosController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: MiembrosController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MiembrosController/Create
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

        // GET: MiembrosController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: MiembrosController/Edit/5
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

        // GET: MiembrosController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: MiembrosController/Delete/5
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
