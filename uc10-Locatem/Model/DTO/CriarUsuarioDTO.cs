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

        //public string Tipo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Telefone é obrigatório")]
        [StringLength(11, MinimumLength = 10, ErrorMessage = "Telefone inválido")]
        public required string Telefone { get; set; }

        [Required(ErrorMessage = "Documento é obrigatório")]
        [StringLength(14, MinimumLength = 11, ErrorMessage = "Documento inválido")]
        public required string Documento { get; set; }

        public TipoUsuario TipoUsuario { get; set; }

        [Required(ErrorMessage = "CEP é obrigatório")]
        [StringLength(9, MinimumLength = 8, ErrorMessage = "CEP inválido")]
        public string Cep { get; set; } = string.Empty;

        [Required(ErrorMessage = "Logradouro é obrigatório")]
        public string Logradouro { get; set; } = string.Empty;

        [Required(ErrorMessage = "Número é obrigatório")]
        public string Numero { get; set; } = string.Empty;

        public string Complemento { get; set; } = string.Empty;

        [Required(ErrorMessage = "Bairro é obrigatório")]
        public string Bairro { get; set; } = string.Empty;

        [Required(ErrorMessage = "Cidade é obrigatória")]
        public string Cidade { get; set; } = string.Empty;

        [Required(ErrorMessage = "Estado é obrigatório")]
        [StringLength(2, MinimumLength = 2)]
        public string Estado { get; set; } = string.Empty;

    }
}
