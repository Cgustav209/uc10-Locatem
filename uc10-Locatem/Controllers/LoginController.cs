using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using uc10_Locatem.Data;
using uc10_Locatem.Model.DTO;
using uc10_Locatem.Services;

namespace uc10_Locatem.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly UsuarioService _usuarioService;
        private readonly TokenService _tokenService;
        private readonly AuthService _authService;

        public LoginController(UsuarioService usuarioService, TokenService tokenService, AuthService authService)
        {
            _usuarioService = usuarioService;
            _tokenService = tokenService;
            _authService = authService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDTO dadosUsuario)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var usuario = await _authService.Login(dadosUsuario);

            if (usuario == null)
            {
                return Unauthorized(new
                {
                    Mensagem = "E-mail ou senha inválidos."
                });
            }

            var token = _tokenService.GerarToken(usuario);

            return Ok(new
            {
                Mensagem = "Login realizado com sucesso",
                Nome = usuario.Nome,
                TipoUsuario = usuario.TipoUsuario.ToString(),
                Token = token
            });
        }
    }
}