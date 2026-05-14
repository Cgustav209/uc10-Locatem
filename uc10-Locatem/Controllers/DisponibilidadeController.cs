using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using uc10_Locatem.Model.DTO;
using uc10_Locatem.Services.Interfaces;

namespace uc10_Locatem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DisponibilidadeController : ControllerBase
    {
        private readonly IDisponibilidadeService _disponibilidadeService;

        public DisponibilidadeController(
            IDisponibilidadeService disponibilidadeService)
        {
            _disponibilidadeService = disponibilidadeService;
        }

        // =========================================================
        // VERIFICAR DISPONIBILIDADE
        // GET: api/disponibilidade?ferramentaId=1&dataInicio=2026-03-30&dataFim=2026-04-02
        // =========================================================

        [HttpGet("Disponibilidade")]
        public async Task<IActionResult> VerificarDisponibilidade(
            [FromQuery] int ferramentaId,
            [FromQuery] DateTime dataInicio,
            [FromQuery] DateTime dataFim)
        {
            // VALIDA ID
            if (ferramentaId <= 0)
            {
                return BadRequest(new ApiResponseDTO
                {
                    Sucesso = false,
                    Mensagem = "ID da ferramenta inválido."
                });
            }

            // VALIDA DATAS PASSADAS
            if (dataInicio.Date < DateTime.UtcNow.Date)
            {
                return BadRequest(new ApiResponseDTO
                {
                    Sucesso = false,
                    Mensagem = "Não é permitido consultar datas passadas."
                });
            }

            // VALIDA INTERVALO
            if (dataFim <= dataInicio)
            {
                return BadRequest(new ApiResponseDTO
                {
                    Sucesso = false,
                    Mensagem = "A data final deve ser maior que a data inicial."
                });
            }

            VerificarDisponibilidadeDTO dto = new()
            {
                FerramentaId = ferramentaId,
                DataInicio = dataInicio,
                DataFim = dataFim
            };

            DisponibilidadeResponseDTO response =
                await _disponibilidadeService.VerificarDisponibilidade(dto);

            return Ok(response);
        }

        // =========================================================
        // OBTER AGENDA DA FERRAMENTA
        // GET: api/disponibilidade/agenda/1
        // =========================================================

        [HttpGet("agenda/{ferramentaId}")]
        public async Task<IActionResult> ObterAgenda(
            [FromRoute] int ferramentaId)
        {
            if (ferramentaId <= 0)
            {
                return BadRequest(new ApiResponseDTO
                {
                    Sucesso = false,
                    Mensagem = "ID da ferramenta inválido."
                });
            }

            AgendaDisponibilidadeResponseDTO agenda =
                await _disponibilidadeService.ObterAgenda(ferramentaId);

            return Ok(agenda);
        }

        // =========================================================
        // CRIAR BLOQUEIO
        // POST: api/disponibilidade/bloquear
        // =========================================================

        [Authorize]
        [HttpPost("bloquear")]
        public async Task<IActionResult> CriarBloqueio(
            [FromBody] BloqueioDisponibilidadeDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // VALIDA DATAS
            if (dto.DataFim <= dto.DataInicio)
            {
                return BadRequest(new ApiResponseDTO
                {
                    Sucesso = false,
                    Mensagem = "A data final deve ser maior que a inicial."
                });
            }

            // VALIDA DATAS PASSADAS
            if (dto.DataInicio.Date < DateTime.UtcNow.Date)
            {
                return BadRequest(new ApiResponseDTO
                {
                    Sucesso = false,
                    Mensagem = "Não é permitido bloquear datas passadas."
                });
            }

            // PEGA USUÁRIO LOGADO
            var usuarioIdClaim = User.FindFirst("id")?.Value;

            if (string.IsNullOrWhiteSpace(usuarioIdClaim))
            {
                return Unauthorized(new ApiResponseDTO
                {
                    Sucesso = false,
                    Mensagem = "Usuário não autenticado."
                });
            }

            // CONVERTE ID
            bool conversao = int.TryParse(usuarioIdClaim, out int usuarioId);

            if (!conversao)
            {
                return Unauthorized(new ApiResponseDTO
                {
                    Sucesso = false,
                    Mensagem = "ID do usuário inválido."
                });
            }

            // EXECUTA SERVICE
            ApiResponseDTO response =
                await _disponibilidadeService.CriarBloqueio(dto, usuarioId);

            // ERRO DO SERVICE
            if (!response.Sucesso)
            {
                return BadRequest(new ApiResponseDTO
                {
                    Sucesso = false,
                    Mensagem = response.Mensagem
                });
            }

            // SUCESSO
            return Ok(new ApiResponseDTO
            {
                Sucesso = true,
                Mensagem = response.Mensagem
            });
        }
    }
}