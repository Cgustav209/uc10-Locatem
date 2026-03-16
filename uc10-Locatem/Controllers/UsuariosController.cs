using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using uc10_Locatem.Data;
using uc10_Locatem.Model;
using uc10_Locatem.Model.DTO;
using uc10_Locatem.Enum;

namespace uc10_Locatem.Controllers
{
    [ApiController]

    [Route("api/[controller]")]

    public class UsuariosController : ControllerBase
    {
        private readonly AppDbContext _usuarioDbContext;


        public UsuariosController(AppDbContext context)
        {
            _usuarioDbContext = context;
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

            Usuario? usuarioEncontrado = await _usuarioDbContext.Usuario.FirstOrDefaultAsync(usuario => usuario.Documento == dadosUsuario.Documeto);

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

            int resultadoGravacao = await _usuarioDbContext.SaveChangesAsync();

            if (resultadoGravacao > 0)
                return Created();

            return BadRequest("Erro ao criar usuario");
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
    }
}     

