using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;
using YnnovaComprobantes.Data;
using YnnovaComprobantes.Models;

namespace YnnovaComprobantes.Controllers
{
    //[Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
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
        [HttpPost]
        public async Task<JsonResult> IniciarSesion(string dni, string password)
        {
            try
            {
                var usuario = _context.Usuarios.FirstOrDefault(u => u.Dni == dni && u.Password == password);

                if (usuario == null)
                {
                    return Json(new ApiResponse { data = null, message = "Credenciales incorrectas.", status = false });
                }
                else
                {
                    var empresas = (from emp in _context.Empresas
                                    from eu in _context.EmpresasUsuarios
                                    where emp.Id == eu.EmpresaId
                                    where eu.UsuarioId == usuario.Id
                                    select emp).ToList();

                    var tipoUsuario = _context.TipoUsuarios.FirstOrDefault(tu => tu.Id == usuario.TipoUsuarioId);

                    string nombreTipoUsuario = tipoUsuario != null ? tipoUsuario.Nombre : "SinRol";

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, usuario.Nombre),
                        new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                        new Claim("DNI", usuario.Dni),
                        new Claim("TipoUsuario", tipoUsuario.Nombre)
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

                    GuardarEmpresasEnSesion(empresas);

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
            if (HttpContext.Session != null)
                HttpContext.Session.Clear();

            // 3. Redirigir al usuario a la página de Login
            return RedirectToAction("Index", "Home");
        }

        private void GuardarEmpresasEnSesion(List<Empresa> empresas)
        {
            // Usa la extensión SetObjectAsJson para guardar la lista
            HttpContext.Session.SetObjectAsJson("EmpresasUsuario", empresas);
        }
    }
}
