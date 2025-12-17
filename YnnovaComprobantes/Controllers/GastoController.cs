using Microsoft.AspNetCore.Mvc;
using YnnovaComprobantes.Data;
using YnnovaComprobantes.Models;
using YnnovaComprobantes.ViewModels;

namespace YnnovaComprobantes.Controllers
{
    public class GastoController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public GastoController(IWebHostEnvironment hostingEnvironment, ApplicationDbContext context)
        {
            _hostingEnvironment = hostingEnvironment;
            _context = context;
        }
        // Listar
        public IActionResult Index()
        {
            return View();
        }
        public JsonResult GetGastosData()
        {
            try
            {
                var gastos = (from g in _context.Gastos
                              join tg in _context.TipoGastos on g.TipoGastoId equals tg.Id
                              join e in _context.Empresas on g.EmpresaId equals e.Id
                              join u in _context.Usuarios on g.UsuarioId equals u.Id
                              join est in _context.Estados on g.EstadoId equals est.Id
                              join bLeft in _context.Bancos on g.BancoId equals bLeft.Id into BancoGroup
                              from b in BancoGroup.DefaultIfEmpty()
                              join trLeft in _context.TipoRendiciones on g.TipoRendicionId equals trLeft.Id into TipoRendicionGroup
                              from tr in TipoRendicionGroup.DefaultIfEmpty()
                              join moLeft in _context.Monedas on g.MonedaId equals moLeft.Id into MonedaGroup
                              from mo in MonedaGroup.DefaultIfEmpty()
                              orderby g.FechaRegistro descending
                              select new
                              {
                                  g.Id,
                                  g.Fecha,
                                  Empresa = e.Ruc + " - " + e.Nombre,
                                  Banco = b == null ? "Sin banco" : b.Descripcion,
                                  TipoRendicion = tr == null ? "Sin tipo rendición" : tr.Descripcion,
                                  Usuario = u.Nombre,
                                  TipoGasto = tg.Nombre,
                                  Moneda = mo == null ? "Sin moneda" : mo.Nombre + " (" + mo.Simbolo + ")",
                                  g.Importe,
                                  Estado = est.Nombre,
                                  Descripcion = g.Descripcion ?? "Sin descripción",
                              }).ToList();
                return Json(new { data = gastos, message = "Gastos retornados exitosamente.", status = true });
            }
            catch (Exception ex)
            {
                return Json(new ApiResponse { data = null, message = ex.Message, status = false });
            }
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
        // Get monedas
        [HttpGet]
        public JsonResult GetMonedasData()
        {
            try
            {
                var tiposRendiciones = _context.Monedas.ToList();
                return Json(new { data = tiposRendiciones, message = "Monedas retornadas exitosamente.", status = true });
            }
            catch (Exception ex)
            {
                return Json(new ApiResponse { data = null, message = ex.Message, status = false });
            }
        }
        // Get tipos comprobantes
        [HttpGet]
        public JsonResult GetTipoComprobanteData()
        {
            try
            {
                var tiposComprobantes = _context.TipoComprobantes.Where(tc => tc.Estado == true).ToList();
                return Json(new { data = tiposComprobantes, message = "Tipos de comprobantes retornados exitosamente.", status = true });
            }
            catch (Exception ex)
            {
                return Json(new ApiResponse { data = null, message = ex.Message, status = false });
            }
        }
        // Registrar gasto
        [HttpPost]
        public JsonResult RegistrarGasto(Gasto gasto)
        {
            try
            {
                gasto.EstadoId = _context.Estados.Where(e => e.Tabla == "GASTO" && e.Nombre == "Pendiente").Select(e => e.Id).FirstOrDefault();
                gasto.FechaRegistro = DateTime.Now;

                _context.Gastos.Add(gasto);
                _context.SaveChanges();

                return Json(new ApiResponse { data = null, message = "Gasto registrado exitosamente.", status = true });
            }
            catch (Exception ex)
            {
                return Json(new ApiResponse { data = null, message = ex.Message, status = false });
            }
        }
        public IActionResult MisGastos()
        {
            return View();
        }
        public JsonResult GetMisGastosData()
        {
            try
            {
                //string? userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

                //if (userIdString == null)
                //{
                //    return Json(new ApiResponse { data = null, message = "El usuario no se encuentra logueado.", status = false });
                //}

                //int userId = int.Parse(userIdString.Trim());

                var gastos = (from g in _context.Gastos
                              join tg in _context.TipoGastos on g.TipoGastoId equals tg.Id
                              join e in _context.Empresas on g.EmpresaId equals e.Id
                              join u in _context.Usuarios on g.UsuarioId equals u.Id
                              join est in _context.Estados on g.EstadoId equals est.Id
                              join bLeft in _context.Bancos on g.BancoId equals bLeft.Id into BancoGroup
                              from b in BancoGroup.DefaultIfEmpty()
                              join trLeft in _context.TipoRendiciones on g.TipoRendicionId equals trLeft.Id into TipoRendicionGroup
                              from tr in TipoRendicionGroup.DefaultIfEmpty()
                              join moLeft in _context.Monedas on g.MonedaId equals moLeft.Id into MonedaGroup
                              from mo in MonedaGroup.DefaultIfEmpty()
                              orderby g.FechaRegistro descending
                              //where g.UsuarioId == userId
                              select new
                              {
                                  g.Id,
                                  g.Fecha,
                                  Empresa = e.Ruc + " - " + e.Nombre,
                                  Banco = b == null ? "Sin banco" : b.Descripcion,
                                  TipoRendicion = tr == null ? "Sin tipo rendición" : tr.Descripcion,
                                  Usuario = u.Nombre,
                                  TipoGasto = tg.Nombre,
                                  Moneda = mo == null ? "Sin moneda" : mo.Nombre + " (" + mo.Simbolo + ")",
                                  g.Importe,
                                  Estado = est.Nombre,
                                  Descripcion = g.Descripcion ?? "Sin descripción",
                              }).ToList();
                return Json(new { data = gastos, message = "Gastos retornados exitosamente.", status = true });
            }
            catch (Exception ex)
            {
                return Json(new ApiResponse { data = null, message = ex.Message, status = false });
            }
        }
        public IActionResult SubirComprobante(int id)
        {
            var gasto = (from g in _context.Gastos
                         join tg in _context.TipoGastos on g.TipoGastoId equals tg.Id
                         join e in _context.Empresas on g.EmpresaId equals e.Id
                         join u in _context.Usuarios on g.UsuarioId equals u.Id
                         join est in _context.Estados on g.EstadoId equals est.Id
                         join bLeft in _context.Bancos on g.BancoId equals bLeft.Id into BancoGroup
                         from b in BancoGroup.DefaultIfEmpty()
                         join trLeft in _context.TipoRendiciones on g.TipoRendicionId equals trLeft.Id into TipoRendicionGroup
                         from tr in TipoRendicionGroup.DefaultIfEmpty()
                         join moLeft in _context.Monedas on g.MonedaId equals moLeft.Id into MonedaGroup
                         from mo in MonedaGroup.DefaultIfEmpty()
                         where g.Id == id
                         select new GastoViewModel
                         {
                             Id = g.Id,
                             Fecha = g.Fecha,
                             Empresa = e.Ruc + " - " + e.Nombre,
                             Banco = b == null ? "Sin banco" : b.Descripcion,
                             TipoRendicion = tr == null ? "Sin tipo rendición" : tr.Descripcion,
                             Usuario = u.Nombre,
                             TipoGasto = tg.Nombre,
                             MonedaNombre = mo == null ? "Sin moneda" : mo.Nombre,
                             MonedaSimbolo = mo == null ? "Sin moneda" : mo.Simbolo,
                             Importe = g.Importe,
                             Estado = est.Nombre,
                             Descripcion = g.Descripcion ?? "Sin descripción",
                             EmpresaId = g.EmpresaId,
                             BancoId = g.BancoId,
                             TipoRendicionId = g.TipoRendicionId,
                             UsuarioId = g.UsuarioId,
                             TipoGastoId = g.TipoGastoId,
                             MonedaId = g.MonedaId,
                             EstadoId = g.EstadoId,
                             FechaRegistro = g.FechaRegistro,
                         }).FirstOrDefault();
            if (gasto == null)
            {
                return NotFound();
            }
            return View(gasto);
        }
        public JsonResult GetConceptoData()
        {
            try
            {
                var conceptoData = _context.Conceptos.Where(c => c.Estado == true).ToList();
                return Json(new { data = conceptoData, message = "Conceptos retornados exitosamente.", status = true });
            }
            catch (Exception ex)
            {
                return Json(new ApiResponse { data = null, message = ex.Message, status = false });
            }
        }
        [HttpPost]
        public async Task<JsonResult> RegistrarComprobante(Comprobante comprobante, IFormFile ArchivoComprobante)
        {
            if (ArchivoComprobante == null || ArchivoComprobante.Length == 0)
            {
                return Json(new ApiResponse { data = null, message = "Error: El archivo comprobante no fue proporcionado.", status = false });
            }

            string urlRelativaDB = string.Empty;

            try
            {
                var carpetaUploads = Path.Combine(_hostingEnvironment.WebRootPath, "uploads", "comprobantes");

                if (!Directory.Exists(carpetaUploads))
                {
                    Directory.CreateDirectory(carpetaUploads);
                }

                var extension = Path.GetExtension(ArchivoComprobante.FileName);
                var nombreUnico = $"{Guid.NewGuid()}{extension}";
                var rutaCompletaArchivo = Path.Combine(carpetaUploads, nombreUnico);

                using (var stream = new FileStream(rutaCompletaArchivo, FileMode.Create))
                {
                    await ArchivoComprobante.CopyToAsync(stream);
                }

                urlRelativaDB = Path.Combine("/uploads/comprobantes", nombreUnico).Replace("\\", "/");

            }
            catch (Exception ex)
            {
                return Json(new ApiResponse { data = null, message = $"Error al guardar el archivo: {ex.Message}", status = false });
            }

            try
            {
                comprobante.FechaRegistro = DateTime.Now;
                comprobante.Archivo = urlRelativaDB;

                comprobante.EstadoId = _context.Estados
                    .Where(e => e.Tabla == "COMPROBANTE" && e.Nombre == "Pendiente")
                    .Select(e => e.Id)
                    .FirstOrDefault();

                _context.Comprobantes.Add(comprobante);
                await _context.SaveChangesAsync();

                return Json(new ApiResponse { data = null, message = "Comprobante registrado exitosamente.", status = true });
            }
            catch (Exception ex)
            {
                return Json(new ApiResponse { data = null, message = $"Error al registrar en BD: {ex.Message}", status = false });
            }
        }
        // VER COMPROBANTES
        public IActionResult VerComprobantes(int id)
        {
            var gasto = _context.Gastos.FirstOrDefault(g => g.Id == id);
            if (gasto == null)
            {
                return NotFound();
            }
            return View(gasto);
        }
        public JsonResult GetComprobantesData(int id)
        {
            try
            {
                var comprobantes = (from c in _context.Comprobantes
                                    join tc in _context.TipoComprobantes on c.TipoComprobanteId equals tc.Id
                                    join mo in _context.Monedas on c.MonedaId equals mo.Id
                                    join co in _context.Conceptos on c.ConceptoId equals co.Id
                                    join est in _context.Estados on c.EstadoId equals est.Id
                                    select new
                                    {
                                        c.Id,
                                        c.Fecha,
                                        c.RucEmpresa,
                                        c.Serie,
                                        c.Numero,
                                        TipoComprobante = tc.Descripcion,
                                        Concepto = co.Nombre,
                                        Moneda = mo.Nombre + " (" + mo.Simbolo + ")",
                                        c.Monto,
                                        c.Descripcion,
                                        est.Nombre,
                                    }).ToList();


                return Json(new { data = comprobantes, message = "Comprobantes retornados exitosamente.", status = true });
            }
            catch (Exception ex)
            {
                return Json(new ApiResponse { data = null, message = ex.Message, status = false });
            }
        }

    }
}
