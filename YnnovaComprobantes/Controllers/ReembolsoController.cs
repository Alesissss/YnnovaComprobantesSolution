using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using YnnovaComprobantes.Data;
using YnnovaComprobantes.Models;
using YnnovaComprobantes.ViewModels;

namespace YnnovaComprobantes.Controllers
{
    [Authorize]
    public class ReembolsoController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ReembolsoController(IWebHostEnvironment webHostEnvironment, ApplicationDbContext context)
        {
            _webHostEnvironment = webHostEnvironment;
            _context = context;
        }

        #region VISTAS
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult RegistrarReembolso()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Editar(int id)
        {
            var reembolso = _context.Reembolsos.FirstOrDefault(r => r.Id == id);
            if (reembolso == null) return NotFound();

            var estado = _context.Estados.FirstOrDefault(e => e.Id == reembolso.EstadoId);

            // Obtener usuario logueado
            int usuarioId = 0;
            var claimId = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claimId != null && int.TryParse(claimId.Value, out int uid)) usuarioId = uid;

            // Usamos el mismo ViewModel o uno similar ajustado a Reembolso
            var model = new EditarReembolsoViewModel
            {
                Reembolso = reembolso,
                Estado = estado ?? new Estado { Nombre = "Pendiente" },
                UsuarioLogueadoId = usuarioId
            };

