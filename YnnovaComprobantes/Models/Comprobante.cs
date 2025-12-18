using System.ComponentModel.DataAnnotations.Schema;

namespace YnnovaComprobantes.Models
{
    [Table("Comprobante")]
    public class Comprobante
    {
        public int Id { get; set; }
        [Column("usuario_id")]
        public int UsuarioId { get; set; }
        [Column("gasto_id")]
        public int? GastoId { get; set; }
        [Column("tipo_comprobante_id")]
        public int TipoComprobanteId { get; set; }
        [Column("concepto_id")]
        public int? ConceptoId { get; set; }
        [Column("concepto_otro")]
        public string? ConceptoOtro { get; set; }
        public string? Serie { get; set; }
        public string? Numero { get; set; }
        [Column("ruc_empresa")]
        public string? RucEmpresa { get; set; }
        public decimal Monto { get; set; }
        public DateOnly Fecha { get; set; }
        public string? Descripcion { get; set; }
        public string? Archivo { get; set; }
        [Column("estado_id")]
        public int EstadoId { get; set; }
        [Column("moneda_id")]
        public int MonedaId { get; set; }
        [Column("usuario_aprobador")]
        public int? UsuarioAprobador { get; set; }
        [Column("fecha_registro")]
        public DateTime FechaRegistro { get; set; }
    }
}
