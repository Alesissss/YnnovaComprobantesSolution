using Microsoft.AspNetCore.Mvc;
using YnnovaComprobantes.Data;
using YnnovaComprobantes.Models;

namespace YnnovaComprobantes.Controllers
{
    public class GastoController : Controller
    {
        private readonly ApplicationDbContext _context;
        public GastoController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Registrar()
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
        // Get tipos de gastos
        [HttpGet]
        public JsonResult GetTiposGastosData()
        {
            try
            {
                var tiposGastos = _context.TipoGastos.ToList();
                return Json(new { data = tiposGastos, message = "Tipos de gastos retornados exitosamente.", status = true });
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
                var usuarios = (from u in _context.Usuarios
                                from eu in _context.EmpresasUsuarios
                                where u.Id == eu.UsuarioId
                                where eu.EmpresaId == EmpresaId
                                where u.Estado == true
                                select new
                                {
                                    u.Id,
                                    u.Dni,
                                    u.Nombre,
                                }).ToList();
                return Json(new { data = usuarios, message = "Usuarios retornados exitosamente.", status = true });
            }
            catch (Exception ex)
            {
                return Json(new ApiResponse { data = null, message = ex.Message, status = false });
            }
        }
        // Get bancos
        [HttpGet]
        public JsonResult GetBancosData()
        {
            try
            {
                var bancos = _context.Bancos.Where(b => b.Estado == true).ToList();
                return Json(new { data = bancos, message = "Bancos retornados exitosamente.", status = true });
            }
            catch (Exception ex)
            {
                return Json(new ApiResponse { data = null, message = ex.Message, status = false });
            }
        }
        // Get tipos de rendiciones
        [HttpGet]
        public JsonResult GetTiposRendicionesData()
        {
            try
            {
                var tiposRendiciones = _context.TipoRendiciones.Where(tr => tr.Estado == true).ToList();
                return Json(new { data = tiposRendiciones, message = "Tipos de rendiciones retornados exitosamente.", status = true });
            }
            catch (Exception ex)
            {
                return Json(new ApiResponse { data = null, message = ex.Message, status = false });
            }
        }
    }
}
