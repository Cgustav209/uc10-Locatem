using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace uc10_Locatem.Model
{
    public class FerramentaImagem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int FerramentaId { get; set; }

        [ForeignKey(nameof(FerramentaId))]
        [JsonIgnore]
        public Ferramenta Ferramenta { get; set; } = null!;

        [Required]
        public string UrlImagem { get; set; } = string.Empty;
    }
}