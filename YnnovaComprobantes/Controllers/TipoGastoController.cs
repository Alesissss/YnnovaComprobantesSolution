using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using YnnovaComprobantes.Data;
using YnnovaComprobantes.Models;

namespace YnnovaComprobantes.Controllers
{


    //[Authorize]
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
        public JsonResult GetTipoGastoData()
        {
            try
            {
                var TipoGastoData = _context.TipoGastos.ToList();
                return Json(new { data = TipoGastoData, message = "Tipos de gastos retornados exitosamente.", status = true });
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
        public JsonResult RegistrarTipoGasto(TipoGasto TipoGasto)
        {
            try
            {
                _context.TipoGastos.Add(TipoGasto);
                _context.SaveChanges();
                return Json(new ApiResponse { data = null, message = "Tipo de gasto registrado exitosamente.", status = true });
            }
            catch (Exception ex)
            {
                return Json(new ApiResponse { data = null, message = ex.Message, status = false });
            }
        }
        // VER
        public IActionResult Ver(int id)
        {
            var TipoGasto = _context.TipoGastos.FirstOrDefault(e => e.Id == id);
            if (TipoGasto == null)
            {
                return NotFound();
            }
            return View(TipoGasto);
        }
        // EDITAR
        public IActionResult Editar(int id)
        {
            var TipoGasto = _context.TipoGastos.FirstOrDefault(e => e.Id == id);
            if (TipoGasto == null)
            {
                return NotFound();
            }
            return View(TipoGasto);
        }
        [HttpPost]
        public JsonResult EditarTipoGasto(TipoGasto TipoGasto)
        {
            try
            {
                if (!_context.TipoGastos.Any(e => e.Id == TipoGasto.Id))
                {
                    return Json(new ApiResponse { data = null, message = "El tipo de gasto que intenta editar no existe.", status = false });
                }

                _context.TipoGastos.Update(TipoGasto);
                _context.SaveChanges();
                return Json(new ApiResponse { data = TipoGasto, message = "Tipo de gasto actualizada exitosamente.", status = true });
            }
            catch (Exception ex)
            {
                return Json(new ApiResponse { data = null, message = ex.Message, status = false });
            }
        }
        // ELIMINAR
        public JsonResult EliminarTipoGasto(int id)
        {
            try
            {
                var TipoGasto = _context.TipoGastos.FirstOrDefault(e => e.Id == id);
                if (TipoGasto == null)
                {
                    return Json(new ApiResponse { data = null, message = "Tipo de gasto no encontrado.", status = false });
                }

                if (_context.Gastos.Any(g => g.TipoGastoId == id))
                {
                    return Json(new ApiResponse { data = null, message = "El tipo de gasto no se puede eliminar porque ya está referenciado.", status = false });
                }

                _context.TipoGastos.Remove(TipoGasto);
                _context.SaveChanges();
                return Json(new ApiResponse { data = null, message = "Tipo de gasto eliminado exitosamente.", status = true });
            }
            catch (Exception ex)
            {
                return Json(new ApiResponse { data = null, message = ex.Message, status = false });
            }
        }
    }

}

