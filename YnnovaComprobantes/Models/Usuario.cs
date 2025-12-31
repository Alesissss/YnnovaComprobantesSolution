using System.ComponentModel.DataAnnotations.Schema;

namespace YnnovaComprobantes.Models
{
    [Table("Usuario")]
    public class Usuario
    {
        public int Id { get; set; }
        public string? Dni { get; set; }
        public string? Nombre { get; set; }
        public string? Email { get; set; }
        public string? Telefono { get; set; }
        [Column("numero_cuenta")]
        public string? NumeroCuenta { get; set; }
        public string? Password { get; set; }
        public bool? Estado { get; set; }
    }
}
