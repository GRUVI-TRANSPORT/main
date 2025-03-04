using ApiGruvi.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiGruvi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Usuarios> Usuarios { get; set; }
        public DbSet<Viaje> Viajes { get; set; }
        public DbSet<Destino> Destinos { get; set; }
        public DbSet<Opinion> Opiniones { get; set; }
        public DbSet<Boleto> Boletos { get; set; }
        public DbSet<Pago> Pagos { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Viaje>()
                .HasOne(v => v.Destino_Navigation)
                .WithMany()
                .HasForeignKey(v => v.Destino_Id);

            base.OnModelCreating(modelBuilder);
        }
    }
}
