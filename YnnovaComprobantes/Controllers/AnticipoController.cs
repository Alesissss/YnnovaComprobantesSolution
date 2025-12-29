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
    public class AnticipoController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public AnticipoController(IWebHostEnvironment webHostEnvironment, ApplicationDbContext context)
        {
            _webHostEnvironment = webHostEnvironment;
            _context = context;
        }
        #region VISTAS
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult RegistrarAnticipo()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Editar(int id)
        {
            var anticipo = _context.Anticipos.FirstOrDefault(a => a.Id == id);
            if (anticipo == null) return NotFound();

            var estado = _context.Estados.FirstOrDefault(e => e.Id == anticipo.EstadoId);

            // Obtener usuario logueado (Simulado o real)
            int usuarioId = 1;
            var claimId = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claimId != null && int.TryParse(claimId.Value, out int uid)) usuarioId = uid;

            var model = new EditarAnticipoViewModel
            {
                Anticipo = anticipo,
                Estado = estado ?? new Estado { Nombre = "Desconocido" },

                // --- NUEVAS LISTAS PARA EL ANTICIPO ---
                Empresas = _context.Empresas.Where(x => x.Estado == true).ToList(),
                Usuarios = _context.Usuarios.Where(x => x.Estado == true).ToList(), // Podrías filtrar por empresa si lo deseas
                Bancos = _context.Bancos.Where(x => x.Estado == true).ToList(),
                TiposRendicion = _context.TipoRendiciones.Where(x => x.Estado == true).ToList(),

                // Listas existentes
                Monedas = _context.Monedas.ToList(),
                TiposComprobante = _context.TipoComprobantes.Where(x => x.Estado == true).ToList(),
                Conceptos = _context.Conceptos.Where(x => x.Estado == true).ToList(),

                UsuarioLogueadoId = usuarioId
            };

            return View(model);
        }
        [HttpGet]
        public IActionResult VerAnticipo(int id)
        {
            var anticipo = _context.Anticipos.FirstOrDefault(a => a.Id == id);
            if (anticipo == null) return NotFound();

            var estado = _context.Estados.FirstOrDefault(e => e.Id == anticipo.EstadoId);

            // Obtener usuario logueado (Simulado o real)
            int usuarioId = 1;
            var claimId = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claimId != null && int.TryParse(claimId.Value, out int uid)) usuarioId = uid;

            var model = new EditarAnticipoViewModel
            {
                Anticipo = anticipo,
                Estado = estado ?? new Estado { Nombre = "Desconocido" },

                // --- NUEVAS LISTAS PARA EL ANTICIPO ---
                Empresas = _context.Empresas.Where(x => x.Estado == true).ToList(),
                Usuarios = _context.Usuarios.Where(x => x.Estado == true).ToList(), // Podrías filtrar por empresa si lo deseas
                Bancos = _context.Bancos.Where(x => x.Estado == true).ToList(),
                TiposRendicion = _context.TipoRendiciones.Where(x => x.Estado == true).ToList(),

                // Listas existentes
                Monedas = _context.Monedas.ToList(),
                TiposComprobante = _context.TipoComprobantes.Where(x => x.Estado == true).ToList(),
                Conceptos = _context.Conceptos.Where(x => x.Estado == true).ToList(),

                UsuarioLogueadoId = usuarioId
            };

            return View(model);
        }
        [HttpGet]
        public IActionResult SubirComprobante(int id)
        {
            var anticipo = _context.Anticipos.FirstOrDefault(a => a.Id == id);
            if (anticipo == null) return NotFound();

            var estado = _context.Estados.FirstOrDefault(e => e.Id == anticipo.EstadoId);

            // Obtener usuario logueado (Simulado o real)
            int usuarioId = 1;
            var claimId = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claimId != null && int.TryParse(claimId.Value, out int uid)) usuarioId = uid;

            var model = new EditarAnticipoViewModel
            {
                Anticipo = anticipo,
                Estado = estado ?? new Estado { Nombre = "Desconocido" },

                // --- NUEVAS LISTAS PARA EL ANTICIPO ---
                Empresas = _context.Empresas.Where(x => x.Estado == true).ToList(),
                Usuarios = _context.Usuarios.Where(x => x.Estado == true).ToList(), // Podrías filtrar por empresa si lo deseas
                Bancos = _context.Bancos.Where(x => x.Estado == true).ToList(),
                TiposRendicion = _context.TipoRendiciones.Where(x => x.Estado == true).ToList(),

                // Listas existentes
                Monedas = _context.Monedas.ToList(),
                TiposComprobante = _context.TipoComprobantes.Where(x => x.Estado == true).ToList(),
                Conceptos = _context.Conceptos.Where(x => x.Estado == true).ToList(),

                UsuarioLogueadoId = usuarioId
            };

            return View(model);
        }
        #endregion
        #region APIs
        [HttpGet]
        public JsonResult GetAnticiposData()
        {
            try
            {
                var anticipos = (from a in _context.Anticipos
                                 join e in _context.Empresas on a.EmpresaId equals e.Id
                                 join u in _context.Usuarios on a.UsuarioId equals u.Id
                                 join b in _context.Bancos on a.BancoId equals b.Id
                                 join mo in _context.Monedas on a.MonedaId equals mo.Id
                                 join tr in _context.TipoRendiciones on a.TipoRendicionId equals tr.Id
                                 join est in _context.Estados on a.EstadoId equals est.Id
                                 select new
                                 {
                                     a.Id,
                                     a.FechaSolicitud,
                                     a.FechaLimiteRendicion,
                                     Empresa = e.Nombre,
                                     Usuario = u.Nombre,
                                     Banco = b.Descripcion,
                                     TipoRendicion = tr.Descripcion,
                                     Moneda = mo.Nombre,
                                     a.Monto,
                                     Descripcion = a.Descripcion ?? "Sin descripción",
                                     Estado = est.Nombre
                                 }).ToList();

                return Json(new { data = anticipos, status = true, message = "Anticipos recuperados exitosamente." });
            }
            catch (Exception ex)
            {
                return Json(new ApiResponse { data = null, status = false, message = ex.Message });
            }
        }
        [HttpGet]
        public JsonResult GetMisAnticiposData()
        {
            try
            {
                string? userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (userIdString == null)
                {
                    return Json(new ApiResponse { data = null, message = "El usuario no se encuentra logueado.", status = false });
                }

                int userId = int.Parse(userIdString.Trim());

                var anticipos = (from a in _context.Anticipos
                                 join e in _context.Empresas on a.EmpresaId equals e.Id
                                 join u in _context.Usuarios on a.UsuarioId equals u.Id
                                 join b in _context.Bancos on a.BancoId equals b.Id
                                 join mo in _context.Monedas on a.MonedaId equals mo.Id
                                 join tr in _context.TipoRendiciones on a.TipoRendicionId equals tr.Id
                                 join est in _context.Estados on a.EstadoId equals est.Id
                                 where a.UsuarioId == userId
                                 select new
                                 {
                                     a.Id,
                                     a.FechaSolicitud,
                                     a.FechaLimiteRendicion,
                                     Empresa = e.Nombre,
                                     Usuario = u.Nombre,
                                     Banco = b.Descripcion,
                                     TipoRendicion = tr.Descripcion,
                                     Moneda = mo.Nombre,
                                     a.Monto,
                                     Descripcion = a.Descripcion ?? "Sin descripción",
                                     Estado = est.Nombre
                                 }).ToList();

                return Json(new { data = anticipos, status = true, message = "Anticipos recuperados exitosamente." });
            }
            catch (Exception ex)
            {
                return Json(new ApiResponse { data = null, status = false, message = ex.Message });
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
                var monedas = _context.Monedas.ToList();
                return Json(new { data = monedas, message = "Monedas retornadas exitosamente.", status = true });
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
        [HttpPost]
        public JsonResult RegistrarAnticipo(Anticipo anticipo)
        {
            try
            {
                anticipo.EstadoId = _context.Estados.Where(e => e.Tabla == "ANTICIPO" && e.Nombre == "Pendiente").Select(e => e.Id).FirstOrDefault();
                anticipo.FechaRegistro = DateTime.Now;

                _context.Anticipos.Add(anticipo);
                _context.SaveChanges();

                return Json(new ApiResponse { data = null, message = "Anticipo registrado exitosamente.", status = true });
            }
            catch (Exception ex)
            {
                return Json(new ApiResponse { data = null, message = ex.Message, status = false });
            }
        }
        [HttpGet]
        public JsonResult GetObservaciones(int anticipoId)
        {
            try
            {
                var lista = (from o in _context.Observaciones
                             join u in _context.Usuarios on o.UsuarioId equals u.Id
                             where o.AnticipoId == anticipoId
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
        public async Task<IActionResult> RegistrarObservacion(int AnticipoId, string Mensaje, string Prioridad)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    string? userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

                    if (userIdString == null)
                    {
                        return Json(new ApiResponse { data = null, message = "El usuario no se encuentra logueado.", status = false });
                    }

                    int userId = int.Parse(userIdString.Trim());

                    // 1. Registrar la Observación (Tu lógica actual)
                    var nuevaObs = new Observacion
                    {
                        AnticipoId = AnticipoId,
                        UsuarioId = userId,
                        Mensaje = Mensaje,
                        Prioridad = Prioridad,
                        FechaCreacion = DateTime.Now
                    };
                    _context.Observaciones.Add(nuevaObs);

                    // 2. LÓGICA DE ESTADOS: "Pendiente" -> "Observado"
                    // Buscamos los IDs de los estados según tu tabla
                    var estadoPendiente = await _context.Estados.FirstOrDefaultAsync(e => e.Nombre == "Pendiente" && e.Tabla == "COMPROBANTE");
                    var estadoObservado = await _context.Estados.FirstOrDefaultAsync(e => e.Nombre == "Observado" && e.Tabla == "COMPROBANTE");

                    if (estadoPendiente != null && estadoObservado != null)
                    {
                        // Buscar comprobantes de este anticipo que estén "Pendientes"
                        var comprobantesPendientes = await _context.Comprobantes
                            .Where(c => c.AnticipoId == AnticipoId && c.EstadoId == estadoPendiente.Id)
                            .ToListAsync();

                        if (comprobantesPendientes.Any())
                        {
                            foreach (var comp in comprobantesPendientes)
                            {
                                comp.EstadoId = estadoObservado.Id;
                            }
                            // _context.UpdateRange(comprobantesPendientes); // Opcional según versión de EF
                        }
                    }

                    await _context.SaveChangesAsync();
                    transaction.Commit();

                    return Json(new { status = true, message = "Observación registrada y estados actualizados." });
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return Json(new { status = false, message = "Error: " + ex.Message });
                }
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
                if (estadoDb == null) return Json(new { status = false, message = "Estado no configurado en BD" });

                comprobante.EstadoId = estadoDb.Id;
                comprobante.UsuarioAprobador = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
                await _context.SaveChangesAsync();

                return Json(new { status = true, message = $"Comprobante marcado como {nuevoEstadoNombre}" });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = ex.Message });
            }
        }
        [HttpPost]
        public JsonResult ActualizarAnticipoDatos(Anticipo datos)
        {
            try
            {
                var anticipoDb = _context.Anticipos.Find(datos.Id);
                if (anticipoDb == null) return Json(new { status = false, message = "Anticipo no encontrado" });

                // Actualizamos TODOS los campos editables
                anticipoDb.EmpresaId = datos.EmpresaId;
                anticipoDb.UsuarioId = datos.UsuarioId;
                anticipoDb.BancoId = datos.BancoId;
                anticipoDb.MonedaId = datos.MonedaId;
                anticipoDb.TipoRendicionId = datos.TipoRendicionId;

                anticipoDb.Descripcion = datos.Descripcion;
                anticipoDb.Monto = datos.Monto;
                anticipoDb.FechaSolicitud = datos.FechaSolicitud;
                anticipoDb.FechaLimiteRendicion = datos.FechaLimiteRendicion;

                _context.SaveChanges();

                return Json(new { status = true, message = "Datos del anticipo actualizados correctamente." });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = "Error: " + ex.Message });
            }
        }
        [HttpPost]
        public async Task<JsonResult> RegistrarComprobante(Comprobante comprobante, IFormFile ArchivoComprobante)
        {
            try
            {
                // 1. Validar Anticipo
                // Nota: Asegúrate de que comprobante.AnticipoId venga lleno desde el JS
                if (comprobante.AnticipoId == null || comprobante.AnticipoId == 0)
                    return Json(new { status = false, message = "Error de vinculación con el anticipo." });

                // 2. Manejo del Archivo (Imagen/PDF)
                if (ArchivoComprobante != null && ArchivoComprobante.Length > 0)
                {
                    // Definir ruta: wwwroot/comprobantes/
                    string folderPath = Path.Combine(_webHostEnvironment.WebRootPath, "comprobantes");
                    if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

                    // Generar nombre único para evitar reemplazos
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(ArchivoComprobante.FileName);
                    string filePath = Path.Combine(folderPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ArchivoComprobante.CopyToAsync(stream);
                    }

                    // Guardar la URL relativa en la BD
                    comprobante.ArchivoUrl = "/comprobantes/" + fileName;
                }

                // 3. Completar datos faltantes
                // Asignar estado "Pendiente" (Búscalo en tu DB o usa el ID directo si lo conoces)
                var estadoPendiente = _context.Estados.FirstOrDefault(e => e.Tabla == "COMPROBANTE" && e.Nombre == "Pendiente");
                comprobante.EstadoId = estadoPendiente != null ? estadoPendiente.Id : 1;
                comprobante.UsuarioRegistro = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
                comprobante.FechaRegistro = DateTime.Now;
                // Asignar el usuario logueado (puedes tomarlo de Claims o del modelo si lo enviaste)
                // comprobante.UsuarioId = ... 

                _context.Comprobantes.Add(comprobante);
                await _context.SaveChangesAsync();

                return Json(new { status = true, message = "Comprobante registrado correctamente." });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = "Error al subir comprobante: " + ex.Message });
            }
        }

        [HttpGet]
        public JsonResult GetComprobantesPorAnticipo(int anticipoId)
        {
            try
            {
                // Hacemos Join para obtener nombres (Proveedor, Concepto) si es necesario
                // Ojo: Ajusta los nombres de las propiedades según tu modelo exacto
                var lista = (from c in _context.Comprobantes
                             join con in _context.Conceptos on c.ConceptoId equals con.Id
                             join est in _context.Estados on c.EstadoId equals est.Id
                             where c.AnticipoId == anticipoId
                             select new
                             {
                                 id = c.Id,
                                 fechaEmision = c.FechaEmision,
                                 // Si no tienes tabla Proveedor y usas el string directo:
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
        public JsonResult EliminarComprobante(int id)
        {
            try
            {
                var comp = _context.Comprobantes.Find(id);
                if (comp != null)
                {
                    // Opcional: Borrar el archivo físico del servidor para ahorrar espacio
                    if (!string.IsNullOrEmpty(comp.ArchivoUrl))
                    {
                        string path = Path.Combine(_webHostEnvironment.WebRootPath, comp.ArchivoUrl.TrimStart('/'));
                        if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
                    }

                    _context.Comprobantes.Remove(comp);
                    _context.SaveChanges();
                    return Json(new { status = true, message = "Eliminado correctamente" });
                }
                return Json(new { status = false, message = "No encontrado" });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = ex.Message });
            }
        }
        #endregion
        #region LIQUIDACION Y CIERRE
        [HttpGet]
        public async Task<JsonResult> ObtenerResumenLiquidacion(int anticipoId)
        {
            var anticipo = await _context.Anticipos.FindAsync(anticipoId);
            if (anticipo == null) return Json(new { status = false });

            // Sumamos solo comprobantes APROBADOS
            var estadoAprobado = await _context.Estados.FirstOrDefaultAsync(e => e.Nombre == "Aprobado" && e.Tabla == "COMPROBANTE");
            var totalGastado = await _context.Comprobantes
                .Where(c => c.AnticipoId == anticipoId && c.EstadoId == (estadoAprobado != null ? estadoAprobado.Id : 0))
                .SumAsync(c => c.MontoTotal);

            decimal? diferencia = anticipo.Monto - totalGastado;

            return Json(new
            {
                status = true,
                montoAnticipo = anticipo.Monto,
                totalGastado = totalGastado,
                diferencia = diferencia // Positivo: Devolución, Negativo: Reembolso
            });
        }
        [HttpGet]
        public JsonResult GetMovimientosLiquidacion(int anticipoId)
        {
            try
            {
                // 1. Obtener Devoluciones
                var devoluciones = _context.DevolucionesAnticipos
                    .Where(d => d.AnticipoId == anticipoId)
                    .Select(d => new
                    {
                        id = d.Id,
                        Estado = _context.Estados.FirstOrDefault(e => e.Id == d.EstadoId).Nombre,
                        fecha = d.FechaDevolucion.ToString("dd/MM/yyyy"),
                        monto = d.Monto
                    }).ToList();

                // 2. Obtener Reembolsos amarrados
                var reembolsos = _context.Reembolsos
                    .Where(r => r.AnticipoId == anticipoId)
                    .Select(r => new
                    {
                        id = r.Id,
                        fecha = r.FechaSolicitud.HasValue ? r.FechaSolicitud.Value.ToString("dd/MM/yyyy") : "-",
                        monto = r.MontoTotal,
                        estado = _context.Estados.FirstOrDefault(e => e.Id == r.EstadoId).Nombre
                    }).ToList();

                return Json(new { status = true, devoluciones, reembolsos });
            }
            catch (Exception ex) { return Json(new { status = false, message = ex.Message }); }
        }

        [HttpPost]
        public async Task<JsonResult> RegistrarCierreLiquidacion(int anticipoId, decimal monto, string tipo)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var anticipo = await _context.Anticipos.FindAsync(anticipoId);
                    int usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

                    // 1. Registrar el movimiento (Devolución o Reembolso)
                    var estadoAprobadoDevolucion = await _context.Estados.FirstOrDefaultAsync(e => e.Nombre == "Aprobado" && e.Tabla == "DEVOLUCION_ANTICIPO");
                    if (tipo == "DEVOLUCION")
                    {
                        var dev = new DevolucionAnticipo
                        {
                            AnticipoId = anticipoId,
                            Monto = monto,
                            FechaDevolucion = DateTime.Now,
                            // Asumiendo que existe estado 'Aprobado' para devoluciones, sino usa 1
                            EstadoId = estadoAprobadoDevolucion?.Id ?? 1,
                            UsuarioRegistro = usuarioId,
                            FechaRegistro = DateTime.Now
                        };
                        _context.DevolucionesAnticipos.Add(dev);
                    }
                    else if (tipo == "REEMBOLSO")
                    {
                        // Buscar estado Aprobado para reembolso
                        var estadoAprobadoReem = await _context.Estados.FirstOrDefaultAsync(e => e.Nombre == "Aprobado" && e.Tabla == "REEMBOLSO");

                        var reem = new Reembolso
                        {
                            AnticipoId = anticipoId,
                            EmpresaId = anticipo.EmpresaId,
                            UsuarioId = anticipo.UsuarioId,
                            MontoTotal = monto,
                            FechaSolicitud = DateTime.Now,
                            MonedaId = anticipo.MonedaId,
                            EstadoId = estadoAprobadoReem?.Id ?? 1,
                            Descripcion = $"Reembolso por cierre de Anticipo #{anticipo.Id}",
                            UsuarioRegistro = usuarioId,
                            FechaRegistro = DateTime.Now
                        };
                        _context.Reembolsos.Add(reem);
                    }

                    // 2. CAMBIAR ESTADO DEL ANTICIPO A "APROBADO" (CERRADO)
                    var estadoAprobadoAnticipo = await _context.Estados.FirstOrDefaultAsync(e => e.Nombre == "Aprobado" && e.Tabla == "ANTICIPO");
                    if (estadoAprobadoAnticipo != null)
                    {
                        anticipo.EstadoId = estadoAprobadoAnticipo.Id;
                        anticipo.UsuarioAprobador = usuarioId;
                    }

                    await _context.SaveChangesAsync();
                    transaction.Commit();

                    return Json(new { status = true, message = "Registro creado y Anticipo Cerrado (Aprobado) correctamente." });
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return Json(new { status = false, message = ex.Message });
                }
            }
        }
        #endregion
    }
}
