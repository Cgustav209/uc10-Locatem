using Microsoft.AspNetCore.Authorization;
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
        private const int duracao_maxima_dias = 30;

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

        // criar aluguel (manual/admin/teste)
        [HttpPost("CriarAluguel")]
        // [FromBody] = os dados virão no corpo da requisição em JSON.
        public async Task<IActionResult> CriarAluguel([FromBody] CriarAluguelDTO dadosAluguel)
        {
            // Validação do DTO. Se algum campo obrigatório faltar, a API responde: 400 Bad Request
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var usuario = await _aluguelDbContext.Usuario
            .FirstOrDefaultAsync(u => u.Id == dadosAluguel.UsuarioId);

            if (usuario == null)
            {
                return NotFound("Usuário não encontrado.");
            }

            if (usuario.Bloqueado)
            {
                return BadRequest("Usuário bloqueado por inadimplência.");
            }

            // validação de data
            if (dadosAluguel.DataInicio >= dadosAluguel.DataFim)
            {
                return BadRequest("A data de início deve ser menor que a data de fim.");
            }

            // bloquear datas no passado
            DateTime hoje = DateTime.UtcNow.Date;

            if (dadosAluguel.DataInicio < hoje)
            {
                return BadRequest("Não é permitido criar aluguel com data no passado.");
            }

            // limita duração máxima
            int duracao = (dadosAluguel.DataFim - dadosAluguel.DataInicio).Days + 1;

            if (duracao > duracao_maxima_dias)
            {
                return BadRequest($"O aluguel não pode exceder {duracao_maxima_dias} dias.");
            }

            // Verifica se a ferramenta existe. Isso evita criar aluguel de algo que não existe no banco.
            Ferramenta? ferramentaEncontrada = await _aluguelDbContext.Ferramenta.FirstOrDefaultAsync(f => f.FerramentaId == dadosAluguel.FerramentaId);

            if (ferramentaEncontrada == null)
            {
               return BadRequest($"Ferramenta com ID {dadosAluguel.FerramentaId} não encontrada");
            }

            bool conflitoPeriodo = await _aluguelDbContext.Alugueis.AnyAsync(a =>
                a.FerramentaId == dadosAluguel.FerramentaId &&
                (a.Status == StatusAluguel.Ativo || a.Status == StatusAluguel.AguardandoPagamento) &&
                dadosAluguel.DataInicio <= a.DataFim &&
                dadosAluguel.DataFim >= a.DataInicio
            );

            if (conflitoPeriodo)
            {
                return BadRequest("A ferramenta já está alugada nesse período.");
            }

            // Criar objeto Aluguel
            // Esse objeto representa uma linha na tabela Aluguel.
            Aluguel novoAluguel = new Aluguel
            {
                FerramentaId = dadosAluguel.FerramentaId,
                UsuarioId = dadosAluguel.UsuarioId,
                DataInicio = dadosAluguel.DataInicio,
                DataFim = dadosAluguel.DataFim,
                Status = StatusAluguel.AguardandoPagamento,
                ValorCaucao = ferramentaEncontrada.Caucao,
                CaucaoRetida = false
            };

            // Salvar no banco
            _aluguelDbContext.Alugueis.Add(novoAluguel);

            int resultadoInsercao = await _aluguelDbContext.SaveChangesAsync();

            // Retornar resposta da API
            if (resultadoInsercao > 0)
                return Created("", novoAluguel);

            return BadRequest("O aluguel não foi registrado!");
        }


        [Authorize] //[Authorize(Roles = "Locador")]
        // cria o aluguel a patir de uma reserva (fluxo real do sistema)
        [HttpPost("criar-por-reserva/{reservaId}")]
        public async Task<IActionResult> CriarPorReserva(int reservaId)
        {
            var usuarioId = int.Parse(User.FindFirst("id").Value);

            var usuario = await _aluguelDbContext.Usuario
                .FirstOrDefaultAsync(u => u.Id == usuarioId);

            if (usuario == null)
            {
                return NotFound("Usuário não encontrado.");
            }

            if (usuario.Bloqueado)
            {
                return BadRequest("Usuário bloqueado por inadimplência.");
            }

            var reserva = await _aluguelDbContext.Reservas.Include(r => r.Ferramenta).FirstOrDefaultAsync(r => r.Id == reservaId);

            // valida null
            if (reserva == null)
            {
                return NotFound("Reserva não encontrada.");
            }

            bool conflitoPeriodo = await _aluguelDbContext.Alugueis.AnyAsync(a =>
                a.FerramentaId == reserva.FerramentaId &&
                (a.Status == StatusAluguel.Ativo || a.Status == StatusAluguel.AguardandoPagamento) &&
                reserva.DataInicio <= a.DataFim &&
                reserva.DataFim >= a.DataInicio
            );

            if (conflitoPeriodo)
            {
                return BadRequest("A ferramenta já está alugada nesse período.");
            }

            // bloquear reserva com data no passado
            if (reserva.DataInicio < DateTime.UtcNow.Date)
            {
                return BadRequest("A reserva possui data no passado.");
            }

            // LLimitar duração da reserva
            int duracao = (reserva.DataFim - reserva.DataInicio).Days;

            if (duracao > duracao_maxima_dias)
            {
                return BadRequest($"A reserva excede o limite de {duracao_maxima_dias} dias.");
            }

            if (reserva.Status != StatusReserva.Aceita)
            {
                return BadRequest("A reserva não está aceita.");
            }

            if (reserva.Ferramenta.UsuarioId != usuarioId)
            {
                return Forbid("Você não tem permissão para criar este aluguel.");
            }

            // Evita duplicidade
            bool jaExiste = await _aluguelDbContext.Alugueis.AnyAsync(a => a.ReservaId == reserva.Id);

            if (jaExiste) 
            { 
                return BadRequest("Já existe um aluguel para essa reserva.");
            }

            // copia os dados da reserva
            var aluguel = new Aluguel
            {
                FerramentaId = reserva.FerramentaId,
                UsuarioId = reserva.UsuarioId, 
                DataInicio = reserva.DataInicio,
                DataFim = reserva.DataFim,
                Status = StatusAluguel.AguardandoPagamento,
                ReservaId = reserva.Id,
                ValorCaucao = reserva.Ferramenta.Caucao,
                CaucaoRetida = false,
            };

            _aluguelDbContext.Alugueis.Add(aluguel);

            // Atualiza reserva
            reserva.Status = StatusReserva.ConvertidaEmAluguel;

            await _aluguelDbContext.SaveChangesAsync();

            return Created("", aluguel);
        }

        [Authorize]
        [HttpPost("pagar/{aluguelId}")]
        public async Task<IActionResult> PagarAluguel(int aluguelId)
        {
            var usuarioId = int.Parse(User.FindFirst("id").Value);

            var usuario = await _aluguelDbContext.Usuario.FirstOrDefaultAsync(u => u.Id == usuarioId);

            if (usuario == null)
            {
                return NotFound("Usuário não encontrado.");
            }

            if (usuario.Bloqueado)
            {
                return BadRequest("Usuário bloqueado.");
            }

            var aluguel = await _aluguelDbContext.Alugueis.Include(a => a.FerramentaId).FirstOrDefaultAsync(a => a.Id == aluguelId);

            if (aluguel == null)
            {
                return NotFound("Aluguel não encontrado.");
            }

            // segurança: só quem alugou pode pagar
            if (aluguel.UsuarioId != usuarioId)
            {
                return Forbid("Você não tem permissão para pagar este aluguel.");
            }

            // validação de status (sem isso alguém poderia: pagar 2x, ativar aluguel cancelado, quebrar o sistema)
            if (aluguel.Status != StatusAluguel.AguardandoPagamento)
            {
                return BadRequest("O aluguel não está aguardando pagamento.");
            }

            // pega o valor da diária
            decimal precoDia = aluguel.Ferramenta.Diaria;

            // calcula valor do aluguel
            decimal valorAluguel = CalcularValorAluguel(
                aluguel.DataInicio,
                aluguel.DataFim,
                precoDia
            );

            // pega caução
            decimal caucao = aluguel.ValorCaucao;

            // total pago
            decimal valorTotalPagamento = valorAluguel + caucao;

            // salva só o valor do aluguel (sem caução)
            aluguel.ValorTotal = valorAluguel;

            // registra quanto foi pago no total (aluguel + caução)
            aluguel.ValorDevolvido = 0; // ainda não devolveu nada

            // simulação de pagamento (fake)
            aluguel.Status = StatusAluguel.Ativo;

            // atualização de status
            await _aluguelDbContext.SaveChangesAsync();

            return Ok(new
            {
                mensagem = "Pagamento aprovado",
                aluguelId = aluguel.Id,
                valorAluguel,
                caucao,
                valorTotalPagamento,
                status = aluguel.Status
            });
        }


        // locatário solicita a finalização do aluguel
        [Authorize]
        [HttpPost("solicitar-finalizacao/{aluguelId}")]
        public async Task<IActionResult> SolicitarFinalizacao(int aluguelId)
        {
            var usuarioId = int.Parse(User.FindFirst("id").Value);

            var usuario = await _aluguelDbContext.Usuario.FirstOrDefaultAsync(u => u.Id == usuarioId);

            if (usuario == null)
            {
                return NotFound("Usuário não encontrado.");
            }

            if (usuario.Bloqueado)
            {
                return BadRequest("Usuário bloqueado.");
            }

            var aluguel = await _aluguelDbContext.Alugueis
                .FirstOrDefaultAsync(a => a.Id == aluguelId);

            if (aluguel == null)
                return NotFound("Aluguel não encontrado.");

            // só quem alugou pode solicitar
            if (aluguel.UsuarioId != usuarioId)
            { 
                return Forbid("Você não tem permissão.");
            }

            if (aluguel.Status != StatusAluguel.Ativo)
            {
                return BadRequest("Só é possível solicitar finalização de um aluguel ativo.");
            }

            aluguel.Status = StatusAluguel.AguardandoConfirmacao;

            await _aluguelDbContext.SaveChangesAsync();

            return Ok("Solicitação de finalização enviada.");
        }


        // locador confirma a finalização do aluguel após checar que a ferramenta chegou em perfeitas condições
        [Authorize]
        [HttpPost("confirmar-finalizacao/{aluguelId}")]
        public async Task<IActionResult> ConfirmarFinalizacao(int aluguelId)
        {
            var usuarioId = int.Parse(User.FindFirst("id").Value);

            // caso o locador não devolva a caução, entregar uma ferramenta diferente do anúncio (ex: quebrada), resusar finalização sem motivoo locador é bloqueado
            var usuario = await _aluguelDbContext.Usuario.FirstOrDefaultAsync(u => u.Id == usuarioId);

            if (usuario == null)
            {
                return NotFound("Usuário não encontrado.");
            }

            if (usuario.Bloqueado)
            {
                return BadRequest("Usuário bloqueado.");
            }

            var aluguel = await _aluguelDbContext.Alugueis
                .Include(a => a.FerramentaId)
                .FirstOrDefaultAsync(a => a.Id == aluguelId);

            if (aluguel == null)
            {
                return NotFound("Aluguel não encontrado.");
            }

            // só o dono da ferramenta pode confirmar
            if (aluguel.Ferramenta.UsuarioId != usuarioId)
            {
                return Forbid("Você não tem permissão.");
            }

            if (aluguel.Status != StatusAluguel.AguardandoConfirmacao)
            {
                return BadRequest("O aluguel não está aguardando confirmação.");
            }

            // CÁLCULOS
            decimal precoDia = aluguel.Ferramenta.Diaria;

            decimal valorAluguel = CalcularValorAluguel(
                aluguel.DataInicio,
                aluguel.DataFim,
                precoDia
            );

            decimal multa = CalcularMulta(aluguel.DataFim, precoDia);

            decimal valorFinal = valorAluguel + multa;

            // salva no aluguel
            aluguel.ValorTotal = valorFinal;

            // devolve caução (tudo certo)
            aluguel.CaucaoRetida = false;

            // devolve caução integral
            aluguel.ValorDevolvido = aluguel.ValorCaucao;

            // finaliza de verdade
            aluguel.Status = StatusAluguel.Finalizado;

            await _aluguelDbContext.SaveChangesAsync();

            return Ok(new
            {
                mensagem = "Aluguel finalizado com sucesso.",
                aluguelId = aluguel.Id,
                valorAluguel,
                multa,
                valorFinal
            });
        }


        // locador recusa a finalização do aluguel após checar que a ferramenta não chegou ou chegou com defeitos
        [Authorize]
        [HttpPost("recusar-finalizacao/{aluguelId}")]
        public async Task<IActionResult> RecusarFinalizacao(int aluguelId)
        {
            var usuarioId = int.Parse(User.FindFirst("id").Value);

            // caso o locador não devolva a caução, entregar uma ferramenta diferente do anúncio (ex: quebrada), resusar finalização sem motivoo locador é bloqueado
            var usuario = await _aluguelDbContext.Usuario.FirstOrDefaultAsync(u => u.Id == usuarioId);

            if (usuario == null)
            {
                return NotFound("Usuário não encontrado.");
            }

            if (usuario.Bloqueado)
            {
                return BadRequest("Usuário bloqueado.");
            }

            var aluguel = await _aluguelDbContext.Alugueis
                .Include(a => a.Ferramenta)
                .FirstOrDefaultAsync(a => a.Id == aluguelId);

            if (aluguel == null)
                return NotFound("Aluguel não encontrado.");

            // só o dono da ferramenta pode recusar
            if (aluguel.Ferramenta.UsuarioId != usuarioId)
                return Forbid("Você não tem permissão.");

            // só pode recusar se estiver aguardando confirmação
            if (aluguel.Status != StatusAluguel.AguardandoConfirmacao)
                return BadRequest("O aluguel não está aguardando confirmação.");

            // muda o status (problema detectado)
            aluguel.Status = StatusAluguel.Atrasado;

            // caução retida (problema detectado)
            aluguel.CaucaoRetida = true;

            aluguel.ValorDevolvido = 0;

            await _aluguelDbContext.SaveChangesAsync();

            return Ok(new
            {
                mensagem = "Finalização recusada. O aluguel foi marcado como atrasado.",
                aluguelId = aluguel.Id,
                status = aluguel.Status
            });
        }


        // ================= CÁLCULOS =================

        // calcula valor base do aluguel
        private decimal CalcularValorAluguel(DateTime inicio, DateTime fim, decimal precoDia)
        {
            // +1 pq a diária conta o dia inicial
            int dias = (fim - inicio).Days + 1;

            if (dias <= 0)
                dias = 1;

            return dias * precoDia;
        }

        // calcula multa por atraso (1-3 dias de atraso = multa de 1.2x; 4-7 dias de atraso = multa de 1.5x; 8+ dias de atraso = multa de 2.0x)
        private decimal CalcularMulta(DateTime dataFim, decimal precoDia)
        {
            if (DateTime.UtcNow <= dataFim)
                return 0;

            int diasAtraso = (int)Math.Ceiling((DateTime.UtcNow - dataFim).TotalDays);

            decimal fatorMulta;
            if (diasAtraso <= 3)
                fatorMulta = 1.2m;
            else if (diasAtraso <= 7)
                fatorMulta = 1.5m;
            else
                fatorMulta = 2.0m;

            return diasAtraso * precoDia * fatorMulta;
        }
    }
}