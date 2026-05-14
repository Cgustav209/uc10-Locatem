using Microsoft.AspNetCore.Mvc;
using uc10_Locatem.Model;

namespace uc10_Locatem.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class Pagamentocs : ControllerBase
    {
        [HttpGet]
        public ActionResult<List<Pagamentocs>> GetPagamento()
        {
            return Ok();
        }


        [HttpPost("SalvarBanco")]

        public ActionResult CreatePagamento(Pagamento pagamento) 
        { 
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            pagamento.Id = new Random().Next(1, 1000); // Simula a geração de um ID único
            pagamento.DataPagamento = DateTime.Now; // Define a data de pagamento como a data atual
            pagamento.ValorTotal = pagamento.ValorTotal; // Define o valor total como o valor do pagamento
            pagamento.Status = pagamento.Status; // Define o status como "Pendente"
            pagamento.MetodoPagamento = pagamento.MetodoPagamento; // Define o método de pagamento como "Cartão de Crédito"
            pagamento.UsuarioId = pagamento.UsuarioId; // Simula um ID de usuário
            pagamento.AluguelId = pagamento.AluguelId; // Simula um ID de aluguel
            pagamento.Valor = pagamento.Valor; // Simula um valor de pagamento
            pagamento.DataPagamento = DateTime.Now; // Define a data de pagamento como a data atual

            return Ok(pagamento);


        }
       

    }
}
