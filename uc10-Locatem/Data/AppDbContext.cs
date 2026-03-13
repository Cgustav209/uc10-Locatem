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

        public DbSet<Endereco> Endereco { get; set; } // Cria uma tabela chamada Endereco baseada na classe Endereco

        public DbSet<Aluguel> Alugueis { get; set; } // Cria uma tabela chamada Alugueis baseada na classe Aluguel

    }
}
