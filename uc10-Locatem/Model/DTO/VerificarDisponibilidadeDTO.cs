using System.ComponentModel.DataAnnotations;

namespace uc10_Locatem.Model.DTO
{
    public class VerificarDisponibilidadeDTO
    {
        [Required(ErrorMessage = "O ID da ferramenta é obrigatório.")]
        public int FerramentaId { get; set; }


        [Required(ErrorMessage = "A data de início é obrigatória.")]
        public DateTime DataInicio { get; set; }


        [Required(ErrorMessage = "A data de fim é obrigatória.")]
        public DateTime DataFim { get; set; }
    }
}
