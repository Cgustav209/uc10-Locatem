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

        //[Authorize]
        [HttpPut("cancelar/{id}")]
        public async Task<IActionResult> CancelarReserva(int id)
        {
            // Aqui estou pegando o id do usuário logado para verificar se ele é o dono da reserva que está tentando cancelar
            var usuarioId = int.Parse(User.FindFirst("id").Value);

            // Chama o método CancelarReserva do services de reservas, passando o id da reserva e o id do usuário logado
            var (sucesso, mensagem) = await _reservaService.CancelarReserva(id, usuarioId);

            // Se o cancelamento não for bem-sucedido, retorna um erro 400 com a mensagem de erro
            if (!sucesso)
                return BadRequest(mensagem);

            // Se o cancelamento for bem-sucedido, retorna um status 200 com a mensagem de sucesso
            return Ok(mensagem);

        }

        //[Authorize]
        [HttpPut("aceitar/{id}")]
        public async Task<IActionResult> AceitarReserva(int id)
        {
            // Aqui estou pegando o id do usuário logado para verificar se ele é o dono da ferramenta relacionada à reserva que está tentando aceitar
            var usuarioId = int.Parse(User.FindFirst("id").Value);

            // Chama o método AceitarReserva do serviço de reservas, passando o id da reserva e o id do usuário logado
            var (sucesso, mensagem) = await _reservaService.AceitarReserva(id, usuarioId);

            // Se a aceitação não for bem-sucedida, retorna um erro 400 com a mensagem de erro
            if (!sucesso)
                return BadRequest(mensagem);
            // Se a aceitação for bem-sucedida, retorna um status 200 com a mensagem de sucesso
            return Ok(mensagem);

        }

        //[Authorize]
        [HttpPut("recusar/{id}")]
        public async Task<IActionResult> RecusarReserva(int id)
        {
            // Aqui estou pegando o id do usuário logado para verificar se ele é o dono da ferramenta relacionada à reserva que está tentando recusar
            var usuarioId = int.Parse(User.FindFirst("id").Value);
            // Chama o método RecusarReserva do serviço de reservas, passando o id da reserva e o id do usuário logado
            var (sucesso, mensagem) = await _reservaService.RecusarReserva(id, usuarioId);
            // Se a recusa não for bem-sucedida, retorna um erro 400 com a mensagem de erro
            if (!sucesso)
                return BadRequest(mensagem);
            // Se a recusa for bem-sucedida, retorna um status 200 com a mensagem de sucesso
            return Ok(mensagem);
        }

        //[Authorize]
        [HttpGet("minhasReservas")]
        public async Task<IActionResult> MinhasReservas()
        {
            // Aqui estou pegando o id do usuário logado para buscar as reservas relacionadas a ele
            var usuarioId = int.Parse(User.FindFirst("id").Value);

            // Chama o método ListarReservasDoUsuario do service de reservas, passando o id do usuário logado para obter a lista de reservas relacionadas a ele
            var reservas = await _reservaService.ListarReservasDoUsuario(usuarioId);

            // Retorna a lista de reservas do usuário logado
            return Ok(reservas);
        }

        //[Authorize]
        [HttpGet("reservasRecebidas")]
        public async Task<IActionResult> ReservasRecebidas()
        {
            // Aqui estou pegando o id do usuário logado para buscar as reservas relacionadas às ferramentas que ele possui
            var usuarioId = int.Parse(User.FindFirst("id").Value);
            // Chama o método ListarReservasRecebidas do service de reservas, passando o id do usuário logado para obter a lista de reservas relacionadas às ferramentas que ele possui
            var reservas = await _reservaService.ListarResrvasRecebidas(usuarioId);
            // Retorna a lista de reservas recebidas para as ferramentas do usuário logado
            return Ok(reservas);
        }
    }
}
