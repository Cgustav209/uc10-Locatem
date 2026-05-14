using uc10_Locatem.Enum;

namespace uc10_Locatem.Model
{
    public class Avaliacao
    {
        public int Id { get; set; }

        public int AluguelId { get; set; }
        public Aluguel Aluguel { get; set; } = null!;

        // Quem avaliou
        public int AvaliadorId { get; set; }
        public Usuario Avaliador { get; set; } = null!;

        // Tipo da avaliação
        public TipoAvaliacao Tipo { get; set; }

        // Avaliação de usuário
        public int? AvaliadoUsuarioId { get; set; }
        public Usuario? AvaliadoUsuario { get; set; }

        // Avaliação de ferramenta
        public int? FerramentaId { get; set; }
        public Ferramenta? Ferramenta { get; set; }

        // Nota
        public int Nota { get; set; }

        public string? Comentario { get; set; }

        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    }

}
