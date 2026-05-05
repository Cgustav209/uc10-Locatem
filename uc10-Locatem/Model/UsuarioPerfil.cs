using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace uc10_Locatem.Model
{
    public class UsuarioPerfil
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public Usuario? Usuario { get; set; }

        public string? Telefone { get; set; }

        public string? TipoUsuario { get; set; } // Locador / Locatario

        public string? UrlFoto { get; set; }
    }
}