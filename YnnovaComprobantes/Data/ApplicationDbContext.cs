using Microsoft.EntityFrameworkCore;
using YnnovaComprobantes.Models;

namespace YnnovaComprobantes.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }
        public DbSet<Empresa> Empresas { get; set; }
        public DbSet<TipoComprobante> TipoComprobantes { get; set; }
        public DbSet<Comprobante> Comprobantes { get; set; }
        public DbSet<Concepto> Conceptos { get; set; }
        public DbSet<Banco> Bancos { get; set; }
        public DbSet<Moneda> Monedas { get; set; }
        public DbSet<TipoUsuario> TipoUsuarios { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<TipoRendicion> TipoRendiciones { get; set; }
        public DbSet<EmpresaUsuario> EmpresasUsuarios { get; set; }
        public DbSet<TipoGasto> TipoGastos { get; set; }
        public DbSet<Gasto> Gastos { get; set; }
        public DbSet<Estado> Estados { get; set; }
        public DbSet<Observacion> Observaciones { get; set; }
        public DbSet<DevolucionGasto> DevolucionGastos { get; set; }
    }
}
