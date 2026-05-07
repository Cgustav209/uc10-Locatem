using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace uc10_Locatem.Model
{
    public class UsuarioPerfil
    {
        [Key]
        public int Id { get; set; }

        public int UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public Usuario Usuario { get; set; } = null!;

        public string? Telefone { get; set; }

        public string? TipoUsuario { get; set; }

        public string? UrlFoto { get; set; }
    }
}