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
    [Authorize]
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

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllClientes()
        {
            List<Usuario> listaUsuario = await _usuarioDbContext.Usuario.
                Include(usuario => usuario.Enderecos).
                ToListAsync();


            return Ok(listaUsuario);
        }


        [HttpGet("{tipo}/{id}")]

        public async Task<IActionResult> GetByTipoAndId([FromRoute] string tipo, [FromRoute] int id)
        {
            List<Usuario> listaUsuario = await _usuarioDbContext.Usuario.Include(usuario => usuario.Enderecos).
                ToListAsync();

            var usuario = listaUsuario.FirstOrDefault(usuario => usuario.Id == id && usuario.Tipo.ToLower() == tipo.ToLower());

            if (usuario == null)
            {
                return BadRequest(
                    new
                    {
                        Erro = true,
                        Mensagem = $"O cliente com o id {id} não foi encontrado"
                    }
                    );
            }

            return Ok(usuario);
        }

        [HttpGet("tipo/{tipo}")]
        public async Task<IActionResult> GetByTipo(TipoUsuario tipo)
        {
            var usuarios = await _usuarioDbContext.Usuario
                .Where(u => u.TipoUsuario == tipo)
                .ToListAsync();

            return Ok(usuarios);
        }

        [HttpPost("CriarUsuario")]
        public async Task<ActionResult> CriarUsuario([FromBody] CriarUsuarioDTO dadosUsuario)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            bool usuarioExiste = await _usuarioDbContext.Usuario.AnyAsync(u =>
                u.Email == dadosUsuario.Email
                || u.Documento == dadosUsuario.Documeto
            );

            if (usuarioExiste)
            {
                return Conflict(new
                {
                    Erro = true,
                    Mensagem = "Já existe um usuário com esse CPF ou e-mail."
                });
            }

            Usuario usuario = new Usuario
            {
                Nome = dadosUsuario.Nome,
                Email = dadosUsuario.Email,
                Tipo = dadosUsuario.Tipo,
                Telefone = dadosUsuario.Telefone,
                Documento = dadosUsuario.Documeto,
                TipoUsuario = dadosUsuario.TipoUsuario,

            };

            _usuarioDbContext.Usuario.Add(usuario);

            try
            {
                int resultadoGravacao = await _usuarioDbContext.SaveChangesAsync();

                if (resultadoGravacao > 0)
                    return Created();
            }
            catch (DbUpdateException)
            {
                return Conflict(new
                {
                    Erro = true,
                    Mensagem = "CPF ou e-mail já cadastrado."
                });
            }

            return BadRequest("Erro ao criar usuário");
        }

        [HttpPost("upload-foto")]
        public async Task<IActionResult> UploadFoto([FromForm] UsuarioFotoDTO dto)
        {
            var usuario = await _usuarioDbContext.Usuario.FindAsync(dto.UsuarioId);

            if (usuario == null)
                return NotFound("Usuário não encontrado");

            if (dto.Foto == null || dto.Foto.Length == 0)
                return BadRequest("Arquivo inválido");

            var nomeArquivo = Guid.NewGuid().ToString()
                + Path.GetExtension(dto.Foto.FileName);

            var pasta = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot/uploads/usuarios"
            );

            if (!Directory.Exists(pasta))
                Directory.CreateDirectory(pasta);

            var caminho = Path.Combine(pasta, nomeArquivo);

            using (var stream = new FileStream(caminho, FileMode.Create))
            {
                await dto.Foto.CopyToAsync(stream);
            }

            usuario.FotoPerfilUrl = $"/uploads/usuarios/{nomeArquivo}";

            await _usuarioDbContext.SaveChangesAsync();

            return Ok(new { usuario.FotoPerfilUrl });
        }


        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDTO dadosUsuario)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Busca usuário pelo email
            var usuario = await _usuarioService.GetUserByEmail(dadosUsuario.Email);

            if (usuario == null || usuario.Senha != dadosUsuario.Senha)
            {
                return Unauthorized();
            }

            // AQUI ENTRA O TOKEN
            var token = _tokenService.GerarToken(usuario);

            return Ok(new { token });
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
            var usuario = await _usuarioDbContext.Usuario
                .FirstOrDefaultAsync(u => u.Id == id);

            if (usuario == null)
            {
                return NotFound("Usuário não encontrado");
            }

            // Valida senha atual
            if (usuario.Senha != dadosUsuario.SenhaAtual)
            {
                return BadRequest("Senha atual incorreta");
            }

            // Atualiza senha
            usuario.Senha = dadosUsuario.NovaSenha;

            await _usuarioDbContext.SaveChangesAsync();

            return Ok(new
            {
                Mensagem = "Senha alterada com sucesso"
            });
        }
    }
}     