using System.ComponentModel.DataAnnotations;
using uc10_Locatem.Enum;

namespace uc10_Locatem.API.Model.DTO
{
    public class CriarEnderecoDTO
    {
        [Required(ErrorMessage = "Logradouro é obrigatório.")]
        public string Logradouro { get; set; } = string.Empty;

        [Required(ErrorMessage = "Número é obrigatório.")]
        public string Numero { get; set; } = string.Empty;

        public string Complemento { get; set; } = string.Empty;

        [Required(ErrorMessage = "Bairro é obrigatório.")]
        public string Bairro { get; set; } = string.Empty;

        [Required(ErrorMessage = "Cidade é obrigatória.")]
        public string Cidade { get; set; } = string.Empty;

        [Required(ErrorMessage = "Estado é obrigatório.")]
        public string Estado { get; set; } = string.Empty;

        [Required(ErrorMessage = "CEP é obrigatório.")]
        public string CEP { get; set; } = string.Empty;

        public TipoEndereco TipoEndereco { get; set; }

        
         // Deve ser public para que o Scalar consiga enviar true ou false no JSON. 
         
        public bool EhPrioritario { get; set; }
    }
}