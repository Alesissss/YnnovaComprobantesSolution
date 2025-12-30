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

        [Column("liquidacion_id")]
        public int LiquidacionId { get; set; }

        [Column("numero_planilla")]
        public string? NumeroPlanilla { get; set; }

        [Column("fecha_emision")]
        public DateTime? FechaEmision { get; set; }

        [Column("monto_total_declarado")]
        public decimal MontoTotalDeclarado { get; set; }

        [Column("estado_id")]
        public int? EstadoId { get; set; }

        [Column("fecha_registro")]
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
    }
}