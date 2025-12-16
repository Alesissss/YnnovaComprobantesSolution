using Microsoft.AspNetCore.Mvc;
using YnnovaComprobantes.Data;
using YnnovaComprobantes.Models;

namespace YnnovaComprobantes.Controllers
{
    public class TipoRendicionController : Controller
    {
        private readonly ApplicationDbContext _context;
        public TipoRendicionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // LISTAR
        public IActionResult Index()
        {
            return View();
        }
        public JsonResult GetTipoRendicionData()
        {
            try
            {
                var TipoRendicionData = _context.TipoRendiciones.ToList();
                return Json(new { data = TipoRendicionData, message = "Tipos de rendicion retornados exitosamente.", status = true });
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
        public JsonResult RegistrarTipoRendicion(TipoRendicion TipoRendicion)
        {
            try
            {
                if (_context.Empresas.Any(e => e.Ruc == TipoRendicion.Codigo))
                {
                    return Json(new ApiResponse { data = null, message = "Ya existe un tipo Rendicion registrado con el código ingresado.", status = false });
                }

                _context.TipoRendiciones.Add(TipoRendicion);
                _context.SaveChanges();
                return Json(new ApiResponse { data = null, message = "Tipo de rendicion registrado exitosamente.", status = true });
            }
            catch (Exception ex)
            {
                return Json(new ApiResponse { data = null, message = ex.Message, status = false });
            }
        }
        // VER
        public IActionResult Ver(int id)
        {
            var TipoRendicion = _context.TipoRendiciones.FirstOrDefault(e => e.Id == id);
            if (TipoRendicion == null)
            {
                return NotFound();
            }
            return View(TipoRendicion);
        }
        // EDITAR
        public IActionResult Editar(int id)
        {
            var TipoRendicion = _context.TipoRendiciones.FirstOrDefault(e => e.Id == id);
            if (TipoRendicion == null)
            {
                return NotFound();
            }
            return View(TipoRendicion);
        }
        [HttpPost]
        public JsonResult EditarTipoRendicion(TipoRendicion TipoRendicion)
        {
            try
            {
                if (!_context.TipoRendiciones.Any(e => e.Id == TipoRendicion.Id))
                {
                    return Json(new ApiResponse { data = null, message = "El tipo de rendicion que intenta editar no existe.", status = false });
                }

                if (_context.Empresas.Where(e => e.Id != TipoRendicion.Id).Any(e => e.Ruc == TipoRendicion.Codigo))
                {
                    return Json(new ApiResponse { data = null, message = "Ya existe un tipo de rendicion registrado con el código ingresado.", status = false });
                }

                _context.TipoRendiciones.Update(TipoRendicion);
                _context.SaveChanges();
                return Json(new ApiResponse { data = TipoRendicion, message = "Tipo de rendicion actualizado exitosamente.", status = true });
            }
            catch (Exception ex)
            {
                return Json(new ApiResponse { data = null, message = ex.Message, status = false });
            }
        }
        // ELIMINAR
        public JsonResult EliminarTipoRendicion(int id)
        {
            try
            {
                var TipoRendicion = _context.TipoRendiciones.FirstOrDefault(e => e.Id == id);
                if (TipoRendicion == null)
                {
                    return Json(new ApiResponse { data = null, message = "Tipo de rendicion no encontrada.", status = false });
                }
                _context.TipoRendiciones.Remove(TipoRendicion);
                _context.SaveChanges();
                return Json(new ApiResponse { data = null, message = "Tipo de rendicion eliminada exitosamente.", status = true });
            }
            catch (Exception ex)
            {
                return Json(new ApiResponse { data = null, message = ex.Message, status = false });
            }
        }


    }
}
