using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace uc10_Locatem.Model

    //desenvolvida para teste 
{
    public class Ferramenta
    {

        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = default!;       // ou Nome
        public string Slug { get; set; } = default!;       // novo
        public int CategoriaId { get; set; }              // novo
        public Categoria Categoria { get; set; } = default!; // novo

        public string? Brand { get; set; }                 // opcional (Marca)
        public decimal PricePerDay { get; set; }           // opcional (PrecoDiaria)
        public decimal? PricePerWeek { get; set; }         // opcional (PrecoSemanal)
        public int Stock { get; set; }                     // opcional (Estoque)
        public decimal? RatingAvg { get; set; }            // opcional (NotaMedia)
        public int RatingCount { get; set; }               // opcional (QtdAvaliacoes)
        public bool IsActive { get; set; } = true;         // opcional (Ativo)
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // opcional (RegistroCriacao)
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow; // opcional (RegistroAtualizacao)



    }
}