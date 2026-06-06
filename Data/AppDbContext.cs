using Microsoft.EntityFrameworkCore;
using ViziLogin.Models;

namespace ViziLogin.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Servico> Servicos { get; set; }
        public DbSet<Avaliacao> Avaliacao { get; set; }
        public DbSet<Denuncia> Denuncias { get; set; }
        public DbSet<Notificacao> Notificacoes { get; set; }
        public DbSet<Area> Areas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Area>().HasData(
                new Area { Id_Area = 1, Nome = "Barreiro" },
                new Area { Id_Area = 2, Nome = "Centro-Sul" },
                new Area { Id_Area = 3, Nome = "Leste" },
                new Area { Id_Area = 4, Nome = "Nordeste" },
                new Area { Id_Area = 5, Nome = "Noroeste" },
                new Area { Id_Area = 6, Nome = "Norte" },
                new Area { Id_Area = 7, Nome = "Oeste" },
                new Area { Id_Area = 8, Nome = "Pampulha" },
                new Area { Id_Area = 9, Nome = "Venda Nova" }
            );
        }
    }
}