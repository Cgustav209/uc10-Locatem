using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using uc10_Locatem.Model.DTO;
using uc10_Locatem.Services;

namespace uc10_Locatem.Controllers
{
    [Route("api/chat")]
    [ApiController]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly ChatService _service;

        public ChatController(ChatService service)
        {
            _service = service;
        }

        private int ObterUsuarioId()
        {
            var id = User.FindFirst("id")?.Value;

            if (string.IsNullOrEmpty(id))
                throw new Exception("Usuário não autenticado");

            return int.Parse(id);
        }

        [HttpPost("abrir")]
        public async Task<IActionResult> AbrirConversa(
            [FromBody] CriarConversaDTO dados)
        {
            var usuarioId = ObterUsuarioId();

            var conversa = await _service.ObterOuCriarConversa(
                usuarioId,
                dados.OutroUsuarioId,
                dados.ReservaId,
                dados.FerramentaId
            );

            return Ok(conversa);
        }

        [HttpGet("me")]
        public async Task<IActionResult> ListarConversas()
        {
            var usuarioId = ObterUsuarioId();

            return Ok(
                await _service.ListarConversasUsuario(usuarioId)
            );
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterConversa(int id)
        {
            var usuarioId = ObterUsuarioId();

            return Ok(
                await _service.ObterConversaPorId(id, usuarioId)
            );
        }

        [HttpPost("{conversaId}/mensagens")]
        public async Task<IActionResult> EnviarMensagem(
            int conversaId,
            [FromBody] EnviarMensagemDTO dados)
        {
            var usuarioId = ObterUsuarioId();

            return Ok(await _service.EnviarMensagem(
                conversaId,
                usuarioId,
                dados.Conteudo
            ));
        }

        [HttpGet("{conversaId}/mensagens")]
        public async Task<IActionResult> ListarMensagens(
            int conversaId,
            int page = 1,
            int limit = 20)
        {
            var usuarioId = ObterUsuarioId();

            return Ok(
                await _service.ListarMensagens(
                    conversaId,
                    usuarioId,
                    page,
                    limit
                )
            );
        }
    }
}