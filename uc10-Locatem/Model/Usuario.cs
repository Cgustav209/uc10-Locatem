using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using uc10_Locatem.API.Model;
using uc10_Locatem.Enum;
using uc10_Locatem.Model.DTO;

namespace uc10_Locatem.Model
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é um campo obrigatório")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "O nome deve ter entre 2 e 100 caracteres")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O email é um campo obrigatório")]
        [EmailAddress(ErrorMessage = "O formato do email é inválido")]
        [StringLength(150, ErrorMessage = "O email deve ter no máximo 150 caracteres")]
        public string Email { get; set; } = string.Empty;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [Required(ErrorMessage = "A senha é obrigatória")]
        [StringLength(255, ErrorMessage = "A senha deve ter no máximo 255 caracteres")]

        public string Senha {  get; set; } = string.Empty;

        public int Hash { get; set; }

        [Required(ErrorMessage = "Telefone é obrigatório")]
        [RegularExpression(@"^\(\d{2}\)\s?\d{4,5}-\d{4}$",
        ErrorMessage = "Telefone deve estar no formato (11) 99999-9999")]
        public string Telefone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Escolha um Tipo")]
        public TipoUsuario TipoUsuario { get; set; } 


        [Required(ErrorMessage = "Documento é obrigatorio")]
        [StringLength(14, ErrorMessage = "Documento deve conter 14 caracteres no formato xxx.xxx.xxx-xx")]
        //[RegularExpression(@"^\d{3}\.\d{3}\.\d{3}-\d{2}$", ErrorMessage ="Documento deve estar no formato xxx.xxx.xxx-xx")]
        public string Documento { get; set; } = string.Empty;

        [JsonIgnore]
        public DateTime DataCadastro { get; set; }
        [JsonIgnore]
        public bool Ativo { get; set; }

        public List<Endereco> Enderecos { get; set; } = new List<Endereco>();
        //  public List<Endereco> Enderecos { get; set; } = [];

        public Usuario()
        {
            DataCadastro = DateTime.UtcNow;
            Ativo = true;
        }

        // bloqueio de usuário inadimplente
        public bool Bloqueado { get; set; }
    }
    // exemplo de como usar Enum, enum faz algo parecido com o bool, o bool da duas opcoes true/false, agora enum pode conter mais do que dois
    //enum TipoUsuario
    //{
    //    Locatario = 0,
    //    Locador = 1,
    //}

}
