using System.ComponentModel.DataAnnotations.Schema;

namespace YnnovaComprobantes.Models
{
    [Table("Moneda")]
    public class Moneda
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Simbolo { get; set; }
    }
}
