using YnnovaComprobantes.Models;

namespace YnnovaComprobantes.ViewModels
{
    public class EditarReembolsoViewModel
    {
        public Reembolso? Reembolso { get; set; }
        public Estado? Estado { get; set; }
        public int? UsuarioLogueadoId { get; set; }
        // Listas para que Razor renderice nombres sin AJAX inicial
        public List<Empresa>? Empresas { get; set; }
        public List<Usuario>? Usuarios { get; set; }
        public List<Banco>? Bancos { get; set; }
        public List<Moneda>? Monedas { get; set; }
    }
}
