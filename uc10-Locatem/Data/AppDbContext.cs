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

        public DbSet<Ferramenta> Ferramenta { get; set; } 

        public DbSet<Reserva> Reserva { get; set; }

        public DbSet<Categoria> Categorias { get; set; }

        public DbSet<FerramentaImagem> FerramentaImagens { get; set; }

        public DbSet<BloqueioDisponibilidade> BloqueioDisponibilidade { get; set; }
        public DbSet<Avaliacao> Avaliacoes { get; set; }

        public DbSet<ChatConversa> ChatConversas { get; set; }
        public DbSet<ChatMensagem> ChatMensagens { get; set; }

        public DbSet<Favorito> Favoritos { get; set; }


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

            modelBuilder.Entity<Ferramenta>()
                .HasOne(f => f.Endereco)
                .WithMany()
                .HasForeignKey(f => f.EnderecoId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Avaliacao>()
                .HasOne(a => a.Avaliador)
                .WithMany()
                .HasForeignKey(a => a.AvaliadorId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Avaliacao>()
                .HasOne(a => a.AvaliadoUsuario)
                .WithMany()
                .HasForeignKey(a => a.AvaliadoUsuarioId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Reserva>()
                .HasOne(r => r.Usuario)
                .WithMany()
                .HasForeignKey(r => r.UsuarioId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Reserva>()
                .HasOne(r => r.Ferramenta)
                .WithMany()
                .HasForeignKey(r => r.FerramentaId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Aluguel>()
                .HasOne(a => a.Usuario)
                .WithMany()
                .HasForeignKey(a => a.UsuarioId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Aluguel>()
                .HasOne(a => a.Ferramenta)
                .WithMany()
                .HasForeignKey(a => a.FerramentaId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Endereco>()
                .HasOne(e => e.Usuario)
                .WithMany(u => u.Enderecos)
                .HasForeignKey(e => e.UsuarioId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<FerramentaImagem>()
                .HasOne(fi => fi.Ferramenta)
                .WithMany(f => f.Imagens)
                .HasForeignKey(fi => fi.FerramentaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Favorito>()
                .HasIndex(f => new { f.UsuarioId, f.FerramentaId })
                .IsUnique();

            modelBuilder.Entity<Favorito>()
                .HasOne(f => f.Usuario)
                .WithMany()
                .HasForeignKey(f => f.UsuarioId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Favorito>()
                .HasOne(f => f.Ferramenta)
                .WithMany()
                .HasForeignKey(f => f.FerramentaId)
                .OnDelete(DeleteBehavior.NoAction);


            // =============================
            // CATEGORIAS PADRÃO DO LOCATEM
            // =============================
            modelBuilder.Entity<Categoria>().HasData(
                new Categoria
                {
                    Id = 1,
                    nome = "Ferramentas Elétricas • Parafusadeira/Furadeira",
                    CategoriaPaiId = null,
                    EhPadrao = true
                },
                new Categoria
                {
                    Id = 2,
                    nome = "Ferramentas Elétricas • Corte e Desgaste",
                    CategoriaPaiId = null,
                    EhPadrao = true
                },
                new Categoria
                {
                    Id = 3,
                    nome = "Ferramentas Elétricas • Pintura",
                    CategoriaPaiId = null,
                    EhPadrao = true
                },
                new Categoria
                {
                    Id = 4,
                    nome = "Ferramentas Manuais",
                    CategoriaPaiId = null,
                    EhPadrao = true
                },
                new Categoria
                {
                    Id = 5,
                    nome = "Jardinagem e Paisagismo",
                    CategoriaPaiId = null,
                    EhPadrao = true
                },
                new Categoria
                {
                    Id = 6,
                    nome = "Construção e Alvenaria",
                    CategoriaPaiId = null,
                    EhPadrao = true
                },
                new Categoria
                {
                    Id = 7,
                    nome = "Elevação e Transporte",
                    CategoriaPaiId = null,
                    EhPadrao = true
                },
                new Categoria
                {
                    Id = 8,
                    nome = "Limpeza e Lavagem",
                    CategoriaPaiId = null,
                    EhPadrao = true
                }
            );


        }
    }
}
