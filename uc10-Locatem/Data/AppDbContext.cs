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

        public DbSet<Usuario> Usuario { get; set; } // Cria uma tabela chamada Usuario baseada na classe Usuario
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Documento)
                .IsUnique();
        }


        public DbSet<Endereco> Endereco { get; set; } // Cria uma tabela chamada Endereco baseada na classe Endereco

        public DbSet<Aluguel> Alugueis { get; set; } // Cria uma tabela chamada Alugueis baseada na classe Aluguel

        public DbSet<UsuarioPerfil> UsuarioPerfis { get; set; } // Cria uma tabela chamada UsuarioPerfis baseada na classe UsuarioPerfil

        
        public DbSet<Ferramenta> Ferramenta { get; set; }

        // O método "OnModelCreating" é usado para configurar o modelo de dados. Ele é chamado quando o modelo é criado e pode ser usado para definir regras, restrições e outras configurações para as entidades.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Aluguel>()
                .Property(a => a.ValorTotal)
                .HasPrecision(10, 2);
        }

        public DbSet<Ferramenta> Ferramenta { get; set; }

        public DbSet<Categorias> Categorias { get; set; }

    }
}
