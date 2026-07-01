using System.ComponentModel.DataAnnotations;

namespace uc10_Locatem.Model.DTO
{
    public class EnviarMensagemDTO
    {
        [Required]
        [StringLength(1000)]
        public string Conteudo { get; set; } = string.Empty;
    }
}