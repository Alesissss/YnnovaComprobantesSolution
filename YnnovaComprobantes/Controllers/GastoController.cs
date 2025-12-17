using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        // VER COMPROBANTES POR GASTO (VISTA DEL ADMIN)
        public IActionResult VerGasto(int id)
        {
            var gasto = _context.Gastos.FirstOrDefault(g => g.Id == id);
            if (gasto == null)
            {
                return NotFound();
            }
            return View(gasto);
        }
        public IActionResult RevisarComprobante(int id)
        {
            var comprobante = (from c in _context.Comprobantes
                               join g in _context.Gastos on c.GastoId equals g.Id
                               join mo in _context.Monedas on c.MonedaId equals mo.Id
                               join tc in _context.TipoComprobantes on c.TipoComprobanteId equals tc.Id
                               join co in _context.Conceptos on c.ConceptoId equals co.Id
                               join est in _context.Estados on c.EstadoId equals est.Id
                               where c.Id == id
                               select new ComprobanteViewModel
                               {
                                   GId = g.Id,
                                   GUsuarioId = g.UsuarioId,
                                   GTipoGasto = _context.TipoGastos.Where(tg => tg.Id == g.TipoGastoId).Select(tg => tg.Nombre).FirstOrDefault(),
                                   GFecha = g.Fecha,
                                   GMoneda = _context.Monedas.Where(m => m.Id == g.MonedaId).Select(m => m.Simbolo).FirstOrDefault(),
                                   GImporte = g.Importe,
                                   Id = c.Id,
                                   RucEmpresa = c.RucEmpresa,
                                   Serie = c.Serie,
                                   Numero = c.Numero,
                                   TipoComprobanteId = c.TipoComprobanteId,
                                   ConceptoId = c.ConceptoId,
                                   MonedaId = c.MonedaId,
                                   Monto = c.Monto,
                                   Fecha = c.Fecha,
                                   Estado = est.Nombre,
                                   Descripcion = c.Descripcion,
                                   Archivo = c.Archivo,
                                   MontoAcumulado = _context.Comprobantes.Where(com => com.GastoId == c.GastoId && com.EstadoId == _context.Estados.Where(e => e.Tabla == "COMPROBANTE" && e.Nombre == "Aprobado").Select(e => e.Id).FirstOrDefault()).Select(com => com.Monto).Sum()
                               }).FirstOrDefault();

            if (comprobante == null)
            {
                return NotFound();
            }
            return View(comprobante);
        }
        public IActionResult VerRevisionComprobante(int id)
        {
            var comprobante = (from c in _context.Comprobantes
                               join g in _context.Gastos on c.GastoId equals g.Id
                               join mo in _context.Monedas on c.MonedaId equals mo.Id
                               join tc in _context.TipoComprobantes on c.TipoComprobanteId equals tc.Id
                               join co in _context.Conceptos on c.ConceptoId equals co.Id
                               join est in _context.Estados on c.EstadoId equals est.Id
                               where c.Id == id
                               select new ComprobanteViewModel
                               {
                                   GId = g.Id,
                                   GUsuarioId = g.UsuarioId,
                                   GTipoGasto = _context.TipoGastos.Where(tg => tg.Id == g.TipoGastoId).Select(tg => tg.Nombre).FirstOrDefault(),
                                   GFecha = g.Fecha,
                                   GMoneda = _context.Monedas.Where(m => m.Id == g.MonedaId).Select(m => m.Simbolo).FirstOrDefault(),
                                   GImporte = g.Importe,
                                   Id = c.Id,
                                   RucEmpresa = c.RucEmpresa,
                                   Serie = c.Serie,
                                   Numero = c.Numero,
                                   TipoComprobanteId = c.TipoComprobanteId,
                                   ConceptoId = c.ConceptoId,
                                   MonedaId = c.MonedaId,
                                   Monto = c.Monto,
                                   Fecha = c.Fecha,
                                   Estado = est.Nombre,
                                   Descripcion = c.Descripcion,
                                   Archivo = c.Archivo,
                                   MontoAcumulado = _context.Comprobantes.Where(com => com.GastoId == c.GastoId && com.EstadoId == _context.Estados.Where(e => e.Tabla == "COMPROBANTE" && e.Nombre == "Aprobado").Select(e => e.Id).FirstOrDefault()).Select(com => com.Monto).Sum()
                               }).FirstOrDefault();

            if (comprobante == null)
            {
                return NotFound();
            }
            return View(comprobante);
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
                                    where c.GastoId == id
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
                                        Descripcion = c.Descripcion ?? "Sin descripción",
                                        Estado = est.Nombre,
                                    }).ToList();


                return Json(new { data = comprobantes, message = "Comprobantes retornados exitosamente.", status = true });
            }
            catch (Exception ex)
            {
                return Json(new ApiResponse { data = null, message = ex.Message, status = false });
            }
        }
        // EDITAR COMPROBANTE
        public IActionResult EditarComprobante(int id)
        {
            var comprobante = (from c in _context.Comprobantes
                               join g in _context.Gastos on c.GastoId equals g.Id
                               join mo in _context.Monedas on c.MonedaId equals mo.Id
                               join tc in _context.TipoComprobantes on c.TipoComprobanteId equals tc.Id
                               join co in _context.Conceptos on c.ConceptoId equals co.Id
                               join est in _context.Estados on c.EstadoId equals est.Id
                               where c.Id == id
                               select new ComprobanteViewModel
                               {
                                   GId = g.Id,
                                   GUsuarioId = g.UsuarioId,
                                   GTipoGasto = _context.TipoGastos.Where(tg => tg.Id == g.TipoGastoId).Select(tg => tg.Nombre).FirstOrDefault(),
                                   GFecha = g.Fecha,
                                   GMoneda = _context.Monedas.Where(m => m.Id == g.MonedaId).Select(m => m.Simbolo).FirstOrDefault(),
                                   GImporte = g.Importe,
                                   Id = c.Id,
                                   RucEmpresa = c.RucEmpresa,
                                   Serie = c.Serie,
                                   Numero = c.Numero,
                                   TipoComprobanteId = c.TipoComprobanteId,
                                   ConceptoId = c.ConceptoId,
                                   MonedaId = c.MonedaId,
                                   Monto = c.Monto,
                                   Fecha = c.Fecha,
                                   Estado = est.Nombre,
                                   Descripcion = c.Descripcion,
                                   Archivo = c.Archivo,
                                   MontoAcumulado = _context.Comprobantes.Where(com => com.GastoId == c.GastoId && com.EstadoId == _context.Estados.Where(e => e.Tabla == "COMPROBANTE" && e.Nombre == "Aprobado").Select(e => e.Id).FirstOrDefault()).Select(com => com.Monto).Sum()
                               }).FirstOrDefault();

            if (comprobante == null)
            {
                return NotFound();
            }
            return View(comprobante);
        }
        [HttpPost]
        public async Task<JsonResult> EditarComprobante(Comprobante comprobante, IFormFile ArchivoComprobante, bool archivoModificado)
        {
            var comprobanteDb = await _context.Comprobantes.FindAsync(comprobante.Id);
            if (comprobanteDb == null) return Json(new ApiResponse { status = false, message = "No existe el comprobante a editar." });
            if (_context.Estados.Any(e => e.Id == comprobanteDb.EstadoId && !new[] { "Pendiente", "Observado" }.Contains(e.Nombre))) return Json(new ApiResponse { status = false, message = "El comprobante no se puede editar porque no tiene estado Observado o Pendiente." });

            string? rutaNuevoArchivoParaLimpiar = null;
            string? rutaViejoArchivoParaEliminar = null;

            try
            {
                comprobanteDb.RucEmpresa = comprobante.RucEmpresa;
                comprobanteDb.Serie = comprobante.Serie;
                comprobanteDb.Numero = comprobante.Numero;
                comprobanteDb.TipoComprobanteId = comprobante.TipoComprobanteId;
                comprobanteDb.ConceptoId = comprobante.ConceptoId;
                comprobanteDb.MonedaId = comprobante.MonedaId;
                comprobanteDb.Monto = comprobante.Monto;
                comprobanteDb.Fecha = comprobante.Fecha;
                comprobanteDb.Descripcion = comprobante.Descripcion;

                if (archivoModificado && ArchivoComprobante != null)
                {
                    var carpetaUploads = Path.Combine(_hostingEnvironment.WebRootPath, "uploads", "comprobantes");
                    if (!Directory.Exists(carpetaUploads)) Directory.CreateDirectory(carpetaUploads);

                    var nombreUnico = $"{Guid.NewGuid()}{Path.GetExtension(ArchivoComprobante.FileName)}";
                    var rutaCompletaNuevo = Path.Combine(carpetaUploads, nombreUnico);

                    using (var stream = new FileStream(rutaCompletaNuevo, FileMode.Create))
                    {
                        await ArchivoComprobante.CopyToAsync(stream);
                    }

                    rutaNuevoArchivoParaLimpiar = rutaCompletaNuevo;

                    if (!string.IsNullOrEmpty(comprobanteDb.Archivo))
                    {
                        rutaViejoArchivoParaEliminar = Path.Combine(_hostingEnvironment.WebRootPath, comprobanteDb.Archivo.TrimStart('/'));
                    }

                    comprobanteDb.Archivo = $"/uploads/comprobantes/{nombreUnico}";
                }

                await _context.SaveChangesAsync();

                if (archivoModificado && !string.IsNullOrEmpty(rutaViejoArchivoParaEliminar))
                {
                    EliminarArchivoFisico(rutaViejoArchivoParaEliminar);
                }

                return Json(new ApiResponse { status = true, message = "Comprobante actualizado correctamente." });
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(rutaNuevoArchivoParaLimpiar))
                {
                    EliminarArchivoFisico(rutaNuevoArchivoParaLimpiar);
                }

                return Json(new ApiResponse { status = false, message = $"Error: {ex.Message}" });
            }
        }
        // Método auxiliar para evitar que un error de borrado rompa la respuesta al usuario
        private void EliminarArchivoFisico(string ruta)
        {
            try
            {
                if (System.IO.File.Exists(ruta))
                {
                    System.IO.File.Delete(ruta);
                }
            }
            catch (Exception)
            {
                // Loguear el error, pero no interrumpir el flujo del usuario
                // El archivo se borrará manualmente o mediante una tarea programada luego
            }
        }
        // LISTAR OBSERVACIONES
        [HttpGet]
        public JsonResult GetObservacionesData(int id)
        {
            try
            {
                var observaciones = _context.Observaciones.Where(o => o.ComprobanteId == id).OrderByDescending(o => o.FechaCreacion);
                return Json(new ApiResponse { data = observaciones, message = "Observaciones recuperadas exitosamente.", status = true });
            }
            catch (Exception ex)
            {
                return Json(new ApiResponse { data = null, message = ex.Message, status = false });
            }
        }
        // REGISTRAR OBSERVACION
        [HttpPost]
        public JsonResult RegistrarObservacion(Observacion observacion, bool actualizarEstado)
        {
            try
            {
                observacion.FechaCreacion = DateTime.Now;

                if (actualizarEstado)
                {
                    var comprobante = _context.Comprobantes.Where(c => c.Id == observacion.ComprobanteId).FirstOrDefault();
                    if (comprobante == null)
                    {
                        return Json(new ApiResponse { data = null, message = "El comprobante no existe.", status = false });
                    }
                    else
                    {
                        comprobante.EstadoId = _context.Estados.Where(e => e.Tabla == "COMPROBANTE" && e.Nombre == "Observado").Select(e => e.Id).FirstOrDefault();
                        _context.Update(comprobante);
                    }
                }

                _context.Observaciones.Add(observacion);
                _context.SaveChanges();

                return Json(new ApiResponse { data = null, message = "Observación registrada exitosamente.", status = true });
            }
            catch (Exception ex)
            {
                return Json(new ApiResponse { data = null, message = ex.Message, status = false });
            }
        }
        // VER OBSERVACIONES
        public IActionResult VerObservaciones(int id)
        {
            var comprobante = (from c in _context.Comprobantes
                               join g in _context.Gastos on c.GastoId equals g.Id
                               join mo in _context.Monedas on c.MonedaId equals mo.Id
                               join tc in _context.TipoComprobantes on c.TipoComprobanteId equals tc.Id
                               join co in _context.Conceptos on c.ConceptoId equals co.Id
                               join est in _context.Estados on c.EstadoId equals est.Id
                               where c.Id == id
                               select new ComprobanteViewModel
                               {
                                   GId = g.Id,
                                   GUsuarioId = g.UsuarioId,
                                   GTipoGasto = _context.TipoGastos.Where(tg => tg.Id == g.TipoGastoId).Select(tg => tg.Nombre).FirstOrDefault(),
                                   GFecha = g.Fecha,
                                   GMoneda = _context.Monedas.Where(m => m.Id == g.MonedaId).Select(m => m.Simbolo).FirstOrDefault(),
                                   GImporte = g.Importe,
                                   Id = c.Id,
                                   RucEmpresa = c.RucEmpresa,
                                   Serie = c.Serie,
                                   Numero = c.Numero,
                                   TipoComprobanteId = c.TipoComprobanteId,
                                   ConceptoId = c.ConceptoId,
                                   MonedaId = c.MonedaId,
                                   Monto = c.Monto,
                                   Fecha = c.Fecha,
                                   Estado = est.Nombre,
                                   Descripcion = c.Descripcion,
                                   Archivo = c.Archivo,
                                   MontoAcumulado = _context.Comprobantes.Where(com => com.GastoId == c.GastoId && com.EstadoId == _context.Estados.Where(e => e.Tabla == "COMPROBANTE" && e.Nombre == "Aprobado").Select(e => e.Id).FirstOrDefault()).Select(com => com.Monto).Sum()
                               }).FirstOrDefault();

            if (comprobante == null)
            {
                return NotFound();
            }
            return View(comprobante);
        }
        // APROBACIONES DE COMPROBANTES
        // Aprobar comprobante
        [HttpPost]
        public JsonResult AprobarComprobante(int id)
        {
            try
            {
                var comprobante = _context.Comprobantes.Where(c => c.Id == id).FirstOrDefault();
                if (comprobante == null)
                {
                    return Json(new ApiResponse { data = null, message = "El comprobante no existe", status = false });
                }

                comprobante.EstadoId = _context.Estados.Where(e => e.Tabla == "COMPROBANTE" && e.Nombre == "Aprobado").Select(e => e.Id).FirstOrDefault();
                _context.Comprobantes.Update(comprobante);
                _context.SaveChanges();

                return Json(new ApiResponse { data = null, message = "Comprobante aprobado exitosamente.", status = true });
            }
            catch (Exception ex)
            {
                return Json(new ApiResponse { data = null, message = ex.Message, status = false });
            }
        }
        // Rechazar comprobante
        [HttpPost]
        public JsonResult RechazarComprobante(int id)
        {
            try
            {
                var comprobante = _context.Comprobantes.Where(c => c.Id == id).FirstOrDefault();
                if (comprobante == null)
                {
                    return Json(new ApiResponse { data = null, message = "El comprobante no existe", status = false });
                }

                comprobante.EstadoId = _context.Estados.Where(e => e.Tabla == "COMPROBANTE" && e.Nombre == "Rechazado").Select(e => e.Id).FirstOrDefault();
                _context.Comprobantes.Update(comprobante);
                _context.SaveChanges();

                return Json(new ApiResponse { data = null, message = "Comprobante aprobado exitosamente.", status = true });
            }
            catch (Exception ex)
            {
                return Json(new ApiResponse { data = null, message = ex.Message, status = false });
            }
        }
        // APROBACIONES DE GASTOS
        // Aprobar gasto
        [HttpPost]
        public JsonResult AprobarGasto(int id)
        {
            try
            {
                var gasto = _context.Gastos.Where(g => g.Id == id).FirstOrDefault();
                if (gasto == null)
                {
                    return Json(new ApiResponse { data = null, message = "El gasto no existe", status = false });
                }

                gasto.EstadoId = _context.Estados.Where(e => e.Tabla == "GASTO" && e.Nombre == "Aprobado").Select(e => e.Id).FirstOrDefault();
                _context.Gastos.Update(gasto);
                _context.SaveChanges();

                return Json(new ApiResponse { data = null, message = "Gasto aprobado exitosamente.", status = true });
            }
            catch (Exception ex)
            {
                return Json(new ApiResponse { data = null, message = ex.Message, status = false });
            }
        }
        [HttpPost]
        public async Task<JsonResult> RechazarGasto(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var gasto = await _context.Gastos.FindAsync(id);
                if (gasto == null)
                    return Json(new ApiResponse { status = false, message = "El gasto no existe." });

                var estadoGastoRechazado = await _context.Estados
                    .Where(e => e.Tabla == "GASTO" && e.Nombre == "Rechazado")
                    .Select(e => e.Id).FirstOrDefaultAsync();

                var estadoComprobanteRechazado = await _context.Estados
                    .Where(e => e.Tabla == "COMPROBANTE" && e.Nombre == "Rechazado")
                    .Select(e => e.Id).FirstOrDefaultAsync();

                if (estadoGastoRechazado == 0 || estadoComprobanteRechazado == 0)
                    return Json(new ApiResponse { status = false, message = "Error: Configuración de estados no encontrada." });

                gasto.EstadoId = estadoGastoRechazado;
                _context.Gastos.Update(gasto);

                var comprobantes = await _context.Comprobantes.Where(c => c.GastoId == id).ToListAsync();
                foreach (var c in comprobantes)
                {
                    c.EstadoId = estadoComprobanteRechazado;
                }

                _context.Comprobantes.UpdateRange(comprobantes);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Json(new ApiResponse { status = true, message = "Gasto y comprobantes rechazados correctamente." });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Json(new ApiResponse { status = false, message = $"Error técnico: {ex.Message}" });
            }
        }
    }
}
