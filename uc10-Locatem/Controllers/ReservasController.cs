using uc10_Locatem.Data;
using Microsoft.AspNetCore.Mvc;
using uc10_Locatem.Model.DTO;
using uc10_Locatem.Enum;

namespace uc10_Locatem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservasController : ControllerBase
    {
        // Injeção de dependência do AppDbContext para acessar o banco de dados
        private readonly AppDbContext _ReservaDbContext;

        public ReservasController(AppDbContext context)
        {
            _ReservaDbContext = context;
        }
        //====================================================

        [HttpPost("")]
        public async Task<IActionResult> CriarReserva([FromBody] CriarReservaDTO dadosReserva )
        {
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
        }
    }

}
