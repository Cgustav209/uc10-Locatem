using Microsoft.EntityFrameworkCore;
using uc10_Locatem.API.Model;
using uc10_Locatem.Model;

namespace uc10_Locatem.Data
{
    // Aqui entra o Entity Framework Core.
    // O "AppDbContext.cs" funciona como um tradutor
    // C# → SQL
    // SQL → C#

    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        { 
        }

        // Cria as tabelas no banco de dados com base em suas respectivas classes
        public DbSet<Usuario> Usuario { get; set; } 
        public DbSet<Endereco> Endereco { get; set; } 

        public DbSet<Aluguel> Alugueis { get; set; } 

        public DbSet<UsuarioPerfil> UsuarioPerfis { get; set; } 

        public DbSet<Ferramenta> Ferramenta { get; set; } 

        public DbSet<Reserva> Reserva { get; set; }

        public DbSet<Categorias> Categorias { get; set; }

        public DbSet<FerramentaImagem> FerramentaImagens { get; set; }

        public DbSet<BloqueioDisponibilidade> BloqueioDisponibilidade { get; set; }


        // O método "OnModelCreating" é usado para configurar o modelo de dados. Ele é chamado quando o modelo é criado e pode ser usado para definir regras, restrições e outras configurações para as entidades.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Documento)
                .IsUnique();

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Aluguel>()
                .Property(a => a.ValorTotal)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Aluguel>()
                .Property(a => a.ValorCaucao)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Aluguel>()
                 .Property(a => a.ValorDevolvido)
                 .HasPrecision(10, 2);

            modelBuilder.Entity<Ferramenta>()
                .Property(f => f.Caucao)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Ferramenta>()
                .Property(f => f.Diaria)
                .HasPrecision(10, 2);
        }
    }
}