using System.ComponentModel.DataAnnotations.Schema;

namespace YnnovaComprobantes.Models
{
    [Table("Observacion")]
    public class Observacion
    {
        public int Id { get; set; }
        [Column("comprobante_id")]
        public int ComprobanteId { get; set; }
        [Column("usuario_id")]
        public int UsuarioId { get; set; }
        public string? Prioridad { get; set; }
        public string? Mensaje { get; set; }
        [Column("fecha_creacion")]
        public DateTime? FechaCreacion { get; set; }
    }
}
