using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YnnovaComprobantes.Models
{
    [Table("detalle_planilla_movilidad")]
    public class DetallePlanillaMovilidad
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("planilla_movilidad_id")]
        public int PlanillaMovilidadId { get; set; }

        [Column("fecha_gasto")]
        public DateTime FechaGasto { get; set; }

        [Column("motivo")]
        public string? Motivo { get; set; }

        [Column("lugar_origen")]
        public string? LugarOrigen { get; set; }

        [Column("lugar_destino")]
        public string? LugarDestino { get; set; }

        [Column("monto")]
        public decimal Monto { get; set; }
    }
}