using System.ComponentModel.DataAnnotations.Schema;

namespace YnnovaComprobantes.Models
{
    [Table("Tipo_Usuario")]
    public class TipoUsuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
    }
}
