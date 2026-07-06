using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace uc10_Locatem.Model
{
    public class ChatMensagem
    {
        public int Id { get; set; }

        [Required]
        public int ConversaId { get; set; }

        [Required]
        public int RemetenteId { get; set; }

        [Required]
        [StringLength(1000)]
        public string Conteudo { get; set; } = string.Empty;

        // RN004
        public DateTime DataEnvio { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        public ChatConversa Conversa { get; set; }
    }
}