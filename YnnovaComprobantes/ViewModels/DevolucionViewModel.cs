namespace YnnovaComprobantes.ViewModels
{
    public class DevolucionViewModel
    {
        public int Id { get; set; }
        public int GastoId { get; set; }
        public DateOnly Fecha { get; set; }
        public decimal? Importe { get; set; }
        public string? Descripcion { get; set; }
        public int EmpresaId { get; set; }
        public string? Empresa { get; set; }
        public int UsuarioId { get; set; }
        public string? Usuario { get; set; }
        public string NroOperacion { get; set; }
        public int? BancoId { get; set; }
        public string? Banco { get; set; }
        public int EstadoId { get; set; }
        public string? Estado { get; set; }
        public int? UsuarioAprobador { get; set; }
        public string? UsuarioAprobadorNombre { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}
