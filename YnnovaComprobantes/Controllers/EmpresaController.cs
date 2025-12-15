using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YnnovaComprobantes.Data;
using YnnovaComprobantes.Models;

namespace YnnovaComprobantes.Controllers
{
    //[Authorize]
    public class EmpresaController : Controller
    {
        private readonly ApplicationDbContext _context;
        public EmpresaController(ApplicationDbContext context)
        {
            _context = context;
        }
        // LISTAR
        public IActionResult Index()
        {
            return View();
        }
        public JsonResult GetEmpresaData()
        {
            try
            {
                var empresaData = _context.Empresas.ToList();
                return Json(new { data = empresaData, message = "Empresas retornadas exitosamente.", status = true });
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
        public JsonResult RegistrarEmpresa(Empresa empresa)
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
        public JsonResult EditarEmpresa(Empresa empresa)
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
        public JsonResult EliminarEmpresa(int id)
        {
            try
            {
                var empresa = _context.Empresas.FirstOrDefault(e => e.Id == id);
                if (empresa == null)
                {
                    return Json(new ApiResponse { data = null, message = "Empresa no encontrada.", status = false });
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
