using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YnnovaComprobantes.Models
{
    [Table("anticipo")]
    public class Anticipo
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("liquidacion_id")]
        public int LiquidacionId { get; set; }

        [Column("banco_id")]
        public int? BancoId { get; set; }

        [Column("moneda_id")]
        public int? MonedaId { get; set; }

        [Column("tipo_rendicion_id")]
        public int? TipoRendicionId { get; set; }

        [Column("monto")]
        public decimal Monto { get; set; }

        [Column("descripcion")]
        public string? Descripcion { get; set; }

        [Column("fecha_solicitud")]
        public DateTime? FechaSolicitud { get; set; }

        [Column("fecha_limite_rendicion")]
        public DateTime? FechaLimiteRendicion { get; set; }

        // === CAMPOS DEL VOUCHER (TRANSFERENCIA ADMIN) ===
        [Column("voucher_numero_operacion")]
        public string? VoucherNumeroOperacion { get; set; }

        [Column("voucher_fecha")]
        public DateTime? VoucherFecha { get; set; }

        [Column("voucher_archivo_url")]
        public string? VoucherArchivoUrl { get; set; }

        [Column("voucher_banco_origen_id")]
        public int? VoucherBancoOrigenId { get; set; }
        // ================================================

        [Column("estado_id")]
        public int? EstadoId { get; set; }

        [Column("usuario_aprobador")]
        public int? UsuarioAprobador { get; set; }

        [Column("fecha_registro")]
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
    }
}