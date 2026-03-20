using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using uc10_Locatem.Data;
using uc10_Locatem.Enum;
using uc10_Locatem.Model;
using uc10_Locatem.Model.DTO;


[ApiController]
[Route("api/[controller]")]
public class AluguelController : ControllerBase
{
    // ================= BANCO DE DADOS =================

    // Guarda a instância do banco
    public readonly AppDbContext _aluguelDbContext;

    // Injeção de dependência (o sistema entrega o banco pronto)
    public AluguelController(AppDbContext context)
    {
        _aluguelDbContext = context;
    }


    // ================= GET - LISTAR ALUGUÉIS =================

    [HttpGet("GetAllAlugueis")]
    public async Task<IActionResult> GetAllAlugueis()
    {
        // Busca todos os registros da tabela Aluguel
        List<Aluguel> listaAlugueis = await _aluguelDbContext.Alugueis.ToListAsync();

        // Loop para aplicar regras automáticas de negócio
        foreach (var aluguel in listaAlugueis)
        {
            // ================= TIMEOUT DE PAGAMENTO =================
            // Se passou 24h e não pagou → cancela automaticamente
            if (aluguel.Status == StatusAluguel.AguardandoPagamento &&
                DateTime.Now > aluguel.DataCriacao.AddHours(24))
            {
                aluguel.Status = StatusAluguel.Cancelado;
            }

            // ================= ATRASO AUTOMÁTICO =================
            // Se passou da data final e ainda está ativo → vira atrasado
            if (DateTime.Now > aluguel.DataFim && aluguel.Status == StatusAluguel.Ativo)
            {
                aluguel.Status = StatusAluguel.Atrasado;
            }
        }

        // Salva possíveis alterações feitas no loop
        await _aluguelDbContext.SaveChangesAsync();

        // Retorna lista em JSON
        return Ok(listaAlugueis);

        /*
        FLUXO:
        1. Recebe GET
        2. Consulta banco
        3. Aplica regras de negócio
        4. Retorna JSON
        */
    }


    // ================= POST - CRIAR ALUGUEL =================

    [HttpPost("CriarAluguel")]
    public async Task<IActionResult> CriarAluguel([FromBody] CriarAluguelDTO dadosAluguel)
    {
        // ================= VALIDAÇÃO DO DTO =================

        // Se faltar campo obrigatório → erro 400
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // ================= VALIDAÇÃO DE EXISTÊNCIA =================

        // Verifica se a ferramenta existe no banco
        Ferramenta? ferramentaEncontrada =
            await _aluguelDbContext.Ferramenta
            .FirstOrDefaultAsync(f => f.FerramentaId == dadosAluguel.FerramentaId);

        if (ferramentaEncontrada == null)
        {
            return BadRequest($"Ferramenta com ID {dadosAluguel.FerramentaId} não encontrada");
        }

        // ================= VALIDAÇÃO DE DATAS =================

        if (dadosAluguel.DataFim <= dadosAluguel.DataInicio)
        {
            return BadRequest("Data final deve ser maior que a inicial");
        }

        // ================= VALIDAÇÃO DE USUÁRIOS =================

        if (dadosAluguel.LocadorId == dadosAluguel.LocatarioId)
        {
            return BadRequest("Locador e locatário não podem ser a mesma pessoa.");
        }

        // ================= VERIFICAÇÃO DE CONFLITO =================

        // Verifica se já existe aluguel ativo no mesmo período
        bool conflito = await _aluguelDbContext.Alugueis.AnyAsync(a =>
            a.FerramentaId == dadosAluguel.FerramentaId &&
            a.Status != StatusAluguel.Cancelado &&
            (
                dadosAluguel.DataInicio < a.DataFim &&
                dadosAluguel.DataFim > a.DataInicio
            )
        );

        if (conflito)
        {
            return BadRequest("Ferramenta já está alugada nesse período.");
        }

        // ================= CRIAÇÃO DO OBJETO =================

        Aluguel novoAluguel = new Aluguel
        {
            FerramentaId = dadosAluguel.FerramentaId,
            LocadorId = dadosAluguel.LocadorId,
            LocatarioId = dadosAluguel.LocatarioId,
            DataInicio = dadosAluguel.DataInicio,
            DataFim = dadosAluguel.DataFim,

            // Começa aguardando pagamento
            Status = StatusAluguel.AguardandoPagamento
        };

        // ================= CÁLCULO DE VALOR =================

        int dias = (dadosAluguel.DataFim - dadosAluguel.DataInicio).Days;

        if (dias <= 0)
        {
            return BadRequest("O período do aluguel deve ter pelo menos 1 dia.");
        }

        // Valor = diária * quantidade de dias
        novoAluguel.ValorTotal = ferramentaEncontrada.PrecoDiaria * dias;

        // ================= SALVAR NO BANCO =================

        _aluguelDbContext.Alugueis.Add(novoAluguel);

        int resultadoInsercao = await _aluguelDbContext.SaveChangesAsync();

        // ================= RESPOSTA =================

        if (resultadoInsercao > 0)
            return Created("", novoAluguel);

        return BadRequest("O aluguel não foi registrado!");
    }


