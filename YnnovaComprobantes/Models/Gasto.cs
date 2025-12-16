using System.ComponentModel.DataAnnotations.Schema;

namespace YnnovaComprobantes.Models
{
    [Table("Gasto")]
    public class Gasto
    {
        public int Id { get; set; }
        public DateOnly Fecha { get; set; }
        public decimal Importe { get; set; }
        public string Descripcion {  get; set; }
        [Column("empresa_id")]
        public int EmpresaId { get; set; }
        [Column("banco_id")]
        public int BancoId { get; set; }
        [Column("tipo_rendicion_id")]
        public int TipoRendicionId { get; set; }
        [Column("usuario_id")]
        public int UsuarioId { get; set; }
        [Column("tipo_gasto_id")]
        public int TipoGastoId { get; set; }
        [Column("estado_id")]
        public int EstadoId { get; set; }
        [Column("fecha_registro")]
        public DateTime FechaRegistro { get; set; }
    }
}
