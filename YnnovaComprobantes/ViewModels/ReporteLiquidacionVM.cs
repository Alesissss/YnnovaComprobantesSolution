namespace YnnovaComprobantes.Models
{
    public class ReporteLiquidacionVM
    {
        // Cabecera
        public string Codigo { get; set; }
        public string FechaInicio { get; set; }
        public string Estado { get; set; }
        public string Empresa { get; set; }
        public string RucEmpresa { get; set; }
        public string EmpleadoNombre { get; set; }
        public string EmpleadoNumeroCuenta { get; set; }
        public string EmpleadoDni { get; set; }

        // Totales Generales (Tal cual se guardan en BD)
        public decimal TotalRecibido { get; set; }
        public decimal TotalGastado { get; set; }
        public decimal Saldo { get; set; }

        // Listas Detalladas
        public List<ReporteAnticipoVM> Anticipos { get; set; } = new List<ReporteAnticipoVM>();
        public List<ReporteComprobanteVM> Comprobantes { get; set; } = new List<ReporteComprobanteVM>(); // Renombrado a Comprobantes
        public List<ReportePlanillaVM> PlanillaItems { get; set; } = new List<ReportePlanillaVM>();
        public ReporteReembolsoVM Cierre { get; set; }
    }

    public class ReporteAnticipoVM
    {
        public string Fecha { get; set; }
        public string FechaLimite { get; set; }
        public string TipoRendicion { get; set; } // Viáticos, etc.
        public string Banco { get; set; }
        public string MonedaSimbolo { get; set; } // S/. o $
        public decimal Monto { get; set; }
        public string Estado { get; set; }
        public string NumeroOperacion { get; set; }
        public string AprobadoPor { get; set; }
    }

    public class ReporteComprobanteVM
    {
        public string Fecha { get; set; }
        public string TipoComprobante { get; set; } // Factura, Boleta...
        public string Proveedor { get; set; }
        public string SerieNumero { get; set; }
        public string Concepto { get; set; } // Hospedaje, Alimentos...
        public string MonedaSimbolo { get; set; }
        public decimal Monto { get; set; }
        public string Estado { get; set; }
        public string AprobadoPor { get; set; }
    }

    public class ReportePlanillaVM
    {
        public string Fecha { get; set; }
        public string Motivo { get; set; }
        public string Ruta { get; set; }
        public decimal Monto { get; set; } // Siempre S/.
        public string Estado { get; set; }
    }

    public class ReporteReembolsoVM
    {
        public string Tipo { get; set; }
        public decimal Monto { get; set; }
        public string MonedaSimbolo { get; set; } // El reembolso también tiene moneda
        public string Fecha { get; set; }
    }
}