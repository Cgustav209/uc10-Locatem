using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using uc10_Locatem.Data;
using uc10_Locatem.Model;
using uc10_Locatem.Model.DTO;
using uc10_Locatem.Services;

namespace uc10_Locatem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    // aqui vai tudo q é resposta http, rota e requisição
    public class AluguelController : ControllerBase
    {
        // ================================
        // DEPENDÊNCIAS
        // ================================
        private readonly AppDbContext _aluguelDbContext;
        private readonly AluguelService _aluguelService;

        // ================================
        // CONSTRUTOR
        // ================================
        public AluguelController(AppDbContext context, AluguelService aluguelService)
        {
            _aluguelDbContext = context;
            _aluguelService = aluguelService;
        }

        // ================================
        // GET - LISTAR TODOS OS ALUGUÉIS
        // ================================
        [HttpGet("GetAllAlugueis")]
        public async Task<IActionResult> GetAllAlugueis()
        {
            List<Aluguel> listaAlugueis = await _aluguelDbContext.Alugueis.ToListAsync();
            return Ok(listaAlugueis);
        }

        // ================================
        // POST - CRIAR ALUGUEL MANUALMENTE
        // ================================
        [HttpPost("CriarAluguel")]
        public async Task<IActionResult> CriarAluguel([FromBody] CriarAluguelDTO dadosAluguel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var resultado = await _aluguelService.CriarAluguelAsync(dadosAluguel);

            if (!resultado.Sucesso)
            {
                return StatusCode(resultado.StatusCode, resultado.Mensagem);
            }

            return Created("", resultado.Dados);
        }

        // =========================================
        // POST - CRIAR ALUGUEL A PARTIR DE RESERVA
        // =========================================
        //[Authorize]
        [HttpPost("criar-por-reserva/{reservaId}")]
        public async Task<IActionResult> CriarPorReserva(int reservaId, int? usuarioIdTeste = null)
        {
            var claim = User.FindFirst("id");

            if (!usuarioIdTeste.HasValue && claim == null)
            {
                return Unauthorized("Token inválido: ID não encontrado.");
            }

            int usuarioId = usuarioIdTeste ?? int.Parse(claim!.Value);

            var resultado = await _aluguelService.CriarPorReservaAsync(reservaId, usuarioId);

            if (!resultado.Sucesso)
            {
                return StatusCode(resultado.StatusCode, resultado.Mensagem);
            }

            return Created("", resultado.Dados);
        }

        // ================================
        // POST - PAGAR ALUGUEL
        // ================================
        //[Authorize]
        [HttpPost("pagar/{aluguelId}")]
        public async Task<IActionResult> PagarAluguel(int aluguelId, int? usuarioIdTeste = null)
        {
            var claim = User.FindFirst("id");

            if (!usuarioIdTeste.HasValue && claim == null)
            {
                return Unauthorized("Token inválido: ID não encontrado.");
            }

            int usuarioId = usuarioIdTeste ?? int.Parse(claim!.Value);

            var resultado = await _aluguelService.PagarAluguelAsync(aluguelId, usuarioId);

            if (!resultado.Sucesso)
            {
                return StatusCode(resultado.StatusCode, resultado.Mensagem);
            }

            return Ok(resultado.Dados);
        }

        // ================================
        // POST - SOLICITAR FINALIZAÇÃO
        // ================================
        //[Authorize]
        [HttpPost("solicitar-finalizacao/{aluguelId}")]
        public async Task<IActionResult> SolicitarFinalizacao(int aluguelId, int? usuarioIdTeste = null)
        {
            var claim = User.FindFirst("id");

            if (!usuarioIdTeste.HasValue && claim == null)
            {
                return Unauthorized("Token inválido: ID não encontrado.");
            }

            int usuarioId = usuarioIdTeste ?? int.Parse(claim!.Value);

            var resultado = await _aluguelService.SolicitarFinalizacaoAsync(aluguelId, usuarioId);

            if (!resultado.Sucesso)
            {
                return StatusCode(resultado.StatusCode, resultado.Mensagem);
            }

            return Ok(resultado.Dados);
        }

        // ================================
        // POST - CONFIRMAR FINALIZAÇÃO
        // ================================
        //[Authorize]
        [HttpPost("confirmar-finalizacao/{aluguelId}")]
        public async Task<IActionResult> ConfirmarFinalizacao(int aluguelId, int? usuarioIdTeste = null)
        {
            var claim = User.FindFirst("id");

            if (!usuarioIdTeste.HasValue && claim == null)
            {
                return Unauthorized("Token inválido: ID não encontrado.");
            }

            int usuarioId = usuarioIdTeste ?? int.Parse(claim!.Value);

            var resultado = await _aluguelService.ConfirmarFinalizacaoAsync(aluguelId, usuarioId);

            if (!resultado.Sucesso)
            {
                return StatusCode(resultado.StatusCode, resultado.Mensagem);
            }

            return Ok(resultado.Dados);
        }

        // ================================
        // POST - RECUSAR FINALIZAÇÃO
        // ================================
        //[Authorize]
        [HttpPost("recusar-finalizacao/{aluguelId}")]
        public async Task<IActionResult> RecusarFinalizacao(int aluguelId, int? usuarioIdTeste = null)
        {
            var claim = User.FindFirst("id");

            if (!usuarioIdTeste.HasValue && claim == null)
            {
                return Unauthorized("Token inválido: ID não encontrado.");
            }

            int usuarioId = usuarioIdTeste ?? int.Parse(claim!.Value);

            var resultado = await _aluguelService.RecusarFinalizacaoAsync(aluguelId, usuarioId);

            if (!resultado.Sucesso)
            {
                return StatusCode(resultado.StatusCode, resultado.Mensagem);
            }

            return Ok(resultado.Dados);
        }
    }
}