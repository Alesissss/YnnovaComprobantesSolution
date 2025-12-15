using System.ComponentModel.DataAnnotations.Schema;

namespace YnnovaComprobantes.Models
{
    [Table("Tipo_Comprobante")]
    public class TipoComprobante
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Descripcion {  get; set; }
        public bool Estado { get; set; }
    }
}
