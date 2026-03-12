using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using uc10_Locatem.Data;
using uc10_Locatem.Model;
using uc10_Locatem.Model.DTO;

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

            if(usuario == null)
            {
                return BadRequest(
                    new 
                    {
                        Erro = true,
                        Mensagem = $"O cliente com o id {id} não foi encontrado"
                    }
                    );
            }

            return Ok( usuario );
        }

        [HttpPost ("CriarUsuario")]
        public async Task<ActionResult> CriarUsuario([FromBody] CriarUsuarioDTO dadosUsuario) {

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

            };

            _usuarioDbContext.Usuario.Add(usuario);

            int resultadoGravacao = await _usuarioDbContext.SaveChangesAsync();

            if (resultadoGravacao > 0) 
                return Created();

            return BadRequest("Erro ao criar usuario");
        }

    }
}
