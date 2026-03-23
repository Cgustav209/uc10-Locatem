using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using uc10_Locatem.Data;
using uc10_Locatem.Enum;
using uc10_Locatem.Model;
using uc10_Locatem.Model.DTO;

namespace uc10_Locatem.Controllers
{
    [ApiController]

    [Route("api/[controller]")]
    public class FerramentaController : ControllerBase
    {
        private readonly AppDbContext _ferramentaDbContext;

        public FerramentaController(AppDbContext context)
        {
            _ferramentaDbContext = context;
        }

        [HttpGet("GetAllTools")]
        public async Task<IActionResult> GetAllFerrametas()
        {
            List<Ferramenta> listaFerramenta = await _ferramentaDbContext.Ferramenta.ToListAsync();

            return Ok(listaFerramenta);
        }

        //[HttpGet("categoria/{categoriaId}")]
        //public async Task<IActionResult> GetByCategoria(int categoriaId)
        //{
        //    var ferramentas = await _ferramentaDbContext.Ferramenta
        //        .Include(f => f.categoria)
        //        .Where(f => f.CategoriaId == categoriaId)
        //        .ToListAsync();

        //    if (!ferramentas.Any())
        //    {
        //        return NotFound(new
        //        {
        //            Erro = true,
        //            Mensagem = "Nenhuma ferramenta encontrada para essa categoria"
        //        });
        //    }

        //    return Ok(ferramentas);
        //}

        [HttpPost("CadastrarFerramenta")]
        public async Task<ActionResult> CadastrarFerramenta([FromBody] CadastrarFerramentaDTO dadosFerramenta)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Pega ID do usuário logado (quando tiver JWT)
            var locadorId = User.FindFirst("id")?.Value;

            if (locadorId == null)
            {
                return Unauthorized("Usuário não autenticado");
            }

            int id = int.Parse(locadorId);

            Ferramenta novaFerramenta = new Ferramenta
            {
                Nome = dadosFerramenta.Nome,
                Marca = dadosFerramenta.Marca,
                Modelo = dadosFerramenta.Modelo,
                Descricao = dadosFerramenta.Descricao,
                Acessorios = dadosFerramenta.Acessorios,
                Diaria = dadosFerramenta.Diaria,

                CategoriaId = dadosFerramenta.CategoriaId,
                UsuarioId = id,
            };

            _ferramentaDbContext.Ferramenta.Add(novaFerramenta);
            int resultadoInsercao = await _ferramentaDbContext.SaveChangesAsync();

            if (resultadoInsercao > 0)
                return Created();
            return BadRequest("Ferramenta não foi registrada!");
        }

        [HttpPut("EditarFerramenta/{id}")]
        public async Task<ActionResult> EditarFerramenta(int id, [FromBody] EditarFerramentaDTO dadosFerramenta)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            
            var ferramenta = await _ferramentaDbContext.Ferramenta.FindAsync(id);

            if (ferramenta == null)
            {
                return NotFound("Ferramenta não encontrada!");
            }

            ferramenta.Nome = dadosFerramenta.Nome;
            ferramenta.Marca = dadosFerramenta.Marca;
            ferramenta.Modelo = dadosFerramenta.Modelo;
            ferramenta.Descricao = dadosFerramenta.Descricao;
            ferramenta.Acessorios = dadosFerramenta.Acessorios;
            ferramenta.Diaria = dadosFerramenta.Diaria;
          
          
           

            int resultado = await _ferramentaDbContext.SaveChangesAsync();

            if (resultado > 0)
                return Ok("Ferramenta atualizada com sucesso!");

            return BadRequest("Erro ao atualizar ferramenta!");
        }




    }
}
