using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;
using YnnovaComprobantes.Data;
using YnnovaComprobantes.Models;
using YnnovaComprobantes.ViewModels;

namespace YnnovaComprobantes.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var tipoUsuario = User.FindFirst("TipoUsuario")?.Value;

            if (string.IsNullOrEmpty(tipoUsuario))
            {
                return View(); // Muestra la Home pública si no hay tipo definido
            }

            return tipoUsuario switch
            {
                "Administrador del sistema" => View(),
                "Usuario" => RedirectToAction("IndexUsuario", "Liquidacion"),
                _ => View()
            };
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpGet]
        public JsonResult GetEmpresaData()
        {
            try
            {
                var empresaData = _context.Empresas.Where(e => e.Estado == true).ToList();
                return Json(new { data = empresaData, message = "Empresas retornadas exitosamente.", status = true });
            }
            catch (Exception ex)
            {
                return Json(new ApiResponse { data = null, message = ex.Message, status = false });
            }
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<JsonResult> IniciarSesion(string dni, string password, string ruc)
        {
            try
            {
                var usuario = (from eu in _context.EmpresasUsuarios
                               from tu in _context.TipoUsuarios
                               where eu.TipoUsuarioId == tu.Id
                               where eu.EmpresaId == (_context.Empresas.Where(e => e.Ruc == ruc).Select(e => e.Id)).FirstOrDefault()
                               from u in _context.Usuarios
                               where eu.UsuarioId == u.Id
                               where u.Dni == dni
                               where u.Password == password
                               where u.Estado == true
                               select new
                               {
                                   u.Id,
                                   u.Dni,
                                   u.Nombre,
                                   tipoUsuario = tu.Nombre,
                               }).FirstOrDefault();

                if (usuario == null)
                {
                    return Json(new ApiResponse { data = null, message = "Credenciales incorrectas.", status = false });
                }
                else
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, usuario.Nombre),
                        new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                        new Claim("RUC", ruc),
                        new Claim("DNI", usuario.Dni),
                        new Claim("TipoUsuario", usuario.tipoUsuario)
                    };

                    var claimsIdentity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {
                        // Puedes configurar la duración de la cookie si lo deseas
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(60)
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    return Json(new ApiResponse { data = null, message = "Login satisfactorio.", status = true });
                }
            }
            catch (Exception ex)
            {
                return Json(new ApiResponse { data = null, message = ex.Message, status = false });
            }
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            // 1. Eliminar la Cookie de Autenticación
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // 2. Eliminar los datos de la Sesión Estándar (Lista de Empresas, etc.)
            HttpContext.Session.Clear();

            // 3. Redirigir al usuario a la página de Login
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public async Task<IActionResult> GetDashboardData(DateTime? fechaInicio, DateTime? fechaFin)
        {
            try
            {
                // 1. Configurar rango de fechas (Si es nulo, todo el año actual)
                var inicio = fechaInicio ?? new DateTime(DateTime.Now.Year, 1, 1);
                var fin = fechaFin ?? new DateTime(DateTime.Now.Year, 12, 31).AddHours(23).AddMinutes(59);

                // ==============================================================================
                // CONSULTA 1: COMPROBANTES (Join Manual: Comprobante -> Estado, Comprobante -> Concepto)
                // ==============================================================================
                // Traemos los datos planos a memoria para poder agrupar por mes sin problemas de traducción SQL
                var queryComprobantes = from c in _context.Comprobantes
                                        join e in _context.Estados on c.EstadoId equals e.Id
                                        join con in _context.Conceptos on c.ConceptoId equals con.Id into conJoin
                                        from con in conJoin.DefaultIfEmpty() // Left Join por si concepto es nulo
                                        where c.FechaEmision >= inicio && c.FechaEmision <= fin
                                        select new
                                        {
                                            c.MontoTotal,
                                            EstadoNombre = e.Nombre,
                                            c.FechaEmision,
                                            ConceptoNombre = con != null ? con.Nombre : "Sin Concepto"
                                        };

                var dataComprobantes = await queryComprobantes.ToListAsync();

                // ==============================================================================
                // CONSULTA 2: LIQUIDACIONES (Join Manual: Liquidacion -> Estado)
                // ==============================================================================
                var queryLiquidaciones = from l in _context.Liquidaciones
                                         join e in _context.Estados on l.EstadoId equals e.Id
                                         where l.FechaRegistro >= inicio && l.FechaRegistro <= fin
                                         select new
                                         {
                                             l.Id,
                                             EstadoNombre = e.Nombre
                                         };

                var dataLiquidaciones = await queryLiquidaciones.ToListAsync();

                // ==============================================================================
                // CONSULTA 3: FLUJO DE DINERO (Anticipos vs Reembolsos)
                // ==============================================================================

                // Suma de Anticipos (Dinero entregado)
                var totalAnticipos = await _context.Anticipos
                    .Where(a => a.FechaRegistro >= inicio && a.FechaRegistro <= fin)
                    .SumAsync(a => (decimal?)a.Monto) ?? 0;

                // Suma de Reembolsos (Dinero devuelto por la empresa al usuario)
                var totalReembolsos = await _context.Reembolsos
                    .Where(r => r.FechaRegistro >= inicio && r.FechaRegistro <= fin && r.EsDevolucion == false)
                    .SumAsync(r => (decimal?)r.Monto) ?? 0;

                // Suma de Devoluciones (Dinero devuelto por el usuario a la empresa)
                var totalDevoluciones = await _context.Reembolsos
                    .Where(r => r.FechaRegistro >= inicio && r.FechaRegistro <= fin && r.EsDevolucion == true)
                    .SumAsync(r => (decimal?)r.Monto) ?? 0;


                // ==============================================================================
                // ARMADO DEL DASHBOARD VM
                // ==============================================================================
                var dashboard = new DashboardVM();

                // A. KPIs
                dashboard.GastoTotal = dataComprobantes.Sum(x => x.MontoTotal);
                dashboard.Pendientes = dataComprobantes.Count(x => x.EstadoNombre == "Pendiente");
                dashboard.Aprobados = dataComprobantes.Count(x => x.EstadoNombre == "Aprobado");
                dashboard.Observados = dataComprobantes.Count(x => x.EstadoNombre == "Observado" || x.EstadoNombre == "Rechazado");

                // B. Gráfico 1: Evolución de Gastos (Agrupado por Mes-Año)
                var gastosAgrupados = dataComprobantes
                    .GroupBy(x => x.FechaEmision.ToString("MMM yyyy")) // Ej: "Ene 2025"
                    .Select(g => new { Mes = g.Key, Total = g.Sum(x => x.MontoTotal), Orden = g.Min(x => x.FechaEmision) })
                    .OrderBy(x => x.Orden)
                    .ToList();

                dashboard.EvolucionGastos = new ChartDataVM
                {
                    Labels = gastosAgrupados.Select(x => x.Mes).ToList(),
                    Values = gastosAgrupados.Select(x => x.Total).ToList()
                };

                // C. Gráfico 2: Distribución de Flujo
                dashboard.DistribucionDinero = new List<PieDataVM>
            {
                new PieDataVM { Name = "Anticipos (Entregado)", Value = totalAnticipos },
                new PieDataVM { Name = "Reembolsos (Pagado)", Value = totalReembolsos },
                new PieDataVM { Name = "Devoluciones (Recuperado)", Value = totalDevoluciones }
            };

                // D. Gráfico 3: Top Conceptos
                var topConceptos = dataComprobantes
                    .GroupBy(x => x.ConceptoNombre)
                    .Select(g => new { Concepto = g.Key, Total = g.Sum(x => x.MontoTotal) })
                    .OrderByDescending(x => x.Total)
                    .Take(5)
                    .ToList();

                dashboard.GastosPorConcepto = new ChartDataVM
                {
                    Labels = topConceptos.Select(x => x.Concepto).ToList(),
                    Values = topConceptos.Select(x => x.Total).ToList()
                };

                // E. Gráfico 4: Estado de Liquidaciones
                var estadosLiq = dataLiquidaciones
                    .GroupBy(x => x.EstadoNombre)
                    .Select(g => new { Estado = g.Key, Cantidad = g.Count() })
                    .ToList();

                dashboard.EstadoLiquidaciones = new ChartDataVM
                {
                    Labels = estadosLiq.Select(x => x.Estado).ToList(),
                    Values = estadosLiq.Select(x => (decimal)x.Cantidad).ToList()
                };

                return Json(new { status = true, data = dashboard });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = "Error: " + ex.Message });
            }
        }
    }
}
