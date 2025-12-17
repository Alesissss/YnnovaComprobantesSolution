namespace YnnovaComprobantes.ViewModels
{
    public class GastoViewModel
    {
        public int Id { get; set; }
        public DateOnly Fecha { get; set; }
        public string? Empresa { get; set; }
        public string? Banco { get; set; }
        public string? TipoRendicion { get; set; }
        public string Usuario { get; set; }
        public string TipoGasto { get; set; }
        public string? MonedaNombre { get; set; }
        public string? MonedaSimbolo { get; set; }
        public string Estado { get; set; }
        public decimal? Importe { get; set; }
        public string? Descripcion { get; set; }
        public int EmpresaId { get; set; }
        public int? BancoId { get; set; }
        public int? TipoRendicionId { get; set; }
        public int UsuarioId { get; set; }
        public int TipoGastoId { get; set; }
        public int? MonedaId { get; set; }
        public int EstadoId { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}
