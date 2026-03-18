using uc10_Locatem.Data;
using Microsoft.AspNetCore.Mvc;
using uc10_Locatem.Model.DTO;
using uc10_Locatem.Enum;
using Microsoft.EntityFrameworkCore;
using uc10_Locatem.Model;
using uc10_Locatem.Services;

namespace uc10_Locatem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservasController : ControllerBase
    {

        private readonly ReservaService _reservaService;

        // Injeção de dependência do ReservaService para usar a lógica de negócios relacionada às reservas
        public ReservasController(AppDbContext context, ReservaService reservaService)
        {
            _ReservaDbContext = context;
            _reservaService = reservaService;
        }

        // Injeção de dependência do AppDbContext para acessar o banco de dados
        private readonly AppDbContext _ReservaDbContext;

        public ReservasController(AppDbContext context)
        {
            _ReservaDbContext = context;
        }
        //====================================================

        [HttpPost("criarReserva")]
        public async Task<IActionResult> CriarReserva([FromBody] CriarReservaDTO dadosReserva)
        {
            // Verificar se a ferramenta existe

            var produto = await _ReservaDbContext.Produto.FindAsync(dadosReserva.FerramentaId);

            // Se a ferramenta não existir, retornar um erro 404, que o  de não encontrado.

            if (produto == null)
            {
                return NotFound("Produto não encontrado.");
            }

            // Impede a pessoa de reservar sua própria ferramenta

            if (produto.UsuarioId == dadosReserva.UsuarioId)
            {
                return BadRequest("Não é possível reservar sua própria ferramenta.");

            }
            // Verificar se as datas são válidas (data de início deve ser anterior à data de fim)

            if (dadosReserva.DataInicio >= dadosReserva.DataFim)
            {
                return BadRequest("A data de início deve ser anterior à data de fim.");
            }

            // Verificar se há conflito de reservas para a mesma ferramenta no período solicitado

            var conflito = await _ReservaDbContext.Reserva.AnyAsync(r => r.FerramentaId == dadosReserva.FerramentaId && r.Status == StatusReserva.Aceita && dadosReserva.DataInicio <= r.DataFim && dadosReserva.DataFim >= r.DataInicio
            );

            // Se houver conflito, retornar um erro 

            if (conflito)
            {
                return BadRequest("A ferramenta já está reservada para o período selecionado.");
            }


            // Serve para criar uma nova reserva com os dados fornecidos
            var reserva = new Reserva
            {
                FerramentaId = dadosReserva.FerramentaId,
                UsuarioId = dadosReserva.UsuarioId,
                DataInicio = dadosReserva.DataInicio,
                DataFim = dadosReserva.DataFim,
                Status = StatusReserva.Pendente,
                DataCriacao = DateTime.Now
            };

            _ReservaDbContext.Reserva.Add(reserva);
            await _ReservaDbContext.SaveChangesAsync();

            return Ok(reserva);
        }

        [HttpPut("cancelar/{id}")]
        public async Task<IActionResult> CancelarReserva(int id, [FromBody] int usuarioId)
        {
            var (sucesso, mensagem) = await _reservaService.CancelarReserva(id, usuarioId);
            if (!sucesso)
            {
                return BadRequest(mensagem);
            }
            return Ok(mensagem);


        }

    }
}
