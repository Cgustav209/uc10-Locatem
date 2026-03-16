using System.ComponentModel.DataAnnotations;
using uc10_Locatem.API.Model;
using uc10_Locatem.Enum;

namespace uc10_Locatem.Model
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Nome é um valor obrigatorio")]
        [StringLength(100, ErrorMessage = "O nome pode conter até 100 caracteres")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email é um campo obrigatorio")]
        [EmailAddress(ErrorMessage ="Email invalido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Senha é um campo obrigatorio")]
        public string Senha {  get; set; } = string.Empty;

        public int Hash{ get; set; }

        [Required(ErrorMessage = "Telefone é um campo obrigatorio")]
        public string Telefone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Escolha um Tipo")]
        public string Tipo { get; set; } = string.Empty ;

        [Required]
        public TipoUsuario TipoUsuario { get; set; }

        public string? FotoPerfilUrl { get; set; }

        [Required(ErrorMessage ="Documento é obrigatorio")]
        [StringLength(14, ErrorMessage ="Documento deve conter 14 caracteres no formato xxx.xxx.xxx-xx")]
        //[RegularExpression(@"^\d{3}\.\d{3}\.\d{3}-\d{2}$", ErrorMessage ="Documento deve estar no formato xxx.xxx.xxx-xx")]
        public string Documento { get; set; } = string.Empty ;

        public DateTime DataCadastro { get; set; }

        public bool Ativo {  get; set; }


        public List<Endereco> Enderecos { get; set; } = [];

        public Usuario()
        {
            DataCadastro = DateTime.UtcNow;
            Ativo = true;
        }

        //
    }
    // exemplo de como usar Enum, enum faz algo parecido com o bool, o bool da duas opcoes true/false, agora enum pode conter mais do que dois
    //enum TipoUsuario
    //{
    //    Locatario = 0,
    //    Locador = 1,
    //}

}
