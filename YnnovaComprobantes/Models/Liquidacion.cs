using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YnnovaComprobantes.Models
{
    [Table("liquidacion")]
    public class Liquidacion
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("codigo_generado")]
        public string? CodigoGenerado { get; set; }

        [Column("empresa_id")]
        public int EmpresaId { get; set; }

        [Column("usuario_id")]
        public int UsuarioId { get; set; }

        [Column("fecha_inicio")]
        public DateTime FechaInicio { get; set; } = DateTime.Now;

        [Column("fecha_cierre")]
        public DateTime? FechaCierre { get; set; }

        [Column("descripcion")]
        public string? Descripcion { get; set; }

        // --- RESUMEN FINANCIERO ---
        [Column("total_anticibido")] // Mapeado exacto a tu script SQL
        public decimal TotalAnticipado { get; set; }

        [Column("total_gastado")]
        public decimal TotalGastado { get; set; }

        [Column("saldo_final")]
        public decimal SaldoFinal { get; set; }

        [Column("estado_id")]
        public int EstadoId { get; set; }

        [Column("usuario_registro")]
        public int? UsuarioRegistro { get; set; }

        [Column("fecha_registro")]
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
    }
}