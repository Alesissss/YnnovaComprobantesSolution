using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Necesario para EF Core
using System.Security.Claims;
using YnnovaComprobantes.Data;       // Tu Namespace de Contexto
using YnnovaComprobantes.Models;     // Tu Namespace de Modelos

namespace YnnovaComprobantes.Controllers
{
    [Authorize]
    public class LiquidacionController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public LiquidacionController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        #region VISTAS (NAVEGACIÓN)

        // Redirección simple para pruebas: Tú decides a cuál url entrar manualmente
        public IActionResult Index()
        {
            // Como no hay roles, por defecto te mando al del Admin para que veas todo.
            // Puedes cambiarlo manual en la URL a /Liquidacion/IndexUsuario
            return RedirectToAction("IndexAdmin");
        }

        // --- VISTA ADMINISTRADOR ---
        // [Authorize(Roles = "Admin")] <--- COMENTADO
        public IActionResult IndexAdmin()
        {
            return View(); // Vista: Monitor Global (Todas las liquidaciones)
        }

        // [Authorize(Roles = "Admin")] <--- COMENTADO
        public IActionResult GestionarAdmin(int id)
        {
            ViewBag.LiquidacionId = id;
            return View(); // Vista: Auditoría, Aprobación y Cierre
        }

        // --- VISTA USUARIO ---
        public IActionResult IndexUsuario()
        {
            return View(); // Vista: Mis Carpetas Asignadas
        }

        public IActionResult GestionarUsuario(int id)
        {
            // En pruebas, asumimos que eres el usuario dueño.
            // Si quieres probar seguridad, descomenta la validación después.
            ViewBag.LiquidacionId = id;
            return View();
        }

        #endregion

        #region 0. UTILITARIOS (COMBOS / SELECTS)

        [HttpGet]
        public JsonResult GetEmpresas()
        {
            try
            {
                // Asumiendo que 'Estado == true' (BIT 1) significa Activo
                var data = _context.Empresas
                    .Where(x => x.Estado == true)
                    .Select(x => new { x.Id, x.Nombre })
                    .ToList();

                return Json(new { status = true, data = data });
            }
            catch (Exception ex) { return Json(new { status = false, message = ex.Message }); }
        }

