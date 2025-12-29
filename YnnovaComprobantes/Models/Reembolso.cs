using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YnnovaComprobantes.Models
{
    [Table("reembolso")]
    public class Reembolso
    {
        [Key]
        [Column("id")]
        public int? Id { get; set; }
        [Column("anticipo_id")]
        public int? AnticipoId { get; set; }

        [Column("empresa_id")]
        public int? EmpresaId { get; set; }

        [Column("usuario_id")]
        public int? UsuarioId { get; set; }

        [Column("fecha_solicitud")]
        public DateTime? FechaSolicitud { get; set; }

        [Column("moneda_id")]
        public int? MonedaId { get; set; }

        [Column("monto_total")]
        public decimal? MontoTotal { get; set; }

        [Column("descripcion")]
        public string? Descripcion { get; set; }

        [Column("banco_id")]
        public int? BancoId { get; set; }

        [Column("numero_cuenta")]
        public string? NumeroCuenta { get; set; }

        [Column("estado_id")]
        public int? EstadoId { get; set; }
        [Column("usuario_aprobador")]
        public int? UsuarioAprobador { get; set; }
        [Column("usuario_registro")]
        public int? UsuarioRegistro { get; set; }

        [Column("fecha_registro")]
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
    }
}