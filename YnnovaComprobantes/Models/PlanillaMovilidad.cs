using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YnnovaComprobantes.Models
{
    [Table("planilla_movilidad")]
    public class PlanillaMovilidad
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("usuario_id")]
        public int UsuarioId { get; set; }

        [Column("empresa_id")]
        public int EmpresaId { get; set; }

        [Column("numero_planilla")]
        public string? NumeroPlanilla { get; set; }

        [Column("fecha_emision")]
        public DateTime FechaEmision { get; set; }

        // Este es el monto TOPE o ASIGNADO que el usuario no debe exceder
        [Column("monto_total")]
        public decimal MontoTotal { get; set; }

        [Column("estado_id")]
        public int EstadoId { get; set; }
        [Column("usuario_aprobador")]
        public int? UsuarioAprobador { get; set; }
        [Column("usuario_registro")]
        public int? UsuarioRegistro { get; set; }

        [Column("fecha_registro")]
        public DateTime FechaRegistro { get; set; }
    }
}