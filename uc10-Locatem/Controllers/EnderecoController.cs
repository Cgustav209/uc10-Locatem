using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using uc10_Locatem.API.Model;
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

        [HttpPost("CriarEndereco")]
        public async Task<IActionResult> CriarEndereco([FromBody] CriarEnderecoDTO dadosEndereco)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Usuario? clienteEncontrado = await _enderecoDbContext.Usuario.FirstOrDefaultAsync(usuario => usuario.Id == dadosEndereco.ClienteId);

            if (clienteEncontrado == null)
            {
                return NotFound($"Cliente com ID {dadosEndereco.ClienteId} não encontrado.");
            }

            Endereco novoEndereco = new Endereco
            {
                Logradouro = dadosEndereco.Logradouro,
                Numero = dadosEndereco.Numero,
                Complemento = dadosEndereco.Complemento,
                Bairro = dadosEndereco.Bairro,
                Cidade = dadosEndereco.Cidade,
                Estado = dadosEndereco.Estado,
                CEP = dadosEndereco.CEP,
                TipoEndereco = dadosEndereco.TipoEndereco,
                UsuarioId = dadosEndereco.ClienteId
            };

            _enderecoDbContext.Endereco.Add(novoEndereco);
            int resultadoInsercao = await _enderecoDbContext.SaveChangesAsync();

            if (resultadoInsercao > 0)
                return Created();

            return BadRequest("Endereço não foi registrado");
        }
    }
}
