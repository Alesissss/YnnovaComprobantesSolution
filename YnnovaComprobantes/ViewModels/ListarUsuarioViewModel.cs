using YnnovaComprobantes.Models;

namespace YnnovaComprobantes.ViewModels
{
    public class ListarUsuarioViewModel
    {
        public int Id { get; set; }
        public string Dni { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public bool Estado { get; set; }
        // Datos de empresa y tipo usuario por empresa
        public List<EmpresaUsuario> EmpresaUsuarios { get; set; }
    }
}
