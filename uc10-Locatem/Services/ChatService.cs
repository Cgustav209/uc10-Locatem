using Microsoft.EntityFrameworkCore;
using uc10_Locatem.Data;
using uc10_Locatem.Model;

namespace uc10_Locatem.Services
{
    public class ChatService
    {
        private readonly AppDbContext _context;

        public ChatService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ChatConversa> ObterOuCriarConversa(
            int usuario1Id,
            int usuario2Id,
            int? reservaId,
            int? ferramentaId)
        {
            if (usuario1Id == usuario2Id)
                throw new Exception("Não é possível criar conversa consigo mesmo");

            if (reservaId == null && ferramentaId == null)
                throw new Exception(
                    "A conversa deve estar vinculada a uma reserva ou ferramenta"
                );

            var usuario1Existe =
                await _context.Usuario.AnyAsync(x => x.Id == usuario1Id);

            var usuario2Existe =
                await _context.Usuario.AnyAsync(x => x.Id == usuario2Id);

            if (!usuario1Existe || !usuario2Existe)
                throw new Exception("Usuário inválido");

            if (reservaId != null)
            {
                var reservaExiste =
                    await _context.Reserva
                        .AnyAsync(r => r.Id == reservaId);

                if (!reservaExiste)
                    throw new Exception("Reserva não encontrada");
            }

            if (ferramentaId != null)
            {
                var ferramentaExiste =
                    await _context.Ferramenta
                        .AnyAsync(f => f.FerramentaId == ferramentaId);

                if (!ferramentaExiste)
                    throw new Exception("Ferramenta não encontrada");
            }

            var conversa = await _context.ChatConversas
                .FirstOrDefaultAsync(c =>
                    (
                        (c.Usuario1Id == usuario1Id &&
                         c.Usuario2Id == usuario2Id)
                        ||
                        (c.Usuario1Id == usuario2Id &&
                         c.Usuario2Id == usuario1Id)
                    )
                    &&
                    c.ReservaId == reservaId
                    &&
                    c.FerramentaId == ferramentaId
                );

            if (conversa != null)
                return conversa;
            
            conversa = new ChatConversa
            {
                Usuario1Id = usuario1Id,
                Usuario2Id = usuario2Id,
                ReservaId = reservaId,
                FerramentaId = ferramentaId,
                DataAtualizacao = DateTime.UtcNow
            };

            _context.ChatConversas.Add(conversa);

            await _context.SaveChangesAsync();

            return conversa;
        }

        public async Task<ChatConversa> ObterConversaPorId(
            int id,
            int usuarioId)
        {
            var conversa =
                await _context.ChatConversas.FindAsync(id);

            if (conversa == null)
                throw new Exception("Conversa não encontrada");

            ValidarParticipante(conversa, usuarioId);

            return conversa;
        }

        public async Task<List<ChatConversa>> ListarConversasUsuario(
            int usuarioId)
        {
            return await _context.ChatConversas
                .Where(c =>
                    c.Usuario1Id == usuarioId ||
                    c.Usuario2Id == usuarioId)
                .OrderByDescending(c => c.DataAtualizacao)
                .ToListAsync();
        }

        public async Task<ChatMensagem> EnviarMensagem(
            int conversaId,
            int remetenteId,
            string conteudo)
        {
            if (string.IsNullOrWhiteSpace(conteudo))
                throw new Exception(
                    "A mensagem não pode estar vazia"
                );

            var conversa =
                await _context.ChatConversas.FindAsync(conversaId);

            if (conversa == null)
                throw new Exception("Conversa não encontrada");

            ValidarParticipante(conversa, remetenteId);

            var mensagem = new ChatMensagem
            {
                ConversaId = conversaId,
                RemetenteId = remetenteId,
                Conteudo = conteudo.Trim(),
                DataEnvio = DateTime.UtcNow
            };

            _context.ChatMensagens.Add(mensagem);

            conversa.DataAtualizacao = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return mensagem;
        }

        public async Task<List<ChatMensagem>> ListarMensagens(
            int conversaId,
            int usuarioId,
            int page = 1,
            int limit = 20)
        {
            var conversa =
                await _context.ChatConversas.FindAsync(conversaId);

            if (conversa == null)
                throw new Exception("Conversa não encontrada");

            ValidarParticipante(conversa, usuarioId);

            return await _context.ChatMensagens
                .Where(m => m.ConversaId == conversaId)
                .OrderBy(m => m.DataEnvio)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();
        }

        private void ValidarParticipante(
            ChatConversa conversa,
            int usuarioId)
        {
            if (usuarioId != conversa.Usuario1Id &&
                usuarioId != conversa.Usuario2Id)
            {
                throw new Exception(
                    "Usuário não autorizado para esta conversa"
                );
            }
        }
    }
}