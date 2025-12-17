using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YnnovaComprobantes.Data;
using YnnovaComprobantes.Models;

namespace YnnovaComprobantes.Controllers
{


    [Authorize]
    public class TipoUsuarioController : Controller
    {
        private readonly ApplicationDbContext _context;
        public TipoUsuarioController(ApplicationDbContext context)
        {
            _context = context;
        }
        // LISTAR
        public IActionResult Index()
        {
            return View();
        }
        public JsonResult GetTipoUsuarioData()
        {
            try
            {
                var TipoUsuarioData = _context.TipoUsuarios.ToList();
                return Json(new { data = TipoUsuarioData, message = "Tipos de usuarios retornados exitosamente.", status = true });
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
        public JsonResult RegistrarTipoUsuario(TipoUsuario TipoUsuario)
        {
            try
            {
                _context.TipoUsuarios.Add(TipoUsuario);
                _context.SaveChanges();
                return Json(new ApiResponse { data = null, message = "Tipo de usuario registrado exitosamente.", status = true });
            }
            catch (Exception ex)
            {
                return Json(new ApiResponse { data = null, message = ex.Message, status = false });
            }
        }
        // VER
        public IActionResult Ver(int id)
        {
            var TipoUsuario = _context.TipoUsuarios.FirstOrDefault(e => e.Id == id);
            if (TipoUsuario == null)
            {
                return NotFound();
            }
            return View(TipoUsuario);
        }
        // EDITAR
        public IActionResult Editar(int id)
        {
            var TipoUsuario = _context.TipoUsuarios.FirstOrDefault(e => e.Id == id);
            if (TipoUsuario == null)
            {
                return NotFound();
            }
            return View(TipoUsuario);
        }
        [HttpPost]
        public JsonResult EditarTipoUsuario(TipoUsuario TipoUsuario)
        {
            try
            {
                if (!_context.TipoUsuarios.Any(e => e.Id == TipoUsuario.Id))
                {
                    return Json(new ApiResponse { data = null, message = "El tipo de usuario que intenta editar no existe.", status = false });
                }

                _context.TipoUsuarios.Update(TipoUsuario);
                _context.SaveChanges();
                return Json(new ApiResponse { data = TipoUsuario, message = "Tipo de usuario actualizado exitosamente.", status = true });
            }
            catch (Exception ex)
            {
                return Json(new ApiResponse { data = null, message = ex.Message, status = false });
            }
        }
        // ELIMINAR
        public JsonResult EliminarTipoUsuario(int id)
        {
            try
            {
                var TipoUsuario = _context.TipoUsuarios.FirstOrDefault(e => e.Id == id);
                if (TipoUsuario == null)
                {
                    return Json(new ApiResponse { data = null, message = "Tipo de usuario no encontrado.", status = false });
                }

                if (_context.EmpresasUsuarios.Any(eu => eu.TipoUsuarioId == id))
                {
                    return Json(new ApiResponse { data = null, message = "El tipo de usuario no se puede eliminar porque ya está referenciado.", status = false });
                }

                _context.TipoUsuarios.Remove(TipoUsuario);
                _context.SaveChanges();
                return Json(new ApiResponse { data = null, message = "Tipo de usuario eliminado exitosamente.", status = true });
            }
            catch (Exception ex)
            {
                return Json(new ApiResponse { data = null, message = ex.Message, status = false });
            }
        }
    }

}
