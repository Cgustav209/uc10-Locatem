using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using uc10_Locatem.Data;
using uc10_Locatem.Enum;
using uc10_Locatem.Model;
using uc10_Locatem.Model.DTO;
using uc10_Locatem.Services;

namespace uc10_Locatem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CadastroController : ControllerBase
    {
        private readonly UsuarioService _usuarioService;

        public CadastroController(UsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [AllowAnonymous]
        [HttpPost("CriarUsuario")]
        public async Task<ActionResult> CriarUsuario([FromBody] CriarUsuarioDTO dadosUsuario)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var usuarioExiste = await _usuarioService.UsuarioExiste(dadosUsuario.Email, dadosUsuario.Documento);

            if (usuarioExiste)
            {
                return Conflict(new { Mensagem = "Usuário já existe" });
            }

            string senhaHash = BCrypt.Net.BCrypt.HashPassword(dadosUsuario.Senha);

            Usuario usuario = new Usuario
            {
                Nome = dadosUsuario.Nome,
                Email = dadosUsuario.Email,
                Senha = senhaHash,
                TipoUsuario = dadosUsuario.TipoUsuario,
                Telefone = dadosUsuario.Telefone,
                Documento = dadosUsuario.Documento,
            };

            await _usuarioService.CriarUsuario(usuario);

            return CreatedAtAction(nameof(CriarUsuario), new { id = usuario.Id }, new
            {
                Mensagem = "Usuário criado com sucesso",
                Nome = usuario.Nome,
                TipoUsuario = usuario.TipoUsuario.ToString(),
                Id = usuario.Id
            });

            
        }
    }
}
