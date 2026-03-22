using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.Xml;
using uc10_Locatem.Data;
using uc10_Locatem.Enum;
using uc10_Locatem.Model;
using uc10_Locatem.Model.DTO;

namespace uc10_Locatem.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // cria rota
    public class AluguelController : ControllerBase
    {
        public readonly AppDbContext _aluguelDbContext; // Aqui fica guardado uma referência ao banco

        // Construtor recebe a refencia automaticamente.
        // Injeção de dependência.
        // Traduzindo: o sistema entrega o banco pronto para o controller.
        public AluguelController(AppDbContext context)
        {
            _aluguelDbContext = context;
        }


        [HttpGet("GetAllAlugueis")]
        public async Task<IActionResult> GetAllAlugueis()
        {
            List<Aluguel> listaAlugueis = await _aluguelDbContext.Alugueis.ToListAsync();

            return Ok(listaAlugueis);

            // Fluxo:
            // 1. API recebe GET
            // 2. Consulta tabela Aluguel       
            // 3. Transforma em lista
            // 4. Retorna JSON
        }

        [HttpPost("CriarAluguel")]
        // [FromBody] = os dados virão no corpo da requisição em JSON.
        public async Task<IActionResult> CriarAluguel([FromBody] CriarAluguelDTO dadosAluguel)
        {
            // Validação do DTO. Se algum campo obrigatório faltar, a API responde: 400 Bad Request
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Verifica se a ferramenta existe. Isso evita criar aluguel de algo que não existe no banco.
            //Ferramenta? ferramentaEncontrada = await _aluguelDbContext.Ferramenta.FirstOrDefaultAsync(f => f.Id == dadosAluguel.FerramentaId);

            //if (ferramentaEncontrada == null)
            //{
            //    return BadRequest($"Ferramenta com ID {dadosAluguel.FerramentaId} não encontrada");
            //}

            // Criar objeto Aluguel
            // Esse objeto representa uma linha na tabela Aluguel.
            Aluguel novoAluguel = new Aluguel
            {
                FerramentaId = dadosAluguel.FerramentaId,
                UsuarioId = dadosAluguel.UsuarioId,
                DataInicio = dadosAluguel.DataInicio,
                DataFim = dadosAluguel.DataFim,
                Status = StatusAluguel.AguardandoPagamento
            };

            // Salvar no banco
            _aluguelDbContext.Alugueis.Add(novoAluguel);

            int resultadoInsercao = await _aluguelDbContext.SaveChangesAsync();

            // Retornar resposta da API
            if (resultadoInsercao > 0)
                return Created("", novoAluguel);

            return BadRequest("O aluguel não foi registrado!");
        }
    }
}