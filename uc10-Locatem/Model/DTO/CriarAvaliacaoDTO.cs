
using uc10_Locatem.Enum;

namespace uc10_Locatem.Model.DTO

{
    public class CriarAvaliacaoDTO
    {
        public int AluguelId { get; set; }

        public TipoAvaliacao TipoAvaliacao { get; set; }

        public int Nota { get; set; }

        public string? Comentario { get; set; }

        public int? AvaliadoUsuarioId { get; set; }

        public int? FerramentaId { get; set; }
    }
}
