using Microsoft.EntityFrameworkCore;
using uc10_Locatem.Data;
using uc10_Locatem.Enum;
using uc10_Locatem.Model;

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



        //Aceitar reserva - Somente o dono da ferramenta pode aceitar a reserva 
        public async Task<(bool sucesso, string mensagem)> AceitarReserva(int reservaId, int usuarioId)
        {
            // Carregar a reserva junto com o produto relacionado para verificar o dono da ferramenta
            var reserva = await _context.Reserva.Include(r => r.Ferramenta).FirstOrDefaultAsync(r => r.Id == reservaId);

            // Verificar se a reserva existe
            if (reserva == null)
            {
                return (false, "Reserva não encontrada.");
            }
            // Verificar se o usuário é o dono da ferramenta
            var ferramenta = await _context.Ferramenta.FindAsync(reserva.FerramentaId);
            if (ferramenta.UsuarioId != usuarioId)
            {
                return (false, "Você não tem permissão para aceitar esta reserva.");
            }
            // Verificar se a reserva já foi aceita ou recusada
            if (reserva.Status != StatusReserva.Pendente)
            {
                return (false, "A reserva já foi processada e não pode ser aceita.");
            }
            reserva.Status = StatusReserva.Aceita;
            await _context.SaveChangesAsync();
            return (true, "Reserva aceita com sucesso.");
        }



        //Recusar reserva - apenas o dono da ferramenta pode recusar a reserva, e somente se a reserva estiver pendente
        public async Task<(bool sucesso, string mensagem)> RecusarReserva(int reservaId, int usuarioId)
        {
            // Carregar a reserva junto com o produto relacionado para verificar o dono da ferramenta
            var reserva = await _context.Reserva.Include(r => r.Ferramenta).FirstOrDefaultAsync(r => r.Id == reservaId);

            // Verificar se a reserva existe
            if (reserva == null)
            {
                return (false, "Reserva não encontrada.");
            }
            // Verificar se o usuário é o dono da ferramenta
            if (reserva.Ferramenta.UsuarioId != usuarioId)
            {
                return (false, "Você não tem permissão para recusar esta reserva.");
            }
            // Verificar se a reserva já foi aceita ou recusada
            if (reserva.Status != StatusReserva.Pendente)
            {
                return (false, "A reserva já foi processada e não pode ser recusada.");
            }

            // Atualizar o status da reserva para recusada
            reserva.Status = StatusReserva.Recusada;

            await _context.SaveChangesAsync();
            return (true, "Reserva recusada com sucesso.");
        }

        public async Task<List<Reserva>> ListarReservasDoUsuario(int usuarioId)
        {
            // Carregar as reservas do usuário junto com os produtos relacionados para exibir detalhes da ferramenta
            return await _context.Reserva.Include(r => r.Ferramenta).Where(r => r.UsuarioId == usuarioId).ToListAsync();
        }

        public async Task<List<Reserva>> ListarResrvasRecebidas(int usuarioId)
        {
            return await _context.Reserva.Include(r => r.Usuario).Where(r => r.Ferramenta.UsuarioId == usuarioId).ToListAsync();
        }
    }
}
