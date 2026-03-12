using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            List<Usuario> listaUsuarios= await _usuarioDbContext.Usuario.Include(usuario => usuario.Enderecos).ToListAsync();

            return Ok(listaUsuarios);
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


        [HttpPost("CriarUsuario")]
        public async Task<IActionResult> CriarUsuario([FromBody] CriarUsuarioDTO dadosUsuario) 
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //importante: para fazer a verificação do documento.
            // _usuarioDbContext.Usuario.FirstOrDefaultAsync(cliente => cliente.CPF == dadosCliente);

            //if (clienteExistente != null)
            //{
            //    return BadRequest($"Já existe um cliente com esse CPF {dadosCliente.CPF}");
            //}

            var usuario = new Usuario
            {
                Nome = dadosUsuario.Nome,
                Email = dadosUsuario.Email,
                Telefone = dadosUsuario.Telefone,
                Tipo = dadosUsuario.Tipo,
                Documento = dadosUsuario.Documento
            };

            _usuarioDbContext.Usuario.Add(usuario);
            int resultadoGravacao = await _usuarioDbContext.SaveChangesAsync();


            if (resultadoGravacao > 0)
                return Created();

            return BadRequest("Erro ao criar usuario");
        }
    }
}
