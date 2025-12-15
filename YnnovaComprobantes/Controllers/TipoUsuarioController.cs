using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace YnnovaComprobantes.Controllers
{
    public class TipoUsuarioController : Controller
    {
        // GET: TipoUsuarioController
        public ActionResult Index()
        {
            return View();
        }

        // GET: TipoUsuarioController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: TipoUsuarioController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TipoUsuarioController/Create
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

        // GET: TipoUsuarioController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: TipoUsuarioController/Edit/5
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

        // GET: TipoUsuarioController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: TipoUsuarioController/Delete/5
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
