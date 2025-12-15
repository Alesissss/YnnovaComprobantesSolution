using Microsoft.AspNetCore.Mvc;
using YnnovaComprobantes.Data;
using YnnovaComprobantes.Models;

namespace YnnovaComprobantes.Controllers
{
    public class TipoGastoController : Controller
    {
        private readonly ApplicationDbContext _context;
        public TipoGastoController(ApplicationDbContext context)
        {
            _context = context;
        }
        // LISTAR
        public IActionResult Index()
        {
            return View();
        }
        public JsonResult GetTipoGasto()
        {
            try
            {
                var TipoGasto = _context.TipoGastos.ToList();
                return Json(new { data = TipoGasto, message = "Tipos de gastos recuperados exitosamente.", status = true });
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
                if (_context.Empresas.Any(e => e.Ruc == TipoComprobante.Codigo))
                {
                    return Json(new ApiResponse { data = null, message = "Ya existe ese tipo comprobante registrada.", status = false });
                }

                _context.TipoComprobantes.Add(TipoComprobante);
                _context.SaveChanges();
                return Json(new ApiResponse { data = null, message = "Comprobante registrado exitosamente.", status = true });
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
                    return Json(new ApiResponse { data = null, message = "El comprobante que intenta editar no existe.", status = false });
                }

                if (_context.Empresas.Where(e => e.Id != TipoComprobante.Id).Any(e => e.Ruc == TipoComprobante.Codigo))
                {
                    return Json(new ApiResponse { data = null, message = "Ya existe un comprobante registrada con el código  ingresado.", status = false });
                }

                _context.TipoComprobantes.Update(TipoComprobante);
                _context.SaveChanges();
                return Json(new ApiResponse { data = TipoComprobante, message = "Comprobante actualizadO exitosamente.", status = true });
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
                    return Json(new ApiResponse { data = null, message = "Comprobante no encontrada.", status = false });
                }
                _context.TipoComprobantes.Remove(TipoComprobante);
                _context.SaveChanges();
                return Json(new ApiResponse { data = null, message = "Comprobante eliminada exitosamente.", status = true });
            }
            catch (Exception ex)
            {
                return Json(new ApiResponse { data = null, message = ex.Message, status = false });
            }
        }
    }
}
