using System.ComponentModel.DataAnnotations;

namespace uc10_Locatem.Model.DTO
{
    public class UpdateUsuarioDTO
    {
        public int Id { get; set; }
       
        public string? Nome { get; set; }

        [RegularExpression(@"^\(\d{2}\)\s?\d{4,5}-\d{4}$",
            ErrorMessage = "Telefone deve estar no formato (11) 99999-9999")]
        public string? Telefone { get; set; }

        [EmailAddress(ErrorMessage = "Email inválido")]
        public string? Email { get; set; }

    }
}
