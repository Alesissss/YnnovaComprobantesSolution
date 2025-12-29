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
    public class PlanillaMovilidadController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PlanillaMovilidadController(ApplicationDbContext context)
        {
            _context = context;
        }

        #region VISTAS

        // Vista principal (Listado)
        public IActionResult Index()
        {
            return View();
        }

        // Vista para que el Admin registre la cabecera
        public IActionResult Registrar()
        {
            return View();
        }

        // Vista para que el Usuario agregue los detalles o el Admin evalúe
        [HttpGet]
        public IActionResult Gestionar(int id)
        {
            // 1. Obtener la cabecera de la planilla
            var planilla = _context.PlanillasMovilidad.FirstOrDefault(p => p.Id == id);
            if (planilla == null) return NotFound();

            // 2. Obtener el estado para el badge
            var estado = _context.Estados.FirstOrDefault(e => e.Id == planilla.EstadoId);

            // 3. Obtener el ID del usuario logueado para la lógica del chat
            int usuarioLogueadoId = 0;
            var claimId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (claimId != null && int.TryParse(claimId.Value, out int uid))
                usuarioLogueadoId = uid;

            // 4. Mapear al ViewModel con la data necesaria para las etiquetas de la vista
            var model = new YnnovaComprobantes.ViewModels.GestionPlanillaViewModel
            {
                Planilla = planilla,
                Estado = estado ?? new Estado { Nombre = "Pendiente" },
                UsuarioLogueadoId = usuarioLogueadoId,

                // Listas completas para que los .FirstOrDefault en la vista funcionen
                Empresas = _context.Empresas.ToList(),
                Usuarios = _context.Usuarios.ToList()
            };

            return View(model);
        }
        [HttpGet]
        public IActionResult Editar(int id)
        {
            var planilla = _context.PlanillasMovilidad.FirstOrDefault(p => p.Id == id);
            if (planilla == null) return NotFound();

            var estado = _context.Estados.FirstOrDefault(e => e.Id == planilla.EstadoId);

            // Obtener usuario logueado para el chat
            int usuarioLogueadoId = 0;
            var claimId = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claimId != null && int.TryParse(claimId.Value, out int uid)) usuarioLogueadoId = uid;

            var model = new GestionPlanillaViewModel
            {
                Planilla = planilla,
                Estado = estado ?? new Estado { Nombre = "Pendiente" },
                UsuarioLogueadoId = usuarioLogueadoId,
                // Listas para los combos de edición
                Empresas = _context.Empresas.ToList(),
                Usuarios = _context.Usuarios.ToList()
            };

            return View(model);
        }

        #endregion

        #region APIS - CABECERA (PLANILLA)

        [HttpGet]
        public JsonResult GetPlanillasData()
        {
            try
            {
                // Filtramos según rol: Si no es Admin, solo ve las suyas (opcional, ajusta según tu lógica de roles)
                // Aquí asumo que el Admin ve todo.

                var query = from p in _context.PlanillasMovilidad
                            join e in _context.Empresas on p.EmpresaId equals e.Id
                            join u in _context.Usuarios on p.UsuarioId equals u.Id
                            join est in _context.Estados on p.EstadoId equals est.Id
                            select new
                            {
                                p.Id,
                                p.NumeroPlanilla,
                                FechaEmision = p.FechaEmision.ToString("yyyy-MM-dd"),
                                Empresa = e.Nombre,
                                Usuario = u.Nombre,
                                MontoAsignado = p.MontoTotal,
                                Estado = est.Nombre
                            };

                return Json(new { data = query.ToList(), status = true });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = ex.Message });
            }
        }
        [HttpGet]
        public JsonResult GetMisPlanillasData()
        {
            try
            {
                int usuarioLogueadoId = 0;
                var claimId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                if (claimId != null && int.TryParse(claimId.Value, out int uid))
                    usuarioLogueadoId = uid;

                if (usuarioLogueadoId == 0)
                    return Json(new { status = false, message = "Usuario no identificado." });

                var query = from p in _context.PlanillasMovilidad
                            join e in _context.Empresas on p.EmpresaId equals e.Id
                            join u in _context.Usuarios on p.UsuarioId equals u.Id
                            join est in _context.Estados on p.EstadoId equals est.Id
                            where u.Id == usuarioLogueadoId
                            select new
                            {
                                p.Id,
                                p.NumeroPlanilla,
                                FechaEmision = p.FechaEmision.ToString("yyyy-MM-dd"),
                                Empresa = e.Nombre,
                                Usuario = u.Nombre,
                                MontoAsignado = p.MontoTotal,
                                Estado = est.Nombre
                            };

                return Json(new { data = query.ToList(), status = true });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult RegistrarPlanilla(PlanillaMovilidad planilla)
        {
            try
            {
                // 1. Asignar Estado Inicial (Pendiente)
                var estadoPendiente = _context.Estados.FirstOrDefault(e => e.Tabla == "PLANILLA_MOVILIDAD" && e.Nombre == "Pendiente");
                planilla.EstadoId = estadoPendiente?.Id ?? 1;

                planilla.FechaRegistro = DateTime.Now;

                // 2. Generar Correlativo de 10 dígitos
                // Obtenemos el ID más alto actual
                int ultimoId = 0;
                if (_context.PlanillasMovilidad.Any())
                {
                    ultimoId = _context.PlanillasMovilidad.Max(p => p.Id);
                }

                // El nuevo número será el último ID + 1, formateado a 10 dígitos con ceros a la izquierda
                int siguienteId = ultimoId + 1;
                planilla.NumeroPlanilla = siguienteId.ToString("D10");
                // Resultado ej: "0000000001", "0000000002", etc.

                _context.PlanillasMovilidad.Add(planilla);
                _context.SaveChanges();

                return Json(new { status = true, message = "Planilla de movilidad creada correctamente con el número: " + planilla.NumeroPlanilla });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = ex.Message });
            }
        }
        #endregion

        #region APIS - DETALLES

        [HttpGet]
        public JsonResult GetDetallesData(int planillaId)
        {
            try
            {
                var detalles = _context.DetallesPlanillaMovilidad
                                .Where(d => d.PlanillaMovilidadId == planillaId)
                                .OrderBy(d => d.FechaGasto)
                                .Select(d => new
                                {
                                    d.Id,
                                    FechaGasto = d.FechaGasto.ToString("dd/MM/yyyy"),
                                    d.Motivo,
                                    d.LugarOrigen,
                                    d.LugarDestino,
                                    d.Monto
                                })
                                .ToList();

                // Calculamos cuánto se ha gastado hasta ahora
                decimal totalGastado = detalles.Sum(d => d.Monto);

                return Json(new { data = detalles, totalGastado = totalGastado, status = true });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult RegistrarDetalle(DetallePlanillaMovilidad detalle)
        {
            try
            {
                // 1. Validar Tope de Monto
                var planilla = _context.PlanillasMovilidad.Find(detalle.PlanillaMovilidadId);
                if (planilla == null) return Json(new { status = false, message = "Planilla no encontrada." });

                // Suma actual de detalles ya registrados
                var sumaActual = _context.DetallesPlanillaMovilidad
                                    .Where(d => d.PlanillaMovilidadId == planilla.Id)
                                    .Sum(d => d.Monto);

                // Validamos: (Lo gastado + lo nuevo) <= Monto Asignado
                if ((sumaActual + detalle.Monto) > planilla.MontoTotal)
                {
                    decimal restante = planilla.MontoTotal - sumaActual;
                    return Json(new { status = false, message = $"El monto excede el límite asignado. Saldo disponible: S/ {restante:N2}" });
                }

                // 2. Guardar
                _context.DetallesPlanillaMovilidad.Add(detalle);
                _context.SaveChanges();

                return Json(new { status = true, message = "Detalle agregado." });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult EliminarDetalle(int id)
        {
            try
            {
                var det = _context.DetallesPlanillaMovilidad.Find(id);
                if (det != null)
                {
                    _context.DetallesPlanillaMovilidad.Remove(det);
                    _context.SaveChanges();
                    return Json(new { status = true, message = "Eliminado correctamente." });
                }
                return Json(new { status = false, message = "No encontrado." });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = ex.Message });
            }
        }
        [HttpGet]
        public JsonResult GetObservaciones(int planillaId)
        {
            try
            {
                var lista = (from o in _context.Observaciones
                             join u in _context.Usuarios on o.UsuarioId equals u.Id
                             where o.PlanillaMovilidadId == planillaId // Nueva columna en BD
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
        public async Task<IActionResult> RegistrarObservacion(int PlanillaId, string Mensaje, string Prioridad)
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
                        PlanillaMovilidadId = PlanillaId,
                        UsuarioId = userId,
                        Mensaje = Mensaje,
                        Prioridad = Prioridad,
                        FechaCreacion = DateTime.Now
                    };
                    _context.Observaciones.Add(nuevaObs);
                    _context.SaveChanges();

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

        #endregion

        #region HELPERS (SELECTS)

        [HttpGet]
        public JsonResult GetEmpresaData()
        {
            var data = _context.Empresas.Where(e => e.Estado == true).Select(e => new { e.Id, e.Nombre, e.Ruc }).ToList();
            return Json(new { data, status = true });
        }

        [HttpGet]
        public JsonResult GetUsuariosData(int EmpresaId)
        {
            // Usando tu lógica de unión con EmpresasUsuarios
            var data = (from u in _context.Usuarios
                        join eu in _context.EmpresasUsuarios on u.Id equals eu.UsuarioId
                        where eu.EmpresaId == EmpresaId && u.Estado == true
                        select new { u.Id, u.Nombre, u.Dni }).ToList();

            return Json(new { data, status = true });
        }

        #endregion

        [HttpPost]
        public JsonResult ActualizarPlanillaDatos(PlanillaMovilidad datos)
        {
            try
            {
                var db = _context.PlanillasMovilidad.Find(datos.Id);
                if (db == null) return Json(new { status = false, message = "Planilla no encontrada" });

                db.EmpresaId = datos.EmpresaId;
                db.UsuarioId = datos.UsuarioId;
                db.FechaEmision = datos.FechaEmision;
                db.MontoTotal = datos.MontoTotal; // El admin puede ajustar el presupuesto tope

                _context.SaveChanges();
                return Json(new { status = true, message = "Cabecera de planilla actualizada." });
            }
            catch (Exception ex) { return Json(new { status = false, message = ex.Message }); }
        }
    }
}