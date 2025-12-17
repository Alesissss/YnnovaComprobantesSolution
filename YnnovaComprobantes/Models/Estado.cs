using System.ComponentModel.DataAnnotations.Schema;

namespace YnnovaComprobantes.Models
{
    [Table("Estado")]
    public class Estado
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Tabla { get; set; }
    }
}
