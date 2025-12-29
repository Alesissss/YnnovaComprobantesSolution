using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YnnovaComprobantes.Models
{
    [Table("devolucion_anticipo")]
    public class DevolucionAnticipo
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("anticipo_id")]
        public int AnticipoId { get; set; }

        [Column("monto")]
        public decimal Monto { get; set; }

        [Column("fecha_devolucion")]
        public DateTime FechaDevolucion { get; set; }

        [Column("estado_id")]
        public int EstadoId { get; set; }

        [Column("usuario_aprobador")]
        public int? UsuarioAprobador { get; set; }

        [Column("usuario_registro")]
        public int UsuarioRegistro { get; set; }

        [Column("fecha_registro")]
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
    }
}