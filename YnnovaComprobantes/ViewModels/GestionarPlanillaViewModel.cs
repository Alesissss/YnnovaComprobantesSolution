using YnnovaComprobantes.Models;

namespace YnnovaComprobantes.ViewModels
{
    public class GestionPlanillaViewModel
    {
        public PlanillaMovilidad Planilla { get; set; }
        public Estado Estado { get; set; }
        public int UsuarioLogueadoId { get; set; }
        // Listas para el Admin (Selects)
        public List<Empresa> Empresas { get; set; }
        public List<Usuario> Usuarios { get; set; }
    }
}
