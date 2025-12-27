using YnnovaComprobantes.Models;

namespace YnnovaComprobantes.ViewModels
{
    public class EditarAnticipoViewModel
    {
        // Para Card 1 (Edición del Anticipo)
        public Anticipo? Anticipo { get; set; }
        public Estado? Estado { get; set; }

        // Listas para los combos de Card 1 (Anticipo) y Card 2 (Comprobantes)
        public List<Empresa>? Empresas { get; set; }       // NUEVO
        public List<Usuario>? Usuarios { get; set; }       // NUEVO
        public List<Banco>? Bancos { get; set; }           // NUEVO
        public List<TipoRendicion>? TiposRendicion { get; set; } // NUEVO

        // Compartido (Anticipo y Comprobante)
        public List<Moneda>? Monedas { get; set; }

        // Para Card 2 (Nuevo Comprobante)
        public List<TipoComprobante>? TiposComprobante { get; set; }
        public List<Concepto>? Conceptos { get; set; }

        // Para Card 3 y 4
        public List<Comprobante>? ComprobantesRegistrados { get; set; }
        public List<Observacion>? Observaciones { get; set; }
        public int UsuarioLogueadoId { get; set; }
    }
}