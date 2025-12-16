using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YnnovaComprobantes.Data;
using YnnovaComprobantes.Models;

namespace YnnovaComprobantes.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly ApplicationDbContext _context;
        public UsuarioController(ApplicationDbContext context)
        {
            _context = context;
        }
        // LISTAR
        public IActionResult Index()
        {
            return View();
        }
        // Get empresas
        [HttpGet]
        public JsonResult GetEmpresaData()
        {
            try
            {
                var empresas = (from e in _context.Empresas
                                where e.Estado == true
                                select new
                                {
                                    e.Id,
                                    e.Ruc,
                                    e.Nombre,
                                }).ToList();
                return Json(new { data = empresas, message = "Empresas retornadas exitosamente.", status = true });
            }
            catch (Exception ex)
            {
                return Json(new ApiResponse { data = null, message = ex.Message, status = false });
            }
        }
        // Get usuarios por empresa
        [HttpGet]
        public JsonResult GetUsuariosData(int EmpresaId)
        {
            try
            {
                var usuarios = (from eu in _context.EmpresasUsuarios
                                join u in _context.Usuarios on eu.UsuarioId equals u.Id
                                join tu in _context.TipoUsuarios on eu.TipoUsuarioId equals tu.Id
                                join e in _context.Empresas on eu.EmpresaId equals e.Id
                                where eu.EmpresaId == EmpresaId
                                select new
                                {
                                    u.Id,
                                    u.Dni,
                                    u.Nombre,
                                    u.Email,
                                    u.Telefono,
                                    u.Estado,
                                    TipoUsuarioId = tu.Id,
                                    TipoUsuario = tu.Nombre,
                                    EmpresaId,
                                    EmpresaNombre = e.Nombre,
                                    EmpresaRuc = e.Ruc,
                                }).ToList();
                return Json(new { data = usuarios, message = "Usuarios retornados exitosamente.", status = true });
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
        public JsonResult RegistrarUsuario(Empresa empresa)
        {
            try
            {
                if (_context.Empresas.Any(e => e.Ruc == empresa.Ruc))
                {
                    return Json(new ApiResponse { data = null, message = "Ya existe una empresa registrada con el código de empresa ingresado.", status = false });
                }

                _context.Empresas.Add(empresa);
                _context.SaveChanges();
                return Json(new ApiResponse { data = null, message = "Empresa registrada exitosamente.", status = true });
            }
            catch (Exception ex)
            {
                return Json(new ApiResponse { data = null, message = ex.Message, status = false });
            }
        }
        // VER
        public IActionResult Ver(int id)
        {
            var empresa = _context.Empresas.FirstOrDefault(e => e.Id == id);
            if (empresa == null)
            {
                return NotFound();
            }
            return View(empresa);
        }
        // EDITAR
        public IActionResult Editar(int id)
        {
            var empresa = _context.Empresas.FirstOrDefault(e => e.Id == id);
            if (empresa == null)
            {
                return NotFound();
            }
            return View(empresa);
        }
        [HttpPost]
        public JsonResult EditarUsuario(Empresa empresa)
        {
            try
            {
                if (!_context.Empresas.Any(e => e.Id == empresa.Id))
                {
                    return Json(new ApiResponse { data = null, message = "La empresa que intenta editar no existe.", status = false });
                }

                if (_context.Empresas.Where(e => e.Id != empresa.Id).Any(e => e.Ruc == empresa.Ruc))
                {
                    return Json(new ApiResponse { data = null, message = "Ya existe una empresa registrada con el código de empresa ingresado.", status = false });
                }

                _context.Empresas.Update(empresa);
                _context.SaveChanges();
                return Json(new ApiResponse { data = empresa, message = "Empresa actualizada exitosamente.", status = true });
            }
            catch (Exception ex)
            {
                return Json(new ApiResponse { data = null, message = ex.Message, status = false });
            }
        }
        // ELIMINAR
        public JsonResult EliminarUsuario(int id)
        {
            try
            {
                var empresa = _context.Empresas.FirstOrDefault(e => e.Id == id);
                if (empresa == null)
                {
                    return Json(new ApiResponse { data = null, message = "Empresa no encontrada.", status = false });
                }

                if (_context.Gastos.Any(g => g.EmpresaId == id) || _context.EmpresasUsuarios.Any(eu => eu.EmpresaId == id))
                {
                    return Json(new ApiResponse { data = null, message = "La empresa no se puede eliminar porque ya está referenciada.", status = false });
                }

                _context.Empresas.Remove(empresa);
                _context.SaveChanges();
                return Json(new ApiResponse { data = null, message = "Empresa eliminada exitosamente.", status = true });
            }
            catch (Exception ex)
            {
                return Json(new ApiResponse { data = null, message = ex.Message, status = false });
            }
        }
    }
}
