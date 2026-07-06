using System.ComponentModel.DataAnnotations;

namespace uc10_Locatem.Model.DTO
{
    public class CriarConversaDTO
    {
        [Required]
        public int OutroUsuarioId { get; set; }

        public int? ReservaId { get; set; }

        public int? FerramentaId { get; set; }
    }
}
