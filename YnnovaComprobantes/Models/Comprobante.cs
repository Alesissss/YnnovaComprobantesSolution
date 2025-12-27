using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YnnovaComprobantes.Models
{
    [Table("comprobante")]
    public class Comprobante
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("usuario_id")]
        public int UsuarioId { get; set; }

        [Column("anticipo_id")]
        public int? AnticipoId { get; set; }

        [Column("reembolso_id")]
        public int? ReembolsoId { get; set; }

        [Column("planilla_movilidad_id")]
        public int? PlanillaMovilidadId { get; set; }

        [Column("tipo_comprobante_id")]
        public int TipoComprobanteId { get; set; }

        [Column("concepto_id")]
        public int? ConceptoId { get; set; }

        [Column("proveedor_nombre")]
        public string? ProveedorNombre { get; set; }

        [Column("ruc_empresa")]
        public string? RucEmpresa { get; set; }

        [Column("serie")]
        public string? Serie { get; set; }

        [Column("numero")]
        public string? Numero { get; set; }

        [Column("fecha_emision")]
        public DateTime FechaEmision { get; set; }

        [Column("moneda_id")]
        public int MonedaId { get; set; }

        [Column("monto_total", TypeName = "decimal(10, 2)")]
        public decimal MontoTotal { get; set; }

        [Column("descripcion")]
        public string? Descripcion { get; set; }

        [Column("archivo_url")]
        public string? ArchivoUrl { get; set; }

        [Column("estado_id")]
        public int EstadoId { get; set; }

        [Column("fecha_registro")]
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
    }
}