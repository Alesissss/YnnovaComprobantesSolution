using System.ComponentModel.DataAnnotations.Schema;

namespace YnnovaComprobantes.Models
{
    [Table("anticipo")]
    public class Anticipo
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("empresa_id")]
        public int? EmpresaId { get; set; }

        [Column("usuario_id")]
        public int? UsuarioId { get; set; }

        [Column("banco_id")]
        public int? BancoId { get; set; }

        [Column("moneda_id")]
        public int? MonedaId { get; set; }
        [Column("tipo_rendicion_id")]
        public int? TipoRendicionId { get; set; }

        [Column("monto", TypeName = "decimal(12, 2)")]
        public decimal? Monto { get; set; }

        [Column("descripcion")]
        public string? Descripcion { get; set; }

        [Column("fecha_solicitud")]
        public DateTime? FechaSolicitud { get; set; }

        [Column("fecha_limite_rendicion")]
        public DateTime? FechaLimiteRendicion { get; set; }

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