using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using uc10_Locatem.Data;
using uc10_Locatem.Model;
using uc10_Locatem.Model.DTO;

namespace uc10_Locatem.Controller
{
    [ApiController]
    [Route("api/[controller]")]

    public class CategoriaController : ControllerBase
    {
        public readonly AppDbContext _categoriaDbContext;

        public CategoriaController(AppDbContext context)
        {
            _categoriaDbContext = context;
        }

        [HttpGet("GetAllCategorias")]
        public async Task<IActionResult> GetAllEnderecos()
        {

            List<Categorias> listaCategoria = await _categoriaDbContext.Categorias.ToListAsync();

            return Ok(listaCategoria);
        }
        [HttpPost("Criar Categoria")]

        public async Task<IActionResult> CriarCategoria([FromBody] CriarCategoriaDTO dadosCategoria)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Categorias? usuarioEncontrado = await _categoriaDbContext.Categorias.FirstOrDefaultAsync(categoria => categoria.nome == dadosCategoria.Nome);

            //if (usuarioEncontrado == null)
            //{
            //    return BadRequest($"Usuario com ID {dadosCategoria.UsuarioId} não encontrado");
            //}

            Categorias novaCategoria = new Categorias
            {
                nome = dadosCategoria.Nome,
            };

            _categoriaDbContext.Categorias.Add(novaCategoria);
            int resultadoInsercao = await _categoriaDbContext.SaveChangesAsync();

            if (resultadoInsercao > 0)
                return Created();
            return BadRequest("Endereço não foi registrada!");
        }
    }
}