            return View(model);
        }
        [HttpGet]
        public IActionResult VerReembolso(int id)
        {
            var reembolso = _context.Reembolsos.FirstOrDefault(r => r.Id == id);
            if (reembolso == null) return NotFound();

            var estado = _context.Estados.FirstOrDefault(e => e.Id == reembolso.EstadoId);

            // Obtener usuario logueado
            int usuarioId = 0;
            var claimId = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claimId != null && int.TryParse(claimId.Value, out int uid)) usuarioId = uid;

            // Usamos el mismo ViewModel o uno similar ajustado a Reembolso
            var model = new EditarReembolsoViewModel
            {
                Reembolso = reembolso,
                Estado = estado ?? new Estado { Nombre = "Pendiente" },
                UsuarioLogueadoId = usuarioId
            };

            return View(model);
        }
        [HttpGet]
        public IActionResult SubirComprobante(int id)
        {
            // 1. Obtener el reembolso
            var reembolso = _context.Reembolsos.FirstOrDefault(r => r.Id == id);
            if (reembolso == null) return NotFound();

            // 2. Obtener el estado actual
            var estado = _context.Estados.FirstOrDefault(e => e.Id == reembolso.EstadoId);

            // 3. Obtener el usuario logueado (desde Claims)
            int usuarioLogueadoId = 0;
            var claimId = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claimId != null && int.TryParse(claimId.Value, out int uid))
                usuarioLogueadoId = uid;

            // 4. Preparar el ViewModel con las listas para las etiquetas de solo lectura 
            // y para llenar los selects del nuevo comprobante
            var model = new EditarReembolsoViewModel
            {
                Reembolso = reembolso,
                Estado = estado ?? new Estado { Nombre = "Pendiente" },
                UsuarioLogueadoId = usuarioLogueadoId,

                // Estas listas se usan para mostrar los nombres en la tarjeta de información (Readonly)
                Empresas = _context.Empresas.ToList(),
                Usuarios = _context.Usuarios.ToList(),
                Bancos = _context.Bancos.ToList(),
                Monedas = _context.Monedas.ToList()
            };

            return View(model);
        }
        #endregion
        #region APIs
        [HttpGet]
        public JsonResult GetReembolsosData()
        {
            try
            {
                var reembolsos = (from r in _context.Reembolsos
                                  join e in _context.Empresas on r.EmpresaId equals e.Id
                                  join u in _context.Usuarios on r.UsuarioId equals u.Id
                                  join b in _context.Bancos on r.BancoId equals b.Id into gj
                                  from subB in gj.DefaultIfEmpty() // Left join por si banco_id es null
                                  join mo in _context.Monedas on r.MonedaId equals mo.Id
                                  join est in _context.Estados on r.EstadoId equals est.Id
                                  select new
                                  {
                                      r.Id,
                                      r.FechaSolicitud,
                                      Empresa = e.Nombre,
                                      Usuario = u.Nombre,
                                      Banco = subB != null ? subB.Descripcion : "No asignado",
                                      r.NumeroCuenta,
                                      Moneda = mo.Nombre,
                                      r.MontoTotal,
                                      Descripcion = r.Descripcion ?? "Sin descripción",
                                      Estado = est.Nombre
                                  }).ToList();

                return Json(new { data = reembolsos, status = true, message = "Reembolsos recuperados." });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = ex.Message });
            }
        }

        [HttpGet]
        public JsonResult GetMisReembolsosData()
        {
            try
            {
                string? userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userIdString == null) return Json(new { status = false, message = "No logueado" });

                int userId = int.Parse(userIdString.Trim());

                var reembolsos = (from r in _context.Reembolsos
                                  join e in _context.Empresas on r.EmpresaId equals e.Id
                                  join u in _context.Usuarios on r.UsuarioId equals u.Id
                                  join b in _context.Bancos on r.BancoId equals b.Id into gj
                                  from subB in gj.DefaultIfEmpty()
                                  join mo in _context.Monedas on r.MonedaId equals mo.Id
                                  join est in _context.Estados on r.EstadoId equals est.Id
                                  where r.UsuarioId == userId
                                  select new
                                  {
                                      r.Id,
                                      r.FechaSolicitud,
                                      Empresa = e.Nombre,
                                      Usuario = u.Nombre,
                                      Banco = subB != null ? subB.Descripcion : "No asignado",
                                      r.NumeroCuenta,
                                      Moneda = mo.Nombre,
                                      r.MontoTotal,
                                      Descripcion = r.Descripcion ?? "Sin descripción",
                                      Estado = est.Nombre
                                  }).ToList();

                return Json(new { data = reembolsos, status = true });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = ex.Message });
            }
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
        // Get monedas
        [HttpGet]
        public JsonResult GetMonedasData()
        {
            try
            {
                var monedas = _context.Monedas.ToList();
                return Json(new { data = monedas, message = "Monedas retornadas exitosamente.", status = true });
            }
            catch (Exception ex)
            {
                return Json(new ApiResponse { data = null, message = ex.Message, status = false });
            }
        }

        [HttpPost]
        public JsonResult RegistrarReembolso(Reembolso reembolso)
        {
            try
            {
                // Buscamos el estado inicial para el flujo de REEMBOLSO
                reembolso.EstadoId = _context.Estados
                    .Where(e => e.Tabla == "REEMBOLSO" && e.Nombre == "Pendiente")
                    .Select(e => e.Id).FirstOrDefault();

                reembolso.FechaRegistro = DateTime.Now;
                reembolso.UsuarioRegistro = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

                _context.Reembolsos.Add(reembolso);
                _context.SaveChanges();

                return Json(new { status = true, message = "Reembolso solicitado correctamente." });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = ex.Message });
            }
        }

        [HttpGet]
        public JsonResult GetObservaciones(int reembolsoId)
        {
            try
            {
                var lista = (from o in _context.Observaciones
                             join u in _context.Usuarios on o.UsuarioId equals u.Id
                             where o.ReembolsoId == reembolsoId // Nueva columna en BD
                             orderby o.FechaCreacion ascending
                             select new
                             {
                                 id = o.Id,
                                 usuarioId = o.UsuarioId,
                                 nombreUsuario = u.Nombre,
                                 prioridad = o.Prioridad,
                                 mensaje = o.Mensaje,
                                 fechaCreacion = o.FechaCreacion
                             }).ToList();

                return Json(new { status = true, data = lista });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> RegistrarObservacion(int ReembolsoId, string Mensaje, string Prioridad)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    string? userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    if (userIdString == null) return Json(new { status = false, message = "No logueado" });
                    int userId = int.Parse(userIdString.Trim());

                    // 1. Registrar Observación
                    var nuevaObs = new Observacion
                    {
                        ReembolsoId = ReembolsoId,
                        UsuarioId = userId,
                        Mensaje = Mensaje,
                        Prioridad = Prioridad,
                        FechaCreacion = DateTime.Now
                    };
                    _context.Observaciones.Add(nuevaObs);

                    // 2. Lógica masiva: Comprobantes del Reembolso Pendientes -> Observados
                    var estadoPendiente = await _context.Estados.FirstOrDefaultAsync(e => e.Nombre == "Pendiente" && e.Tabla == "COMPROBANTE");
                    var estadoObservado = await _context.Estados.FirstOrDefaultAsync(e => e.Nombre == "Observado" && e.Tabla == "COMPROBANTE");

                    if (estadoPendiente != null && estadoObservado != null)
                    {
                        var comprobantes = await _context.Comprobantes
                            .Where(c => c.ReembolsoId == ReembolsoId && c.EstadoId == estadoPendiente.Id)
                            .ToListAsync();

                        foreach (var c in comprobantes) { c.EstadoId = estadoObservado.Id; }
                    }

                    await _context.SaveChangesAsync();
                    transaction.Commit();

                    return Json(new { status = true, message = "Observación enviada." });
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return Json(new { status = false, message = ex.Message });
                }
            }
        }
        [HttpPost]
        public JsonResult ActualizarReembolsoDatos(Reembolso datos)
        {
            try
            {
                var reembolsoDb = _context.Reembolsos.Find(datos.Id);
                if (reembolsoDb == null) return Json(new { status = false, message = "Reembolso no encontrado" });

                // Actualización de campos manual según tu tabla
                reembolsoDb.EmpresaId = datos.EmpresaId;
                reembolsoDb.UsuarioId = datos.UsuarioId;
                reembolsoDb.FechaSolicitud = datos.FechaSolicitud;
                reembolsoDb.MonedaId = datos.MonedaId;
                reembolsoDb.BancoId = datos.BancoId;
                reembolsoDb.NumeroCuenta = datos.NumeroCuenta;
                reembolsoDb.Descripcion = datos.Descripcion;
                // Nota: MontoTotal no se actualiza aquí porque es la suma de comprobantes

                _context.SaveChanges();
                return Json(new { status = true, message = "Datos del reembolso actualizados correctamente." });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = ex.Message });
            }
        }

        [HttpGet]
        public JsonResult GetComprobantesPorReembolso(int reembolsoId)
        {
            try
            {
                var lista = (from c in _context.Comprobantes
                             join con in _context.Conceptos on c.ConceptoId equals con.Id
                             join est in _context.Estados on c.EstadoId equals est.Id
                             where c.ReembolsoId == reembolsoId
                             select new
                             {
                                 id = c.Id,
                                 fechaEmision = c.FechaEmision,
                                 proveedorNombre = c.ProveedorNombre,
                                 rucEmpresa = c.RucEmpresa,
                                 serie = c.Serie,
                                 numero = c.Numero,
                                 concepto = con.Nombre,
                                 montoTotal = c.MontoTotal,
                                 archivoUrl = c.ArchivoUrl,
                                 estadoNombre = est.Nombre
                             }).ToList();

                return Json(new { status = true, data = lista });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CambiarEstadoComprobante(int comprobanteId, string nuevoEstadoNombre)
        {
            try
            {
                var comprobante = await _context.Comprobantes.FindAsync(comprobanteId);
                if (comprobante == null) return Json(new { status = false, message = "Comprobante no encontrado" });

                var estadoDb = await _context.Estados.FirstOrDefaultAsync(e => e.Nombre == nuevoEstadoNombre && e.Tabla == "COMPROBANTE");
                if (estadoDb == null) return Json(new { status = false, message = "Estado no configurado" });

                comprobante.EstadoId = estadoDb.Id;
                comprobante.UsuarioAprobador = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
                await _context.SaveChangesAsync();

                // OPCIONAL: Lógica para actualizar el MontoTotal del Reembolso
                // Si el estado es "Aprobado", sumamos los montos de los comprobantes aprobados al Reembolso cabecera.
                //if (comprobante.ReembolsoId.HasValue)
                //{
                //    var montoAprobado = await _context.Comprobantes
                //        .Where(c => c.ReembolsoId == comprobante.ReembolsoId && c.EstadoId == estadoDb.Id)
                //        .SumAsync(c => c.MontoTotal);

                //    var reembolso = await _context.Reembolsos.FindAsync(comprobante.ReembolsoId);
                //    if (reembolso != null)
                //    {
                //        reembolso.MontoTotal = montoAprobado;
                //        await _context.SaveChangesAsync();
                //    }
                //}

                return Json(new { status = true, message = $"Comprobante actualizado a {nuevoEstadoNombre}" });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = ex.Message });
            }
        }
        [HttpPost]
        public async Task<JsonResult> RegistrarComprobante(Comprobante comprobante, IFormFile ArchivoComprobante)
        {
            try
            {
                if (comprobante.ReembolsoId == null || comprobante.ReembolsoId == 0)
                    return Json(new { status = false, message = "Error de vinculación." });

                if (ArchivoComprobante != null && ArchivoComprobante.Length > 0)
                {
                    string folderPath = Path.Combine(_webHostEnvironment.WebRootPath, "comprobantes_reembolso");
                    if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(ArchivoComprobante.FileName);
                    string filePath = Path.Combine(folderPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ArchivoComprobante.CopyToAsync(stream);
                    }
                    comprobante.ArchivoUrl = "/comprobantes_reembolso/" + fileName;
                }

                var estadoPendiente = _context.Estados.FirstOrDefault(e => e.Tabla == "COMPROBANTE" && e.Nombre == "Pendiente");
                comprobante.EstadoId = estadoPendiente?.Id ?? 1;
                comprobante.UsuarioRegistro = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
                comprobante.FechaRegistro = DateTime.Now;

                _context.Comprobantes.Add(comprobante);
                await _context.SaveChangesAsync();

                return Json(new { status = true, message = "Comprobante registrado." });
            }
            catch (Exception ex) { return Json(new { status = false, message = ex.Message }); }
        }

        [HttpPost]
        public JsonResult EliminarComprobante(int id)
        {
            try
            {
                var comp = _context.Comprobantes.Find(id);
                if (comp != null)
                {
                    // Solo permitir eliminar si no está aprobado
                    var estado = _context.Estados.Find(comp.EstadoId);
                    if (estado?.Nombre == "Aprobado" || estado?.Nombre == "Rechazado") return Json(new { status = false, message = "No se puede eliminar un comprobante ya aprobado." });

                    _context.Comprobantes.Remove(comp);
                    _context.SaveChanges();
                    return Json(new { status = true });
                }
                return Json(new { status = false });
            }
            catch (Exception ex) { return Json(new { status = false, message = ex.Message }); }
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
        // Get conceptos
        [HttpGet]
        public JsonResult GetConceptosData()
        {
            try
            {
                var conceptos = _context.Conceptos.Where(c => c.Estado == true).ToList();
                return Json(new { data = conceptos, message = "Conceptos retornados exitosamente.", status = true });
            }
            catch (Exception ex)
            {
                return Json(new ApiResponse { data = null, message = ex.Message, status = false });
            }
        }
        #endregion
    }
}