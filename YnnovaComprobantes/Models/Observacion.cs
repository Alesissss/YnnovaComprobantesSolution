using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YnnovaComprobantes.Models
{
    [Table("observacion")]
    public class Observacion
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("anticipo_id")]
        public int? AnticipoId { get; set; } // Nuevo FK

        [Column("reembolso_id")]
        public int? ReembolsoId { get; set; } // Nuevo FK

        [Column("planilla_movilidad_id")]
        public int? PlanillaMovilidadId { get; set; } // Nuevo FK

        [Column("usuario_id")]
        public int? UsuarioId { get; set; }

        [Column("prioridad")]
        public string? Prioridad { get; set; } // "A", "M", "B"

        [Column("mensaje")]
        public string? Mensaje { get; set; }

        [Column("fecha_creacion")]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        // Propiedad extra para mostrar el nombre en el chat (no mapeada a BD si usas DTO, 
        // pero si usas el modelo directo, la llenaremos con LINQ)
        [NotMapped]
        public string? NombreUsuario { get; set; }
    }
}