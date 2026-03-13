using uc10_Locatem.Enum;

namespace uc10_Locatem.Model
{
    public class Reserva
    {
        public int Id { get; set; }
        
        public int LocatarioId { get; set; }

        public int LocadorId { get; set; }

        public int ProdutolId { get; set; }

        public DateTime DataInicio { get; set; }

        public DateTime DataFim { get; set; }

        public StatusReserva Status { get; set; }

        public DateTime DataCriacao { get; set; }

        public Usuario Locatario { get; set; } = null!;

        public Usuario Locador { get; set; } = null!;
    }
}
