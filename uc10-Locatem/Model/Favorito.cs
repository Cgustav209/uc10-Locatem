using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace uc10_Locatem.Model
{
    public class Favorito
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [Required]
        public int FerramentaId { get; set; }

        public DateTime DataFavorito { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(UsuarioId))]
        [JsonIgnore]
        public Usuario Usuario { get; set; } = null!;

        [ForeignKey(nameof(FerramentaId))]
        [JsonIgnore]
        public Ferramenta Ferramenta { get; set; } = null!;
    }
}
