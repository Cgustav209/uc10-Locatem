using uc10_Locatem.Data;
using uc10_Locatem.Enum;

namespace uc10_Locatem.Services
{
    public class ReservaService
    {
        private readonly AppDbContext _context;

        public ReservaService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(bool sucesso, string mensagem)> CancelarReserva(int reservaId, int usuarioId)
        {
            var reserva = await _context.Reserva.FindAsync(reservaId);

            // Verificar se a reserva existe
            if (reserva == null)
            {
                return (false, "Reserva não encontrada.");
            }

            // Verificar se o usuário é o dono da reserva
            if (reserva.UsuarioId != usuarioId)
            {
                return (false, "Você não tem permissão para cancelar esta reserva.");
            }

            // Verificar se a reserva já está cancelada
            if (reserva.Status == StatusReserva.Cancelada)
            {
                return (false, "A reserva já está cancelada.");
            }

            // Verificar se a reserva já foi aceita
            if (reserva.Status == StatusReserva.Recusada)
            {
                return (false, "A reserva já foi recusada e não pode ser cancelada.");
            }

            reserva.Status = StatusReserva.Cancelada;
            await _context.SaveChangesAsync();
            return (true, "Reserva cancelada com sucesso.");


        }
    }
}
