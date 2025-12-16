using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using YnnovaComprobantes.Data;
using YnnovaComprobantes.Models;

namespace YnnovaComprobantes.Controllers
{
    

        //[Authorize]
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
                    return Json(new { data = TipoUsuarioData, message = "Usuarios retornadas exitosamente.", status = true });
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
                    if (_context.TipoUsuarios.Any(e => e.Id == TipoUsuario.Id))
                    {
                        return Json(new ApiResponse { data = null, message = "Ya existe una Tipo Usuario registrada con el código de Tipo Usuario ingresado.", status = false });
                    }

                    _context.TipoUsuarios.Add(TipoUsuario);
                    _context.SaveChanges();
                    return Json(new ApiResponse { data = null, message = "Tipo Usuarioregistrada exitosamente.", status = true });
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
                        return Json(new ApiResponse { data = null, message = "La Tipo Usuario que intenta editar no existe.", status = false });
                    }

                    if (_context.TipoUsuarios.Where(e => e.Id != TipoUsuario.Id).Any(e => e.Id == TipoUsuario.Id))
                    {
                        return Json(new ApiResponse { data = null, message = "Ya existe una Tipo Usuario registrada con el código de Tipo Usuario ingresado.", status = false });
                    }

                    _context.TipoUsuarios.Update(TipoUsuario);
                    _context.SaveChanges();
                    return Json(new ApiResponse { data = TipoUsuario, message = "Tipo Usuario actualizada exitosamente.", status = true });
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
                        return Json(new ApiResponse { data = null, message = "Tipo Usuario no encontrada.", status = false });
                    }
                    _context.TipoUsuarios.Remove(TipoUsuario);
                    _context.SaveChanges();
                    return Json(new ApiResponse { data = null, message = "Tipo Usuario eliminada exitosamente.", status = true });
                }
                catch (Exception ex)
                {
                    return Json(new ApiResponse { data = null, message = ex.Message, status = false });
                }
            }
        }

    }