        [HttpGet]
        public JsonResult GetUsuariosPorEmpresa(int empresaId)
        {
            try
            {
                // Hacemos Join con la tabla intermedia empresa_usuario para filtrar correctamente
                var data = (from eu in _context.EmpresasUsuarios
                            join u in _context.Usuarios on eu.UsuarioId equals u.Id
                            where eu.EmpresaId == empresaId && u.Estado == true
                            select new
                            {
                                u.Id,
                                u.Nombre
                            }).ToList();

                return Json(new { status = true, data = data });
            }
            catch (Exception ex) { return Json(new { status = false, message = ex.Message }); }
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

        #region 1. GESTIÓN DE CARPETAS Y LISTADOS

        // API ADMIN: Listar TODO
        [HttpGet]
        public JsonResult ListarTodasLasLiquidaciones()
        {
            try
            {
                var lista = (from l in _context.Liquidaciones
                             join e in _context.Empresas on l.EmpresaId equals e.Id
                             join u in _context.Usuarios on l.UsuarioId equals u.Id
                             join est in _context.Estados on l.EstadoId equals est.Id
                             orderby l.FechaInicio descending
                             select new
                             {
                                 l.Id,
                                 l.CodigoGenerado,
                                 Fecha = l.FechaInicio.ToString("dd/MM/yyyy"),
                                 Empresa = e.Nombre,
                                 Usuario = u.Nombre, // Empleado asignado
                                 Estado = est.Nombre
                             }).ToList();

                return Json(new { status = true, data = lista });
            }
            catch (Exception ex) { return Json(new { status = false, message = ex.Message }); }
        }

        // API USUARIO: Listar SOLO LO MÍO (Filtrado por el ID hardcodeado)
        [HttpGet]
        public JsonResult ListarMisLiquidaciones()
        {
            try
            {
                int userId = GetCurrentUserId(); // Devuelve 1 en pruebas

                var lista = (from l in _context.Liquidaciones
                             join e in _context.Empresas on l.EmpresaId equals e.Id
                             join est in _context.Estados on l.EstadoId equals est.Id
                             where l.UsuarioId == userId
                             orderby l.FechaInicio descending
                             select new
                             {
                                 l.Id,
                                 l.CodigoGenerado,
                                 Fecha = l.FechaInicio.ToString("dd/MM/yyyy"),
                                 Empresa = e.Nombre,
                                 Estado = est.Nombre
                             }).ToList();

                return Json(new { status = true, data = lista });
            }
            catch (Exception ex) { return Json(new { status = false, message = ex.Message }); }
        }

        // ACCIÓN ADMIN: Crear carpeta y asignar a usuario
        [HttpPost]
        public JsonResult CrearNuevaLiquidacion(int empresaId, int usuarioAsignadoId)
        {
            try
            {
                int adminId = GetCurrentUserId();

                string codigo = $"LIQ-{usuarioAsignadoId}-{DateTime.Now:ddMMHHmm}";

                var estadoAbierta = _context.Estados.FirstOrDefault(e => e.Tabla == "LIQUIDACION" && e.Nombre == "Abierta");
                if (estadoAbierta == null) return Json(new { status = false, message = "Error: Falta estado 'Abierta' en BD." });

                var liq = new Liquidacion
                {
                    EmpresaId = empresaId,
                    UsuarioId = usuarioAsignadoId,
                    CodigoGenerado = codigo,
                    EstadoId = estadoAbierta.Id,
                    UsuarioRegistro = adminId,
                    FechaInicio = DateTime.Now
                };

                _context.Liquidaciones.Add(liq);
                _context.SaveChanges();

                return Json(new { status = true, message = "Carpeta creada y asignada.", id = liq.Id });
            }
            catch (Exception ex) { return Json(new { status = false, message = ex.Message }); }
        }

        // DATOS CABECERA (Compartido)
        [HttpGet]
        public JsonResult ObtenerDatosCarpeta(int liquidacionId)
        {
            try
            {
                var data = (from l in _context.Liquidaciones
                            join e in _context.Empresas on l.EmpresaId equals e.Id
                            join u in _context.Usuarios on l.UsuarioId equals u.Id
                            join est in _context.Estados on l.EstadoId equals est.Id
                            where l.Id == liquidacionId
                            select new
                            {
                                l.Id,
                                l.CodigoGenerado,
                                Fecha = l.FechaInicio.ToString("dd/MM/yyyy"),
                                Estado = est.Nombre,
                                Empresa = e.Nombre,
                                Usuario = u.Nombre,
                                EsCerrada = est.Nombre == "Cerrada"
                            }).FirstOrDefault();

                return Json(new { status = true, data = data });
            }
            catch (Exception ex) { return Json(new { status = false, message = ex.Message }); }
        }

        #endregion

        #region 2. SECCIÓN A: INGRESOS (ANTICIPOS)

        [HttpGet]
        public JsonResult ListarAnticipos(int liquidacionId)
        {
            var lista = (from a in _context.Anticipos
                         join e in _context.Estados on a.EstadoId equals e.Id
                         join tr in _context.TipoRendiciones on a.TipoRendicionId equals tr.Id
                         join b in _context.Bancos on a.BancoId equals b.Id
                         join mo in _context.Monedas on a.MonedaId equals mo.Id
                         where a.LiquidacionId == liquidacionId
                         select new
                         {
                             a.Id,
                             Fecha = a.FechaSolicitud.HasValue ? a.FechaSolicitud.Value.ToString("dd/MM/yyyy") : "-",
                             FechaLimite = a.FechaLimiteRendicion.HasValue ? a.FechaLimiteRendicion.Value.ToString("dd/MM/yyyy") : "-",
                             TipoRendicion = tr.Descripcion,
                             Banco = b.Descripcion,
                             Moneda = mo.Nombre,
                             Estado = e.Nombre,
                             a.Monto,
                             a.Descripcion,
                             VoucherUrl = a.VoucherArchivoUrl,
                             VoucherOperacion = a.VoucherNumeroOperacion
                         }).ToList();

            return Json(new { status = true, data = lista });
        }

        // USUARIO: Solicita dinero
        [HttpPost]
        public JsonResult SolicitarAnticipo(Anticipo anticipo)
        {
            try
            {
                var estadoGenerado = _context.Estados.FirstOrDefault(e => e.Tabla == "ANTICIPO" && e.Nombre == "Generado");
                if (estadoGenerado == null) return Json(new { status = false, message = "Estado 'Generado' no encontrado." });

                anticipo.EstadoId = estadoGenerado.Id;
                anticipo.FechaSolicitud = DateTime.Now;

                // Defaults
                if (anticipo.MonedaId == null || anticipo.MonedaId == 0) anticipo.MonedaId = _context.Monedas.FirstOrDefault()?.Id;
                if (anticipo.TipoRendicionId == null || anticipo.TipoRendicionId == 0) anticipo.TipoRendicionId = _context.TipoRendiciones.FirstOrDefault()?.Id;

                _context.Anticipos.Add(anticipo);
                _context.SaveChanges();
                return Json(new { status = true, message = "Solicitud registrada." });
            }
            catch (Exception ex) { return Json(new { status = false, message = ex.Message }); }
        }

        // ADMIN: Registra transferencia (Sube Voucher)
        [HttpPost]
        public async Task<JsonResult> RegistrarTransferenciaAnticipo(int anticipoId, string nroOperacion, DateTime fechaTransferencia, IFormFile archivoVoucher)
        {
            try
            {
                var ant = await _context.Anticipos.FindAsync(anticipoId);
                if (ant == null) return Json(new { status = false, message = "Anticipo no encontrado" });

                if (archivoVoucher != null && archivoVoucher.Length > 0)
                {
                    string folder = Path.Combine(_webHostEnvironment.WebRootPath, "vouchers");
                    if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                    string fileName = $"VOUCHER_{ant.Id}_{Guid.NewGuid()}{Path.GetExtension(archivoVoucher.FileName)}";
                    using (var stream = new FileStream(Path.Combine(folder, fileName), FileMode.Create))
                    {
                        await archivoVoucher.CopyToAsync(stream);
                    }
                    ant.VoucherArchivoUrl = "/vouchers/" + fileName;
                }

                ant.VoucherNumeroOperacion = nroOperacion;
                ant.VoucherFecha = fechaTransferencia;

                var estadoTransferido = _context.Estados.FirstOrDefault(e => e.Tabla == "ANTICIPO" && e.Nombre == "Transferido");
                if (estadoTransferido != null) ant.EstadoId = estadoTransferido.Id;

                ant.UsuarioAprobador = GetCurrentUserId();

                await _context.SaveChangesAsync();
                return Json(new { status = true, message = "Transferencia registrada correctamente." });
            }
            catch (Exception ex) { return Json(new { status = false, message = ex.Message }); }
        }

        #endregion

        #region 3. SECCIÓN B: EGRESOS (COMPROBANTES Y PLANILLA)

        // --- COMPROBANTES ---

        [HttpGet]
        public JsonResult ListarComprobantes(int liquidacionId)
        {
            var lista = (from c in _context.Comprobantes
                         join e in _context.Estados on c.EstadoId equals e.Id
                         join tc in _context.TipoComprobantes on c.TipoComprobanteId equals tc.Id
                         join mo in _context.Monedas on c.MonedaId equals mo.Id
                         join co in _context.Conceptos on c.ConceptoId equals co.Id
                         where c.LiquidacionId == liquidacionId
                         select new
                         {
                             c.Id,
                             Fecha = c.FechaEmision.ToString("dd/MM/yyyy"),
                             Proveedor = c.ProveedorNombre,
                             Serie = c.Serie + "-" + c.Numero,
                             Monto = c.MontoTotal,
                             ArchivoUrl = c.ArchivoUrl,
                             TipoComprobante = tc.Descripcion,
                             Moneda = mo.Nombre,
                             Concepto = co.Nombre,
                             Estado = e.Nombre
                         }).ToList();
            return Json(new { status = true, data = lista });
        }

        // USUARIO: Sube Factura
        [HttpPost]
        public async Task<JsonResult> SubirComprobante(Comprobante comp, IFormFile archivo)
        {
            try
            {
                if (archivo != null && archivo.Length > 0)
                {
                    string folder = Path.Combine(_webHostEnvironment.WebRootPath, "comprobantes");
                    if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                    string fileName = $"CP_{Guid.NewGuid()}{Path.GetExtension(archivo.FileName)}";
                    using (var stream = new FileStream(Path.Combine(folder, fileName), FileMode.Create))
                    {
                        await archivo.CopyToAsync(stream);
                    }
                    comp.ArchivoUrl = "/comprobantes/" + fileName;
                }

                var estadoPendiente = _context.Estados.FirstOrDefault(e => e.Tabla == "COMPROBANTE" && e.Nombre == "Pendiente");
                comp.EstadoId = estadoPendiente != null ? estadoPendiente.Id : 1;
                comp.UsuarioRegistro = GetCurrentUserId();

                if (comp.TipoComprobanteId == 0) comp.TipoComprobanteId = _context.TipoComprobantes.FirstOrDefault()?.Id ?? 0;
                if (comp.MonedaId == 0) comp.MonedaId = _context.Monedas.FirstOrDefault()?.Id ?? 0;

                _context.Comprobantes.Add(comp);
                await _context.SaveChangesAsync();
                return Json(new { status = true, message = "Comprobante registrado." });
            }
            catch (Exception ex) { return Json(new { status = false, message = ex.Message }); }
        }

        // ADMIN: Aprueba o Rechaza
        [HttpPost]
        public JsonResult EvaluarComprobante(int id, string estado)
        {
            var comp = _context.Comprobantes.Find(id);
            if (comp != null)
            {
                var estadoDb = _context.Estados.FirstOrDefault(e => e.Tabla == "COMPROBANTE" && e.Nombre == estado);
                if (estadoDb != null)
                {
                    comp.EstadoId = estadoDb.Id;
                    comp.UsuarioAprobador = GetCurrentUserId();
                    _context.SaveChanges();
                    return Json(new { status = true });
                }
            }
            return Json(new { status = false, message = "Error actualizando estado." });
        }

        // --- PLANILLA MOVILIDAD ---

        [HttpGet]
        public JsonResult ListarDetallesPlanilla(int liquidacionId)
        {
            var planilla = _context.PlanillasMovilidad.FirstOrDefault(p => p.LiquidacionId == liquidacionId);

            if (planilla == null) return Json(new { status = true, data = new List<object>() });

            var detalles = _context.DetallesPlanillaMovilidad
                .Where(d => d.PlanillaMovilidadId == planilla.Id)
                .Select(d => new
                {
                    d.Id,
                    Fecha = d.FechaGasto.HasValue ? d.FechaGasto.Value.ToString("dd/MM/yyyy") : "-",
                    d.Motivo,
                    d.LugarOrigen,
                    d.LugarDestino,
                    d.Monto,
                    Aprobado = d.EstadoAprobacion
                }).ToList();

            return Json(new { status = true, data = detalles });
        }

        // USUARIO: Agrega item
        [HttpPost]
        public JsonResult AgregarDetallePlanilla(int liquidacionId, DetallePlanillaMovilidad detalle)
        {
            try
            {
                var planilla = _context.PlanillasMovilidad.FirstOrDefault(p => p.LiquidacionId == liquidacionId);
                if (planilla == null)
                {
                    var estadoActivo = _context.Estados.FirstOrDefault(e => e.Tabla == "PLANILLA_MOVILIDAD" && e.Nombre == "Activo");
                    planilla = new PlanillaMovilidad
                    {
                        LiquidacionId = liquidacionId,
                        NumeroPlanilla = "PL-" + DateTime.Now.Ticks,
                        EstadoId = estadoActivo?.Id ?? 1
                    };
                    _context.PlanillasMovilidad.Add(planilla);
                    _context.SaveChanges();
                }

                detalle.PlanillaMovilidadId = planilla.Id;
                detalle.EstadoAprobacion = true;
                _context.DetallesPlanillaMovilidad.Add(detalle);
                _context.SaveChanges();

                // Recalcular
                planilla.MontoTotalDeclarado = _context.DetallesPlanillaMovilidad
                    .Where(d => d.PlanillaMovilidadId == planilla.Id).Sum(d => d.Monto);
                _context.SaveChanges();

                return Json(new { status = true, message = "Ruta agregada." });
            }
            catch (Exception ex) { return Json(new { status = false, message = ex.Message }); }
        }

        // ADMIN: Rechaza item
        [HttpPost]
        public JsonResult RechazarItemPlanilla(int id)
        {
            var det = _context.DetallesPlanillaMovilidad.Find(id);
            if (det != null)
            {
                det.EstadoAprobacion = false;
                _context.SaveChanges();
                return Json(new { status = true });
            }
            return Json(new { status = false });
        }

        #endregion

        #region 4. CHAT (OBSERVACIONES)

        [HttpGet]
        public JsonResult ObtenerObservaciones(int liquidacionId)
        {
            try
            {
                int userId = GetCurrentUserId();

                var lista = (from o in _context.Observaciones
                             join u in _context.Usuarios on o.UsuarioId equals u.Id
                             where o.LiquidacionId == liquidacionId
                             orderby o.FechaCreacion ascending
                             select new
                             {
                                 o.Id,
                                 NombreUsuario = u.Nombre,
                                 EsMio = (o.UsuarioId == userId),
                                 o.Prioridad,
                                 o.Mensaje,
                                 Fecha = o.FechaCreacion.ToString("dd/MM HH:mm")
                             }).ToList();

                return Json(new { status = true, data = lista });
            }
            catch (Exception ex) { return Json(new { status = false, message = ex.Message }); }
        }

        [HttpPost]
        public JsonResult RegistrarObservacion(int liquidacionId, string mensaje, string prioridad)
        {
            try
            {
                var obs = new Observacion
                {
                    LiquidacionId = liquidacionId,
                    UsuarioId = GetCurrentUserId(),
                    Mensaje = mensaje,
                    Prioridad = prioridad ?? "B",
                    FechaCreacion = DateTime.Now
                };

                _context.Observaciones.Add(obs);
                _context.SaveChanges();

                return Json(new { status = true });
            }
            catch (Exception ex) { return Json(new { status = false, message = ex.Message }); }
        }

        #endregion

        #region 5. CEREBRO FINANCIERO Y CIERRE

        [HttpGet]
        public JsonResult ObtenerResumenFinanciero(int liquidacionId)
        {
            try
            {
                var estAntTransferido = _context.Estados.FirstOrDefault(e => e.Tabla == "ANTICIPO" && e.Nombre == "Transferido");
                var estCompAprobado = _context.Estados.FirstOrDefault(e => e.Tabla == "COMPROBANTE" && e.Nombre == "Aprobado");

                int idTransferido = estAntTransferido?.Id ?? -1;
                int idAprobado = estCompAprobado?.Id ?? -1;

                decimal ingresos = _context.Anticipos
                    .Where(a => a.LiquidacionId == liquidacionId && a.EstadoId == idTransferido)
                    .Sum(a => a.Monto);

                decimal gastosFactura = _context.Comprobantes
                    .Where(c => c.LiquidacionId == liquidacionId && c.EstadoId == idAprobado)
                    .Sum(c => c.MontoTotal);

                var planilla = _context.PlanillasMovilidad.FirstOrDefault(p => p.LiquidacionId == liquidacionId);
                decimal gastosPlanilla = 0;
                if (planilla != null)
                {
                    gastosPlanilla = _context.DetallesPlanillaMovilidad
                        .Where(d => d.PlanillaMovilidadId == planilla.Id && d.EstadoAprobacion == true)
                        .Sum(d => d.Monto);
                }

                decimal totalGastado = gastosFactura + gastosPlanilla;
                decimal saldo = ingresos - totalGastado;

                return Json(new
                {
                    status = true,
                    ingresos = ingresos,
                    gastos = totalGastado,
                    saldo = saldo
                });
            }
            catch (Exception ex) { return Json(new { status = false, message = ex.Message }); }
        }

        [HttpPost]
        public JsonResult CerrarLiquidacion(int liquidacionId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var estAntTransferido = _context.Estados.FirstOrDefault(e => e.Tabla == "ANTICIPO" && e.Nombre == "Transferido");
                    var estCompAprobado = _context.Estados.FirstOrDefault(e => e.Tabla == "COMPROBANTE" && e.Nombre == "Aprobado");

                    int idTransferido = estAntTransferido?.Id ?? -1;
                    int idAprobado = estCompAprobado?.Id ?? -1;

                    decimal ingresos = _context.Anticipos.Where(a => a.LiquidacionId == liquidacionId && a.EstadoId == idTransferido).Sum(a => a.Monto);
                    decimal gastosFact = _context.Comprobantes.Where(c => c.LiquidacionId == liquidacionId && c.EstadoId == idAprobado).Sum(c => c.MontoTotal);

                    var planilla = _context.PlanillasMovilidad.FirstOrDefault(p => p.LiquidacionId == liquidacionId);
                    decimal gastosPlanilla = (planilla != null)
                        ? _context.DetallesPlanillaMovilidad.Where(d => d.PlanillaMovilidadId == planilla.Id && d.EstadoAprobacion == true).Sum(d => d.Monto)
                        : 0;

                    decimal saldo = ingresos - (gastosFact + gastosPlanilla);

                    // 1. Reembolso
                    if (saldo != 0)
                    {
                        var estadoPendienteReem = _context.Estados.FirstOrDefault(e => e.Tabla == "REEMBOLSO" && e.Nombre == "Pendiente");
                        var cierre = new Reembolso
                        {
                            LiquidacionId = liquidacionId,
                            FechaSolicitud = DateTime.Now,
                            Monto = Math.Abs(saldo),
                            Descripcion = "Cierre automático de Liquidación",
                            EstadoId = estadoPendienteReem?.Id ?? 1,
                            UsuarioAprobador = GetCurrentUserId()
                        };

                        cierre.EsDevolucion = (saldo > 0);
                        _context.Reembolsos.Add(cierre);
                    }

                    // 2. Cierre
                    var liq = _context.Liquidaciones.Find(liquidacionId);
                    var estadoCerrada = _context.Estados.FirstOrDefault(e => e.Tabla == "LIQUIDACION" && e.Nombre == "Cerrada");

                    if (liq != null && estadoCerrada != null)
                    {
                        liq.EstadoId = estadoCerrada.Id;
                        liq.FechaCierre = DateTime.Now;
                        liq.TotalAnticipado = ingresos;
                        liq.TotalGastado = gastosFact + gastosPlanilla;
                        liq.SaldoFinal = saldo;
                    }

                    _context.SaveChanges();
                    transaction.Commit();

                    return Json(new { status = true, message = "Liquidación cerrada correctamente." });
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return Json(new { status = false, message = ex.Message });
                }
            }
        }

        #endregion

        // HELPER PARA PRUEBAS: Hardcodeado a Usuario ID 1
        private int GetCurrentUserId()
        {
            // En entorno real usarías:
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

            // Para tus pruebas locales:
            //return 1; // <--- Asegúrate que el Usuario con ID 1 existe en tu tabla 'usuario'
        }
    }
    #endregion
}