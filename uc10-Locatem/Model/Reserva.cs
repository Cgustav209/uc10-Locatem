using System.ComponentModel.DataAnnotations.Schema;
using uc10_Locatem.Enum;


namespace uc10_Locatem.Model
{
    public class Reserva
    {
        public int Id { get; set; }
                      
        public int FerramentaId { get; set; } // Aqui vou ter que trocar para FerramentaId

        public DateTime DataInicio { get; set; }

        public DateTime DataFim { get; set; }

        public StatusReserva Status { get; set; }

        public DateTime DataCriacao { get; set; }

        public int UsuarioId { get; set; }

        [ForeignKey(nameof(UsuarioId))]
        public Usuario Usuario { get; set; } = null!;

        [ForeignKey(nameof(FerramentaId))]
        public Ferramenta Ferramenta { get; set; } = null!;

    }
}
