using Microsoft.AspNetCore.Mvc;
using YnnovaComprobantes.Data;

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
    }
}
