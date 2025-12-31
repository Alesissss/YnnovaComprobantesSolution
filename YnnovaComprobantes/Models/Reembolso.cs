using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YnnovaComprobantes.Models
{
    [Table("reembolso")]
    public class Reembolso
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("liquidacion_id")]
        public int LiquidacionId { get; set; }

        [Column("fecha_solicitud")]
        public DateTime? FechaSolicitud { get; set; }

        [Column("moneda_id")]
        public int? MonedaId { get; set; }

        [Column("monto")]
        public decimal? Monto { get; set; }

        [Column("descripcion")]
        public string? Descripcion { get; set; }

        [Column("banco_id")]
        public int? BancoId { get; set; }

        [Column("numero_cuenta")]
        public string? NumeroCuenta { get; set; }

        [Column("es_devolucion")]
        public bool? EsDevolucion { get; set; } // 1: Usuario devuelve, 0: Empresa paga

        // === NUEVOS CAMPOS VOUCHER ===
        [Column("voucher_numero_operacion")]
        public string? VoucherNumeroOperacion { get; set; }

        [Column("voucher_fecha")]
        public DateTime? VoucherFecha { get; set; }

        [Column("voucher_archivo_url")]
        public string? VoucherArchivoUrl { get; set; }
        // ==============================

        [Column("estado_id")]
        public int? EstadoId { get; set; }

        [Column("usuario_registro")]
        public int? UsuarioRegistro { get; set; } // Admin o Usuario

        [Column("fecha_registro")]
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
    }
}