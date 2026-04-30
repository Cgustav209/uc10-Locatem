using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using uc10_Locatem.Data;
using uc10_Locatem.Enum;
using uc10_Locatem.Model;
using uc10_Locatem.Model.DTO;

namespace uc10_Locatem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AvaliacaoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AvaliacaoController(AppDbContext context)
        {
            _context = context;
        }

        // Usuário logado
        private int? ObterUsuarioId()
        {
            var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(usuarioIdClaim))
                return null;

            return int.Parse(usuarioIdClaim);
        }

        // CRIAR AVALIAÇÃO
        [HttpPost]
        public async Task<IActionResult> CriarAvaliacao([FromBody] CriarAvaliacaoDTO dto)
        {
            // Regras de negócio: Autenticação, participação, status do aluguel, nota, tipo, autoavaliação, etc.
            var usuarioId = ObterUsuarioId();
            if (usuarioId == null)
                return Unauthorized("Usuário não autenticado");

            var aluguel = await _context.Alugueis
                .FirstOrDefaultAsync(a => a.Id == dto.AluguelId);

            if (aluguel == null)
                return NotFound("Aluguel não encontrado");

            // Regra 1: só após finalizado
            if (aluguel.Status != StatusAluguel.Finalizado)
                return BadRequest("Só pode avaliar após finalização");

            // Regra 2: só quem participou (LOCADOR ou LOCATÁRIO)
            if (aluguel.UsuarioId != usuarioId &&
                aluguel.UsuarioId != usuarioId)
                return BadRequest("Você não participou deste aluguel");

            // Regra 3: uma avaliação por aluguel por usuário
            var jaExiste = await _context.Avaliacoes
                .AnyAsync(a => a.AluguelId == dto.AluguelId && a.AvaliadorId == usuarioId);

            if (jaExiste)
                return BadRequest("Você já avaliou este aluguel");

            // Regra 4: nota válida
            if (dto.Nota < 1 || dto.Nota > 5)
                return BadRequest("Nota deve ser entre 1 e 5");

            // Regra 5: valida tipo
            if (dto.TipoAvaliacao == TipoAvaliacao.Usuario && dto.AvaliadoUsuarioId == null)
                return BadRequest("Usuário avaliado é obrigatório");

            if (dto.TipoAvaliacao == TipoAvaliacao.Ferramenta && dto.FerramentaId == null)
                return BadRequest("Ferramenta é obrigatória");

            // Regra 6: impedir autoavaliação
            if (dto.AvaliadoUsuarioId == usuarioId)
                return BadRequest("Você não pode se autoavaliar");

            var avaliacao = new Avaliacao
            {
                AluguelId = dto.AluguelId,
                AvaliadorId = usuarioId.Value,
                AvaliadoUsuarioId = dto.AvaliadoUsuarioId,
                FerramentaId = dto.FerramentaId,
                Nota = dto.Nota,
                Comentario = dto.Comentario
            };

            _context.Avaliacoes.Add(avaliacao);
            await _context.SaveChangesAsync();

            return Ok(avaliacao);
        }

        // MÉDIA USUÁRIO
        [HttpGet("Usuario/{id}/Media")]
        public async Task<IActionResult> MediaUsuario(int id)
        {
            var media = await _context.Avaliacoes
                .Where(a => a.AvaliadoUsuarioId == id)
                .AverageAsync(a => (double?)a.Nota) ?? 0;

            return Ok(new { media });
        }

        // MÉDIA FERRAMENTA
        [HttpGet("Ferramenta/{id}/Media")]
        public async Task<IActionResult> MediaFerramenta(int id)
        {
            var media = await _context.Avaliacoes
                .Where(a => a.FerramentaId == id)
                .AverageAsync(a => (double?)a.Nota) ?? 0;

            return Ok(new { media });
        }

        // LISTAR AVALIAÇÕES DA FERRAMENTA
        [HttpGet("Ferramenta/{id}")]
        public async Task<IActionResult> GetAvaliacoesFerramenta(int id)
        {
            var avaliacoes = await _context.Avaliacoes
                .Where(a => a.FerramentaId == id)
                .Include(a => a.Avaliador)
                .ToListAsync();

            return Ok(avaliacoes);
        }
    }

}