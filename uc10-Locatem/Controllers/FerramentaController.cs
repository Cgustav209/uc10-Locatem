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
        public async Task<ActionResult> CadastrarFerramenta([FromBody]) CadastrarFerramentaDTO dadosFerramentas){
            
            }

    

       

    }
}
