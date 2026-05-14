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
        private readonly AppDbContext _categoriaDbContext;

        public CategoriaController(AppDbContext context)
        {
            _categoriaDbContext = context;
        }

        // LISTAR TODAS
        [HttpGet]
        public async Task<IActionResult> GetAllCategorias()
        {
            var categorias = await _categoriaDbContext.Categorias.ToListAsync();
            return Ok(categorias);
        }

        // LISTAGEM HIERÁRQUICA
        [HttpGet("Hierarquia")]
        public async Task<IActionResult> GetHierarquia()
        {
            var categorias = await _categoriaDbContext.Categorias
                .Where(c => c.CategoriaPaiId == null)
                .Include(c => c.Subcategorias)
                .ToListAsync();

            return Ok(categorias);
        }

        // CRIAR CATEGORIA / SUBCATEGORIA
        [HttpPost("Criar Categoria")]
        public async Task<IActionResult> CriarCategoria([FromBody] CriarCategoriaDTO dadosCategoria)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //  Verifica duplicidade
            var existe = await _categoriaDbContext.Categorias
                .AnyAsync(c => c.nome == dadosCategoria.Nome);

            if (existe)
                return BadRequest("Categoria já existe");

            //  Se for subcategoria, valida pai
            if (dadosCategoria.CategoriaPaiId.HasValue)
            {
                var categoriaPai = await _categoriaDbContext.Categorias
                    .FindAsync(dadosCategoria.CategoriaPaiId);

                if (categoriaPai == null)
                    return BadRequest("Categoria pai não encontrada");
            }

            var novaCategoria = new Categoria
            {
                nome = dadosCategoria.Nome,
                CategoriaPaiId = dadosCategoria.CategoriaPaiId
            };

            _categoriaDbContext.Categorias.Add(novaCategoria);
            await _categoriaDbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAllCategorias), new { id = novaCategoria.Id }, novaCategoria);
        }

        // DELETAR (COM REGRAS)
        [HttpDelete("Deletar Categoria/{id}")]
        public async Task<IActionResult> DeletarCategoria(int id)
        {
            var categoria = await _categoriaDbContext.Categorias
                .Include(c => c.Subcategorias)
                .Include(c => c.Ferramentas)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (categoria == null)
                return NotFound("Categoria não encontrada");

            // REGRA: não deletar se tiver ferramentas
            if (categoria.Ferramentas != null && categoria.Ferramentas.Any())
                return BadRequest("Categoria possui ferramentas vinculadas");

            // REGRA: não deletar se tiver subcategorias
            if (categoria.Subcategorias != null && categoria.Subcategorias.Any())
                return BadRequest("Categoria possui subcategorias");

            // REGRA: categoria padrão
            if (categoria.EhPadrao)
                return BadRequest("Categoria padrão não pode ser excluída");

            _categoriaDbContext.Categorias.Remove(categoria);
            await _categoriaDbContext.SaveChangesAsync();

            return Ok("Categoria deletada com sucesso");
        }
    }
}
