using System.ComponentModel.DataAnnotations;
using uc10_Locatem.Enum;
namespace uc10_Locatem.Model.DTO
{
    public class CriarUsuarioDTO
    {
        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(100, MinimumLength = 3)]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "A senha é obrigatória")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "A senha deve ter entre 6 e 100 caracteres")]
        public string Senha { get; set; } = string.Empty;

        [Required(ErrorMessage = "A senha é obrigatória")]
        [Compare("Senha", ErrorMessage = "As senhas não conferem")]
        public string ConfirmarSenha { get; set; } = string.Empty;

        public string Tipo { get; set; } = string.Empty;

        public string Telefone { get; set; } = string.Empty;

        public string Documeto { get; set; } = string.Empty;

        public TipoUsuario TipoUsuario { get; set; }

    }
}
