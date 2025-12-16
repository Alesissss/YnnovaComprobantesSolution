using System.ComponentModel.DataAnnotations.Schema;

namespace YnnovaComprobantes.Models
{
    [Table("Tipo_Gasto")]
    public class TipoGasto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }

        public bool Estado { get; set; }
    }
}
