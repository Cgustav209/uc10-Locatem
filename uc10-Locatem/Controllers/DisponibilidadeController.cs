using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using uc10_Locatem.Model.DTO;
using uc10_Locatem.Services;

namespace uc10_Locatem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DisponibilidadeController : ControllerBase
    {
        private readonly DisponibilidadeService _disponibilidadeService;

        public DisponibilidadeController(DisponibilidadeService disponibilidadeService)
        {
            _disponibilidadeService = disponibilidadeService;
        }

        // =========================================================
        // GET: api/disponibilidade?ferramentaId=1&dataInicio=2026-03-30&dataFim=2026-04-02
        // =========================================================
        [HttpGet]
        public async Task<IActionResult> VerificarDisponibilidade(
            [FromQuery] int ferramentaId,
            [FromQuery] DateTime dataInicio,
            [FromQuery] DateTime dataFim)
        {
            VerificarDisponibilidadeDTO dto = new VerificarDisponibilidadeDTO
            {
                FerramentaId = ferramentaId,
                DataInicio = dataInicio,
                DataFim = dataFim
            };

            DisponibilidadeResponseDTO response = await _disponibilidadeService.VerificarDisponibilidade(dto);

            return Ok(response);
        }

        // =========================================================
        // GET: api/disponibilidade/agenda/1
        // =========================================================
        [HttpGet("agenda/{ferramentaId}")]
        public async Task<IActionResult> ObterAgenda([FromRoute] int ferramentaId)
        {
            AgendaDisponibilidadeResponseDTO agenda = await _disponibilidadeService.ObterAgenda(ferramentaId);

            return Ok(agenda);
        }

        // =========================================================
        // POST: api/disponibilidade/bloqueio
        // =========================================================
        [Authorize]
        [HttpPost("bloqueio")]
        public async Task<IActionResult> CriarBloqueio([FromBody] BloqueioDisponibilidadeDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var usuarioIdClaim = User.FindFirst("id")?.Value;

            if (string.IsNullOrEmpty(usuarioIdClaim))
            {
                return Unauthorized(new
                {
                    Mensagem = "Usuário não autenticado."
                });
            }

            int usuarioId = int.Parse(usuarioIdClaim);

            BloqueioResponseDTO response = await _disponibilidadeService.CriarBloqueio(dto, usuarioId);

            if (!response.Sucesso)
            {
                return BadRequest(new
                {
                    Mensagem = response.Mensagem
                });
            }

            return Ok(new
            {
                Mensagem = response.Mensagem
            });
        }
    }
}