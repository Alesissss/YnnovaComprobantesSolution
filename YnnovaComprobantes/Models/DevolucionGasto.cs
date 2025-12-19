using System.ComponentModel.DataAnnotations.Schema;

namespace YnnovaComprobantes.Models
{
    [Table("Devolucion_gasto")]
    public class DevolucionGasto
    {
        public int Id { get; set; }
        [Column("gasto_id")]
        public int GastoId { get; set; }
        public DateOnly Fecha { get; set; }
        public decimal? Importe { get; set; }
        public string? Descripcion { get; set; }
        [Column("empresa_id")]
        public int EmpresaId { get; set; }
        [Column("usuario_id")]
        public int UsuarioId { get; set; }
        [Column("nro_operacion")]
        public string NroOperacion { get; set; }
        [Column("banco_id")]
        public int? BancoId { get; set; }
        [Column("estado_id")]
        public int EstadoId { get; set; }
        [Column("usuario_aprobador")]
        public int? UsuarioAprobador { get; set; }
        [Column("fecha_registro")]
        public DateTime FechaRegistro { get; set; }
    }
}