    // ================= PAGAMENTO =================

    [HttpPatch("{id}/pagar")]
    public async Task<IActionResult> RegistrarPagamento(int id)
    {
        var aluguel = await _aluguelDbContext.Alugueis.FindAsync(id);

        if (aluguel == null)
            return NotFound("Aluguel não encontrado.");

        // Só permite pagar se estiver aguardando pagamento
        if (aluguel.Status != StatusAluguel.AguardandoPagamento)
            return BadRequest("Pagamento já realizado ou status inválido.");

        // Atualiza status
        aluguel.Status = StatusAluguel.PagoAguardandoRetirada;

        await _aluguelDbContext.SaveChangesAsync();

        return Ok("Pagamento confirmado.");
    }


    // ================= INICIAR ALUGUEL =================

    [HttpPatch("{id}/iniciar")]
    public async Task<IActionResult> IniciarAluguel(int id)
    {
        var aluguel = await _aluguelDbContext.Alugueis.FindAsync(id);

        if (aluguel == null)
            return NotFound("Aluguel não encontrado.");

        // Só pode iniciar se já foi pago
        if (aluguel.Status != StatusAluguel.PagoAguardandoRetirada)
            return BadRequest("O aluguel ainda não está pronto para iniciar.");

        // Não pode iniciar antes da data
        if (DateTime.Now < aluguel.DataInicio)
            return BadRequest("Ainda não está no período de retirada.");

        aluguel.Status = StatusAluguel.Ativo;

        await _aluguelDbContext.SaveChangesAsync();

        return Ok("Aluguel iniciado.");
    }


    // ================= FINALIZAR ALUGUEL =================

    [HttpPatch("{id}/finalizar")]
    public async Task<IActionResult> FinalizarAluguel(int id, [FromBody] FinalizarAluguelDTO dados)
    {
        var aluguel = await _aluguelDbContext.Alugueis.FindAsync(id);

        if (aluguel == null)
            return NotFound("Aluguel não encontrado.");

        // Impede alterar aluguel já encerrado
        if (aluguel.Status == StatusAluguel.Finalizado ||
            aluguel.Status == StatusAluguel.Cancelado)
        {
            return BadRequest("Este aluguel já foi encerrado.");
        }

        // Só permite finalizar se estiver ativo ou atrasado
        if (aluguel.Status != StatusAluguel.Ativo &&
            aluguel.Status != StatusAluguel.Atrasado)
        {
            return BadRequest("Somente aluguéis ativos ou atrasados podem ser finalizados.");
        }

        decimal valorFinal = aluguel.ValorTotal;

        // ================= MULTA POR ATRASO =================

        if (DateTime.Now > aluguel.DataFim)
        {
            int diasAtraso = (DateTime.Now - aluguel.DataFim).Days;

            if (diasAtraso > 0)
            {
                decimal multaPorDia = 10; // depois pode vir do banco
                valorFinal += diasAtraso * multaPorDia;
            }
        }

        // ================= TAXA DE AVARIA =================

        if (dados.HouveAvaria)
        {
            decimal taxaAvaria = 50; // valor fixo (pode virar config depois)
            valorFinal += taxaAvaria;
        }

        // Atualiza valores e status
        aluguel.ValorTotal = valorFinal;
        aluguel.Status = StatusAluguel.Finalizado;

        await _aluguelDbContext.SaveChangesAsync();

        return Ok(new
        {
            mensagem = "Aluguel finalizado com sucesso.",
            valorFinal
        });
    }
}