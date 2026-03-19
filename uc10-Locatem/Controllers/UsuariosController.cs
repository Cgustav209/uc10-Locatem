using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using uc10_Locatem.Data;
using uc10_Locatem.Enum;
using uc10_Locatem.Model;
using uc10_Locatem.Model.DTO;
using uc10_Locatem.Services;

namespace uc10_Locatem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class UsuariosController : ControllerBase 
    {
        private readonly AppDbContext _usuarioDbContext;
        private readonly UsuarioService _usuarioService;
        private readonly TokenService _tokenService;


        public UsuariosController(AppDbContext context, UsuarioService usuarioService, TokenService tokenService)
        {
            _usuarioDbContext = context;
            _usuarioService = usuarioService;
            _tokenService = tokenService;
        }

        [HttpGet("{tipo}/{id}")]
        public async Task<IActionResult> GetByTipoAndId([FromRoute] string tipo, [FromRoute] int id)
        {
            var usuario = await _usuarioDbContext.Usuario.Include(u => u.Enderecos).FirstOrDefaultAsync(u =>u.Id == id && u.Tipo.ToLower() == tipo.ToLower()
        );

            if (usuario == null)
            {
                return NotFound(new
                {
                    Erro = true,
                    Mensagem = $"Usuário com id {id} não encontrado"
                });
            }

            return Ok(usuario);
        }

        [Authorize]
        [HttpPut("alterarSenha")]
        public async Task<IActionResult> AlterarSenha([FromBody] AlterarSenhaDTO dadosUsuario)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Pega ID do usuário logado (quando tiver JWT)
            var usuarioId = User.FindFirst("id")?.Value;

            if (usuarioId == null)
            {
                return Unauthorized("Usuário não autenticado");
            }

            int id = int.Parse(usuarioId);

            // Busca usuário no banco
            var usuario = await _usuarioDbContext.Usuario.FirstOrDefaultAsync(u => u.Id == id);

            if (usuario == null)
            {
                return NotFound("Usuário não encontrado");
            }

            // Valida senha atual
            if (!BCrypt.Net.BCrypt.Verify(dadosUsuario.SenhaAtual, usuario.Senha))
            {
                return BadRequest("Senha atual incorreta");
            }

            // Gerar uma nova senha e um hash para ela
            usuario.Senha = BCrypt.Net.BCrypt.HashPassword(dadosUsuario.NovaSenha);

            await _usuarioDbContext.SaveChangesAsync();

            return Ok(new
            {
                Mensagem = "Senha alterada com sucesso"
            });
        }
    }
}     