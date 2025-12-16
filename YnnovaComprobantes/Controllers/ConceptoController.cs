using Microsoft.AspNetCore.Mvc;
using YnnovaComprobantes.Data;
using YnnovaComprobantes.Models;

namespace YnnovaComprobantes.Controllers
{
    public class ConceptoController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ConceptoController(ApplicationDbContext context)
        {
            _context = context;
        }
        // LISTAR
        public IActionResult Index()
        {
            return View();
        }
        public JsonResult GetConceptoData()
        {
            try
            {
                var conceptoData = _context.Conceptos.ToList();
                return Json(new { data = conceptoData, message = "Conceptos retornados exitosamente.", status = true });
            }
            catch (Exception ex)
            {
                return Json(new ApiResponse { data = null, message = ex.Message, status = false });
            }
        }

        // REGISTRAR
        public IActionResult Registrar()
        {
            return View();
        }
        [HttpPost]
        public JsonResult RegistrarConcepto(Concepto concepto)
        {
            try
            {
                if (_context.Conceptos.Any(c => c.Nombre == concepto.Nombre))
                {
                    return Json(new ApiResponse { data = null, message = "Ya existe un concepto registrado con el nombre de concepto ingresado.", status = false });
                }

                _context.Conceptos.Add(concepto);
                _context.SaveChanges();
                return Json(new ApiResponse { data = null, message = "Concepto registrado exitosamente.", status = true });
            }
            catch (Exception ex)
            {
                return Json(new ApiResponse { data = null, message = ex.Message, status = false });
            }
        }
        // VER
        public IActionResult Ver(int id)
        {
            var concepto = _context.Conceptos.FirstOrDefault(c => c.Id == id);
            if (concepto == null)
            {
                return NotFound();
            }
            return View(concepto);
        }
        // EDITAR
        public IActionResult Editar(int id)
        {
            var concepto = _context.Conceptos.FirstOrDefault(c => c.Id == id);
            if (concepto == null)
            {
                return NotFound();
            }
            return View(concepto);
        }
        [HttpPost]
        public JsonResult EditarConcepto(Concepto concepto)
        {
            try
            {
                if (!_context.Conceptos.Any(c => c.Id == concepto.Id))
                {
                    return Json(new ApiResponse { data = null, message = "El concepto que intenta editar no existe.", status = false });
                }

                if (_context.Conceptos.Where(c => c.Id != concepto.Id).Any(c => c.Nombre == concepto.Nombre))
                {
                    return Json(new ApiResponse { data = null, message = "Ya existe un concepto con el nombre de concepto ingresado.", status = false });
                }

                _context.Conceptos.Update(concepto);
                _context.SaveChanges();
                return Json(new ApiResponse { data = concepto, message = "Concepto actualizado exitosamente.", status = true });
            }
            catch (Exception ex)
            {
                return Json(new ApiResponse { data = null, message = ex.Message, status = false });
            }
        }
        // ELIMINAR
        public JsonResult EliminarConcepto(int id)
        {
            try
            {
                var concepto = _context.Conceptos.FirstOrDefault(e => e.Id == id);
                if (concepto == null)
                {
                    return Json(new ApiResponse { data = null, message = "Concepto no encontrado.", status = false });
                }

                if (_context.Comprobantes.Any(c => c.ConceptoId == id)) 
                {
                    return Json(new ApiResponse { data = null, message = "El concepto no se puede eliminar porque ya está referenciado.", status = false });
                }

                _context.Conceptos.Remove(concepto);
                _context.SaveChanges();
                return Json(new ApiResponse { data = null, message = "Concepto eliminado exitosamente.", status = true });
            }
            catch (Exception ex)
            {
                return Json(new ApiResponse { data = null, message = ex.Message, status = false });
            }
        }
    }
}
