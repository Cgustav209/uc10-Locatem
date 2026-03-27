using System.ComponentModel.DataAnnotations;

namespace uc10_Locatem.Model
{
    public class BloqueioDisponibilidade
    {
        [Key]
        public int Id { get; set; }


        [Required(ErrorMessage = "O ID da ferramenta é obrigatório.")]
        public int FerramentaId { get; set; }


        [Required(ErrorMessage = "A data de início é obrigatória.")]
        public DateTime DataInicio { get; set; }


        [Required(ErrorMessage = "A data de fim é obrigatória.")]
        public DateTime DataFim { get; set; }


        [StringLength(200, ErrorMessage = "O motivo pode ter no máximo 200 caracteres.")]
        public string? Motivo { get; set; }


        public bool Ativo { get; set; } = true;
    }
}
