using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using uc10_Locatem.Data;
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

        [HttpPost("CadastrarFerramenta")]
        public async Task<ActionResult> CadastrarFerramenta([FromBody] CadastrarFerramentaDTO dadosFerramenta){
             if (!ModelState.IsValid) 
            {
             return BadRequest(ModelState);
            }

            Ferramenta novaFerramenta = new Ferramenta
            {
                Nome = dadosFerramenta.Nome,
                Marca = dadosFerramenta.Marca,
                Modelo = dadosFerramenta.Modelo,
                Descricao = dadosFerramenta.Descricao,
                Acessorios = dadosFerramenta.Acessorios,
                Diaria = dadosFerramenta.Diaria,
                Caucao = dadosFerramenta.Caucao,
                CategoriaId = dadosFerramenta.CategoriaId,
                UsuarioId = dadosFerramenta.UsuarioId,
            };

            _ferramentaDbContext.Ferramenta.Add(novaFerramenta);
            int resultadoInsercao = await _ferramentaDbContext.SaveChangesAsync();

            if (resultadoInsercao > 0)
                return Created();
            return BadRequest("Ferramenta não foi registrada!");
        }
            

    

       

    }
}
