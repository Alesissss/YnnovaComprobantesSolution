using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YnnovaComprobantes.Data;
using YnnovaComprobantes.Models;

namespace YnnovaComprobantes.Controllers
{
    [Authorize]
    public class TipoComprobanteController : Controller
    {
        private readonly ApplicationDbContext _context;
        public TipoComprobanteController(ApplicationDbContext context)
        {
            _context = context;
        }

        // LISTAR
        public IActionResult Index()
        {
            return View();
        }
        public JsonResult GetTipoComprobanteData()
        {
            try
            {
                var TipoComprobanteData = _context.TipoComprobantes.ToList();
                return Json(new { data = TipoComprobanteData, message = "Tipos de comprobantes retornados exitosamente.", status = true });
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
        public JsonResult RegistrarTipoComprobante(TipoComprobante TipoComprobante)
        {
            try
            {
                if (_context.TipoComprobantes.Any(tc => tc.Codigo == TipoComprobante.Codigo))
                {
                    return Json(new ApiResponse { data = null, message = "Ya existe un tipo de comprobante registrado con el código ingresado.", status = false });
                }

                _context.TipoComprobantes.Add(TipoComprobante);
                _context.SaveChanges();
                return Json(new ApiResponse { data = null, message = "Tipo de comprobante registrado exitosamente.", status = true });
            }
            catch (Exception ex)
            {
                return Json(new ApiResponse { data = null, message = ex.Message, status = false });
            }
        }
        // VER
        public IActionResult Ver(int id)
        {
            var TipoComprobante = _context.TipoComprobantes.FirstOrDefault(e => e.Id == id);
            if (TipoComprobante == null)
            {
                return NotFound();
            }
            return View(TipoComprobante);
        }
        // EDITAR
        public IActionResult Editar(int id)
        {
            var TipoComprobante = _context.TipoComprobantes.FirstOrDefault(e => e.Id == id);
            if (TipoComprobante == null)
            {
                return NotFound();
            }
            return View(TipoComprobante);
        }
        [HttpPost]
        public JsonResult EditarTipoComprobante(TipoComprobante TipoComprobante)
        {
            try
            {
                if (!_context.TipoComprobantes.Any(e => e.Id == TipoComprobante.Id))
                {
                    return Json(new ApiResponse { data = null, message = "El tipo de comprobante que intenta editar no existe.", status = false });
                }

                if (_context.TipoComprobantes.Where(tc => tc.Id != TipoComprobante.Id).Any(tc => tc.Codigo == TipoComprobante.Codigo))
                {
                    return Json(new ApiResponse { data = null, message = "Ya existe un tipo de comprobante registrado con el código ingresado.", status = false });
                }

                _context.TipoComprobantes.Update(TipoComprobante);
                _context.SaveChanges();
                return Json(new ApiResponse { data = TipoComprobante, message = "Tipo de comprobante actualizado exitosamente.", status = true });
            }
            catch (Exception ex)
            {
                return Json(new ApiResponse { data = null, message = ex.Message, status = false });
            }
        }
        // ELIMINAR
        public JsonResult EliminarTipoComprobante(int id)
        {
            try
            {
                var TipoComprobante = _context.TipoComprobantes.FirstOrDefault(e => e.Id == id);
                if (TipoComprobante == null)
                {
                    return Json(new ApiResponse { data = null, message = "Tipo de comprobante no encontrada.", status = false });
                }

                if (_context.Comprobantes.Any(c => c.TipoComprobanteId == id))
                {
                    return Json(new ApiResponse { data = null, message = "El tipo de comprobante no se puede eliminar porque ya está referenciado.", status = false });
                }

                _context.TipoComprobantes.Remove(TipoComprobante);
                _context.SaveChanges();
                return Json(new ApiResponse { data = null, message = "Tipo de comprobante eliminada exitosamente.", status = true });
            }
            catch (Exception ex)
            {
                return Json(new ApiResponse { data = null, message = ex.Message, status = false });
            }
        }


    }
}
