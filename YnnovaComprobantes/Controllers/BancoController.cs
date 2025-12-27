using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YnnovaComprobantes.Data;
using YnnovaComprobantes.Models;

namespace YnnovaComprobantes.Controllers
{
    [Authorize]
    public class BancoController : Controller
    {
        private readonly ApplicationDbContext _context;
        public BancoController(ApplicationDbContext context)
        {
            _context = context;
        }
        // LISTAR
        public IActionResult Index()
        {
            return View();
        }
        public JsonResult GetBancoData()
        {
            try
            {
                var bancoData = _context.Bancos.ToList();
                return Json(new { data = bancoData, message = "Bancos retornados exitosamente.", status = true });
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
        public JsonResult RegistrarBanco(Banco banco)
        {
            try
            {
                _context.Bancos.Add(banco);
                _context.SaveChanges();
                return Json(new ApiResponse { data = null, message = "Banco registrado exitosamente.", status = true });
            }
            catch (Exception ex)
            {
                return Json(new ApiResponse { data = null, message = ex.Message, status = false });
            }
        }
        // VER
        public IActionResult Ver(int id)
        {
            var banco = _context.Bancos.FirstOrDefault(e => e.Id == id);
            if (banco == null)
            {
                return NotFound();
            }
            return View(banco);
        }
        // EDITAR
        public IActionResult Editar(int id)
        {
            var banco = _context.Bancos.FirstOrDefault(e => e.Id == id);
            if (banco == null)
            {
                return NotFound();
            }
            return View(banco);
        }
        [HttpPost]
        public JsonResult EditarBanco(Banco banco)
        {
            try
            {
                if (!_context.Bancos.Any(e => e.Id == banco.Id))
                {
                    return Json(new ApiResponse { data = null, message = "El banco que intenta editar no existe.", status = false });
                }

                if (_context.Bancos.Where(b => b.Id != banco.Id).Any(b => b.Codigo == banco.Codigo))
                {
                    return Json(new ApiResponse { data = null, message = "Ya existe un banco registrada con el código de banco ingresado.", status = false });
                }

                _context.Bancos.Update(banco);
                _context.SaveChanges();
                return Json(new ApiResponse { data = banco, message = "Banco actualizado exitosamente.", status = true });
            }
            catch (Exception ex)
            {
                return Json(new ApiResponse { data = null, message = ex.Message, status = false });
            }
        }
        // ELIMINAR
        public JsonResult EliminarBanco(int id)
        {
            try
            {
                var banco = _context.Bancos.FirstOrDefault(e => e.Id == id);
                if (banco == null)
                {
                    return Json(new ApiResponse { data = null, message = "Banco no encontrado.", status = false });
                }

                if (_context.Anticipos.Any(g => g.BancoId == id))
                {
                    return Json(new ApiResponse { data = null, message = "El banco no se puede eliminar porque ya está referenciado.", status = false });
                }

                _context.Bancos.Remove(banco);
                _context.SaveChanges();
                return Json(new ApiResponse { data = null, message = "Banco eliminada exitosamente.", status = true });
            }
            catch (Exception ex)
            {
                return Json(new ApiResponse { data = null, message = ex.Message, status = false });
            }
        }
    }
}
