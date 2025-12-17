namespace YnnovaComprobantes.ViewModels
{
    public class ComprobanteViewModel
    {
        public int Id { get; set; }
        public string? RucEmpresa { get; set; }
        public string? Serie { get; set; }
        public string? Numero { get; set; }
        public int TipoComprobanteId { get; set; }
        public int? ConceptoId { get; set; }
        public int MonedaId { get; set; }
        public decimal Monto { get; set; }
        public DateOnly Fecha { get; set; }
        public string? Estado { get; set; }
        public string? Descripcion { get; set; }
        public string? Archivo { get; set; }
        // Datos del gasto
        public int GId { get; set; }
        public int GUsuarioId { get; set; }
        public string? GTipoGasto { get; set; }
        public DateOnly GFecha { get; set; }
        public string? GMoneda { get; set; }
        public decimal? GImporte { get; set; }
        public decimal? MontoAcumulado { get; set; }
    }
}
