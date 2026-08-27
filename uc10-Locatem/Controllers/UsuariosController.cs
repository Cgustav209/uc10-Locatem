using Locatem.Models.DTO;
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
        public async Task<IActionResult> GetByTipoAndId(TipoUsuario tipo, int id)
        {
            var usuario = await _usuarioDbContext.Usuario.Include(u => u.Enderecos).FirstOrDefaultAsync(u => u.Id == id && u.TipoUsuario == tipo
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
        [HttpGet("me")]
        public async Task<IActionResult> GetUsuarioLogado()
        {
            var usuarioId = User.FindFirst("id")?.Value;

            if (usuarioId == null)
            {
                return Unauthorized("Usuário não autenticado");
            }

            int id = int.Parse(usuarioId);

            var usuario = await _usuarioDbContext.Usuario
                .Include(u => u.Enderecos)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (usuario == null)
            {
                return NotFound("Usuário não encontrado");
            }

            var avaliacoes = await _usuarioDbContext.Avaliacoes
                .Where(a => a.AvaliadoUsuarioId == id)
                .ToListAsync();

            var mediaAvaliacao = avaliacoes.Count > 0
                ? avaliacoes.Average(a => a.Nota)
                : 0;

            var totalAvaliacoes = avaliacoes.Count;

            var locacoesConcluidas = usuario.TipoUsuario == TipoUsuario.Locador
                ? await _usuarioDbContext.Alugueis
                    .CountAsync(a =>
                        a.Ferramenta.UsuarioId == id &&
                        a.Status == StatusAluguel.Finalizado)
                : await _usuarioDbContext.Alugueis
                    .CountAsync(a =>
                        a.UsuarioId == id &&
                        a.Status == StatusAluguel.Finalizado);
            return Ok(new
            {
                id = usuario.Id,
                nome = usuario.Nome,
                email = usuario.Email,
                telefone = usuario.Telefone,
                documento = usuario.Documento,
                tipoUsuario = usuario.TipoUsuario.ToString(),
                endereco = usuario.Endereco,
                desde = usuario.DataCadastro.Year,
                fotoUrl = usuario.UrlFoto,
                reputacao = new
                {
                    rating = mediaAvaliacao,
                    totalAvaliacoes = totalAvaliacoes,
                    locacoesConcluidas = locacoesConcluidas
                },
                enderecos = usuario.Enderecos
            });
        }

        [HttpPut("me")]
        [Authorize]
        public async Task<IActionResult> AtualizarPerfil([FromBody] EditarUsuarioDTO dto)
        {
            var usuarioIdClaim = User.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(usuarioIdClaim) || !int.TryParse(usuarioIdClaim, out int usuarioId))
            {
                return Unauthorized(new { mensagem = "Usuário não autenticado." });
            }

            var usuario = await _usuarioDbContext.Usuario.FindAsync(usuarioId);
            if (usuario == null)
            {
                return NotFound(new { mensagem = "Usuário não encontrado." });
            }

            // Atualiza os dados
            usuario.Nome = dto.Nome;
            usuario.Telefone = dto.Telefone;
            usuario.Documento = dto.Documento;
            usuario.Endereco = dto.Endereco;

            _usuarioDbContext.Usuario.Update(usuario);
            await _usuarioDbContext.SaveChangesAsync();

            return Ok(new { mensagem = "Perfil atualizado com sucesso." });
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

        // erro (utilizar o CadastroController.cs)
        //[HttpPost("CriarUsuario")]
        //public async Task<IActionResult> CriarUsuario([FromBody] CriarUsuarioDTO dadosUsuario) 
        //{

        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    //importante: para fazer a verificação do documento.
        //    // _usuarioDbContext.Usuario.FirstOrDefaultAsync(cliente => cliente.CPF == dadosCliente);

        //    //if (clienteExistente != null)
        //    //{
        //    //    return BadRequest($"Já existe um cliente com esse CPF {dadosCliente.CPF}");
        //    //}

        //    var usuario = new Usuario
        //    {
        //        Nome = dadosUsuario.Nome,
        //        Email = dadosUsuario.Email,
        //        Telefone = dadosUsuario.Telefone,
        //        TipoUsuario = dadosUsuario.TipoUsuario,
        //        Documento = dadosUsuario.Documento
        //    };

        //    _usuarioDbContext.Usuario.Add(usuario);
        //    int resultadoGravacao = await _usuarioDbContext.SaveChangesAsync();


        //    if (resultadoGravacao > 0)
        //        return Created();

        //    return BadRequest("Erro ao criar usuario");
        //}
    }
}     
