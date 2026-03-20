using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using uc10_Locatem.API.Model;
using uc10_Locatem.API.Model.DTO;
using uc10_Locatem.Data;
using uc10_Locatem.Model;
using uc10_Locatem.Model.DTO;
using uc10_Locatem.Enum;


namespace uc10_Locatem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class EnderecoController : ControllerBase
    {
        private readonly AppDbContext _enderecoDbContext;


        public EnderecoController(AppDbContext context)
        {
            _enderecoDbContext = context;
        }

        [HttpGet("GetAllEnderecos")]
        public async Task<IActionResult> GetAllEnderecos()
        {

            List<Endereco> listaEnderecos = await _enderecoDbContext.Endereco.ToListAsync();

            return Ok(listaEnderecos);
        }

        [HttpPost("Criar endereço")]

        public async Task<IActionResult> CriarEndereco([FromBody] CriarEnderecoDTO dadodEndereco)
        {
            if (!ModelState.IsValid) 
            {
             return BadRequest(ModelState);
            }

           Usuario? usuarioEncontrado = await _enderecoDbContext.Usuario.FirstOrDefaultAsync(usuario => usuario.Id == dadodEndereco.UsuarioId);

            if (usuarioEncontrado == null)
            {
                return BadRequest($"Usuario com ID {dadodEndereco.UsuarioId} não encontrado");
            }

            Endereco novoEndereco = new Endereco 
            {
                Logradouro = dadodEndereco.Logradouro,
                Numero = dadodEndereco.Numero,
                Complemento = dadodEndereco.Complemento,
                Bairro = dadodEndereco.Bairro,
                Cidade = dadodEndereco.Cidade,
                Estado = dadodEndereco.Estado,
                CEP = dadodEndereco.CEP,
                TipoEndereco = dadodEndereco.TipoEndereco,
                UsuarioId = dadodEndereco.UsuarioId,
            };

            _enderecoDbContext.Endereco.Add(novoEndereco);
            int resultadoInsercao = await _enderecoDbContext.SaveChangesAsync();

            if (resultadoInsercao > 0)
                return Created();
            return BadRequest("Endereço não foi registrada!");
        }
    }
}
