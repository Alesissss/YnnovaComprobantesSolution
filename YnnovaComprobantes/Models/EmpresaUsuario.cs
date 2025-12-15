using System.ComponentModel.DataAnnotations.Schema;

namespace YnnovaComprobantes.Models
{
    [Table("Empresa_Usuario")]
    public class EmpresaUsuario
    {
        public int Id { get; set; }
        [Column("Empresa_Id")]
        public int EmpresaId { get; set; }
        [Column("Usuario_Id")]
        public int UsuarioId { get; set; }
    }
}
