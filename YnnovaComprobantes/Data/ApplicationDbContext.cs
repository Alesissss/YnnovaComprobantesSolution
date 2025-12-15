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
    }
}
