using System.ComponentModel.DataAnnotations;

namespace uc10_Locatem.Model.DTO
{
    public class AtualizarUsuarioDTO
    {
        [Required(ErrorMessage = "Nome é obrigatório")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Telefone é obrigatório")]
        [StringLength(11, MinimumLength = 10)]
        public string Telefone { get; set; } = string.Empty;
    }
}