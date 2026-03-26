using Microsoft.EntityFrameworkCore;
using uc10_Locatem.Data;
using uc10_Locatem.Enum;
using uc10_Locatem.Model;
using uc10_Locatem.Model.DTO;

namespace uc10_Locatem.Services
{
    // aqui vai tudo o que é regra do sistema
    public class AluguelService
    {
        // ================================
        // DEPENDÊNCIA DO BANCO
        // ================================
        private readonly AppDbContext _context;

        // Regra de negócio: duração máxima permitida
        private const int duracao_maxima_dias = 30;

        // ================================
        // CONSTRUTOR
        // ================================
        public AluguelService(AppDbContext context)
        {
            _context = context;
        }

        // ================================
        // CRIAR ALUGUEL MANUALMENTE
        // ================================
        public async Task<ResultadoServiceAluguelDTO> CriarAluguelAsync(CriarAluguelDTO dadosAluguel)
        {
            var usuario = await _context.Usuario
                .FirstOrDefaultAsync(u => u.Id == dadosAluguel.UsuarioId);

            if (usuario == null)
            {
                return new ResultadoServiceAluguelDTO
                {
                    Sucesso = false,
                    StatusCode = 404,
                    Mensagem = "Usuário não encontrado."
                };
            }

            if (usuario.Bloqueado)
            {
                return new ResultadoServiceAluguelDTO
                {
                    Sucesso = false,
                    StatusCode = 400,
                    Mensagem = "Usuário bloqueado por inadimplência."
                };
            }

            if (dadosAluguel.DataInicio >= dadosAluguel.DataFim)
            {
                return new ResultadoServiceAluguelDTO
                {
                    Sucesso = false,
                    StatusCode = 400,
                    Mensagem = "A data de início deve ser menor que a data de fim."
                };
            }

            DateTime hoje = DateTime.UtcNow.Date;

            if (dadosAluguel.DataInicio < hoje)
            {
                return new ResultadoServiceAluguelDTO
                {
                    Sucesso = false,
                    StatusCode = 400,
                    Mensagem = "Não é permitido criar aluguel com data no passado."
                };
            }

            int duracao = (dadosAluguel.DataFim - dadosAluguel.DataInicio).Days + 1;

            if (duracao > duracao_maxima_dias)
            {
                return new ResultadoServiceAluguelDTO
                {
                    Sucesso = false,
                    StatusCode = 400,
                    Mensagem = $"O aluguel não pode exceder {duracao_maxima_dias} dias."
                };
            }

            Ferramenta? ferramentaEncontrada = await _context.Ferramenta
                .FirstOrDefaultAsync(f => f.FerramentaId == dadosAluguel.FerramentaId);

            if (ferramentaEncontrada == null)
            {
                return new ResultadoServiceAluguelDTO
                {
                    Sucesso = false,
                    StatusCode = 400,
                    Mensagem = $"Ferramenta com ID {dadosAluguel.FerramentaId} não encontrada."
                };
            }

            bool conflitoPeriodo = await _context.Alugueis.AnyAsync(a =>
                a.FerramentaId == dadosAluguel.FerramentaId &&
                (a.Status == StatusAluguel.Ativo || a.Status == StatusAluguel.AguardandoPagamento) &&
                dadosAluguel.DataInicio <= a.DataFim &&
                dadosAluguel.DataFim >= a.DataInicio
            );

            if (conflitoPeriodo)
            {
                return new ResultadoServiceAluguelDTO
                {
                    Sucesso = false,
                    StatusCode = 400,
                    Mensagem = "A ferramenta já está alugada nesse período."
                };
            }

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

            _context.Alugueis.Add(novoAluguel);
            int resultadoInsercao = await _context.SaveChangesAsync();

            if (resultadoInsercao > 0)
            {
                return new ResultadoServiceAluguelDTO
                {
                    Sucesso = true,
                    StatusCode = 201,
                    Mensagem = "Aluguel criado com sucesso.",
                    Dados = novoAluguel
                };
            }

            return new ResultadoServiceAluguelDTO
            {
                Sucesso = false,
                StatusCode = 400,
                Mensagem = "O aluguel não foi registrado."
            };
        }

        // ================================
        // CRIAR ALUGUEL A PARTIR DE RESERVA
        // ================================
        public async Task<ResultadoServiceAluguelDTO> CriarPorReservaAsync(int reservaId, int usuarioId)
        {
            var usuario = await _context.Usuario
                .FirstOrDefaultAsync(u => u.Id == usuarioId);

            if (usuario == null)
            {
                return new ResultadoServiceAluguelDTO
                {
                    Sucesso = false,
                    StatusCode = 404,
                    Mensagem = "Usuário não encontrado."
                };
            }

            if (usuario.Bloqueado)
            {
                return new ResultadoServiceAluguelDTO
                {
                    Sucesso = false,
                    StatusCode = 400,
                    Mensagem = "Usuário bloqueado."
                };
            }

            var reserva = await _context.Reserva
                .Include(r => r.Ferramenta)
                .FirstOrDefaultAsync(r => r.Id == reservaId);

            if (reserva == null)
            {
                return new ResultadoServiceAluguelDTO
                {
                    Sucesso = false,
                    StatusCode = 404,
                    Mensagem = "Reserva não encontrada."
                };
            }

            bool conflitoPeriodo = await _context.Alugueis.AnyAsync(a =>
                a.FerramentaId == reserva.FerramentaId &&
                (a.Status == StatusAluguel.Ativo || a.Status == StatusAluguel.AguardandoPagamento) &&
                reserva.DataInicio <= a.DataFim &&
                reserva.DataFim >= a.DataInicio
            );

            if (conflitoPeriodo)
            {
                return new ResultadoServiceAluguelDTO
                {
                    Sucesso = false,
                    StatusCode = 400,
                    Mensagem = "A ferramenta já está alugada nesse período."
                };
            }

            int duracao = (reserva.DataFim - reserva.DataInicio).Days + 1;

            if (duracao > duracao_maxima_dias)
            {
                return new ResultadoServiceAluguelDTO
                {
                    Sucesso = false,
                    StatusCode = 400,
                    Mensagem = $"A reserva excede o limite de {duracao_maxima_dias} dias."
                };
            }

            if (reserva.Status != StatusReserva.Aceita)
            {
                return new ResultadoServiceAluguelDTO
                {
                    Sucesso = false,
                    StatusCode = 400,
                    Mensagem = "A reserva não está aceita."
                };
            }

            if (reserva.Ferramenta.UsuarioId != usuarioId)
            {
                return new ResultadoServiceAluguelDTO
                {
                    Sucesso = false,
                    StatusCode = 403,
                    Mensagem = "Você não tem permissão para criar este aluguel."
                };
            }

            bool jaExiste = await _context.Alugueis.AnyAsync(a => a.ReservaId == reserva.Id);

            if (jaExiste)
            {
                return new ResultadoServiceAluguelDTO
                {
                    Sucesso = false,
                    StatusCode = 400,
                    Mensagem = "Já existe um aluguel para essa reserva."
                };
            }

            var aluguel = new Aluguel
            {
                FerramentaId = reserva.FerramentaId,
                UsuarioId = reserva.UsuarioId,
                DataInicio = reserva.DataInicio,
                DataFim = reserva.DataFim,
                Status = StatusAluguel.AguardandoPagamento,
                ReservaId = null,
                ValorCaucao = reserva.Ferramenta.Caucao,
                CaucaoRetida = false,
            };

            _context.Alugueis.Add(aluguel);
            reserva.Status = StatusReserva.ConvertidaEmAluguel;

            await _context.SaveChangesAsync();

            return new ResultadoServiceAluguelDTO
            {
                Sucesso = true,
                StatusCode = 201,
                Mensagem = "Aluguel criado a partir da reserva com sucesso.",
                Dados = aluguel
            };
        }

        // ================================
        // PAGAR ALUGUEL
        // ================================
        public async Task<ResultadoServiceAluguelDTO> PagarAluguelAsync(int aluguelId, int usuarioId)
        {
            var usuario = await _context.Usuario
                .FirstOrDefaultAsync(u => u.Id == usuarioId);

            if (usuario == null)
            {
                return new ResultadoServiceAluguelDTO
                {
                    Sucesso = false,
                    StatusCode = 404,
                    Mensagem = "Usuário não encontrado."
                };
            }

            if (usuario.Bloqueado)
            {
                return new ResultadoServiceAluguelDTO
                {
                    Sucesso = false,
                    StatusCode = 400,
                    Mensagem = "Usuário bloqueado."
                };
            }

            var aluguel = await _context.Alugueis
                .Include(a => a.Ferramenta)
                .FirstOrDefaultAsync(a => a.Id == aluguelId);

            if (aluguel == null)
            {
                return new ResultadoServiceAluguelDTO
                {
                    Sucesso = false,
                    StatusCode = 404,
                    Mensagem = "Aluguel não encontrado."
                };
            }

            if (aluguel.UsuarioId != usuarioId)
            {
                return new ResultadoServiceAluguelDTO
                {
                    Sucesso = false,
                    StatusCode = 403,
                    Mensagem = "Você não tem permissão para pagar este aluguel."
                };
            }

            if (aluguel.Status != StatusAluguel.AguardandoPagamento)
            {
                return new ResultadoServiceAluguelDTO
                {
                    Sucesso = false,
                    StatusCode = 400,
                    Mensagem = "O aluguel não está aguardando pagamento."
                };
            }

            decimal valorAluguel = CalcularValorAluguel(
                aluguel.DataInicio,
                aluguel.DataFim,
                aluguel.Ferramenta.Diaria
            );

            decimal caucao = aluguel.ValorCaucao;
            decimal valorTotalPagamento = valorAluguel + caucao;

            aluguel.ValorTotal = valorAluguel;
            aluguel.ValorDevolvido = 0;
            aluguel.Status = StatusAluguel.Ativo;

            await _context.SaveChangesAsync();

            return new ResultadoServiceAluguelDTO
            {
                Sucesso = true,
                StatusCode = 200,
                Mensagem = "Pagamento aprovado.",
                Dados = new
                {
                    aluguelId = aluguel.Id,
                    valorAluguel,
                    caucao,
                    valorTotalPagamento,
                    status = aluguel.Status
                }
            };
        }

        // ================================
        // SOLICITAR FINALIZAÇÃO
        // ================================
        public async Task<ResultadoServiceAluguelDTO> SolicitarFinalizacaoAsync(int aluguelId, int usuarioId)
        {
            var usuario = await _context.Usuario
                .FirstOrDefaultAsync(u => u.Id == usuarioId);

            if (usuario == null)
            {
                return new ResultadoServiceAluguelDTO
                {
                    Sucesso = false,
                    StatusCode = 404,
                    Mensagem = "Usuário não encontrado."
                };
            }

            if (usuario.Bloqueado)
            {
                return new ResultadoServiceAluguelDTO
                {
                    Sucesso = false,
                    StatusCode = 400,
                    Mensagem = "Usuário bloqueado."
                };
            }

            var aluguel = await _context.Alugueis
                .FirstOrDefaultAsync(a => a.Id == aluguelId);

            if (aluguel == null)
            {
                return new ResultadoServiceAluguelDTO
                {
                    Sucesso = false,
                    StatusCode = 404,
                    Mensagem = "Aluguel não encontrado."
                };
            }

            if (aluguel.UsuarioId != usuarioId)
            {
                return new ResultadoServiceAluguelDTO
                {
                    Sucesso = false,
                    StatusCode = 403,
                    Mensagem = "Você não tem permissão para solicitar a finalização deste aluguel."
                };
            }

            if (aluguel.Status != StatusAluguel.Ativo)
            {
                return new ResultadoServiceAluguelDTO
                {
                    Sucesso = false,
                    StatusCode = 400,
                    Mensagem = "Só é possível solicitar finalização de um aluguel ativo."
                };
            }

            aluguel.Status = StatusAluguel.AguardandoConfirmacao;
            await _context.SaveChangesAsync();

            return new ResultadoServiceAluguelDTO
            {
                Sucesso = true,
                StatusCode = 200,
                Mensagem = "Solicitação de finalização enviada com sucesso.",
                Dados = new
                {
                    aluguelId = aluguel.Id,
                    status = aluguel.Status
                }
            };
        }

        // ================================
        // CONFIRMAR FINALIZAÇÃO
        // ================================
        public async Task<ResultadoServiceAluguelDTO> ConfirmarFinalizacaoAsync(int aluguelId, int usuarioId)
        {
            var usuario = await _context.Usuario
                .FirstOrDefaultAsync(u => u.Id == usuarioId);

            if (usuario == null)
            {
                return new ResultadoServiceAluguelDTO
                {
                    Sucesso = false,
                    StatusCode = 404,
                    Mensagem = "Usuário não encontrado."
                };
            }

            if (usuario.Bloqueado)
            {
                return new ResultadoServiceAluguelDTO
                {
                    Sucesso = false,
                    StatusCode = 400,
                    Mensagem = "Usuário bloqueado."
                };
            }

            var aluguel = await _context.Alugueis
                .Include(a => a.Ferramenta)
                .FirstOrDefaultAsync(a => a.Id == aluguelId);

            if (aluguel == null)
            {
                return new ResultadoServiceAluguelDTO
                {
                    Sucesso = false,
                    StatusCode = 404,
                    Mensagem = "Aluguel não encontrado."
                };
            }

            if (aluguel.Ferramenta.UsuarioId != usuarioId)
            {
                return new ResultadoServiceAluguelDTO
                {
                    Sucesso = false,
                    StatusCode = 403,
                    Mensagem = "Você não tem permissão para confirmar a finalização deste aluguel."
                };
            }

            if (aluguel.Status != StatusAluguel.AguardandoConfirmacao)
            {
                return new ResultadoServiceAluguelDTO
                {
                    Sucesso = false,
                    StatusCode = 400,
                    Mensagem = "O aluguel não está aguardando confirmação."
                };
            }

            decimal valorAluguel = CalcularValorAluguel(
                aluguel.DataInicio,
                aluguel.DataFim,
                aluguel.Ferramenta.Diaria
            );

            decimal multa = CalcularMulta(
                aluguel.DataFim,
                aluguel.Ferramenta.Diaria
            );

            decimal valorFinal = valorAluguel + multa;

            aluguel.ValorTotal = valorFinal;
            aluguel.CaucaoRetida = false;
            aluguel.ValorDevolvido = aluguel.ValorCaucao;
            aluguel.Status = StatusAluguel.Finalizado;

            await _context.SaveChangesAsync();

            return new ResultadoServiceAluguelDTO
            {
                Sucesso = true,
                StatusCode = 200,
                Mensagem = "Aluguel finalizado com sucesso.",
                Dados = new
                {
                    aluguelId = aluguel.Id,
                    valorAluguel,
                    multa,
                    valorFinal,
                    valorCaucao = aluguel.ValorCaucao,
                    valorDevolvido = aluguel.ValorDevolvido,
                    status = aluguel.Status
                }
            };
        }

        // ================================
        // RECUSAR FINALIZAÇÃO
        // ================================
        public async Task<ResultadoServiceAluguelDTO> RecusarFinalizacaoAsync(int aluguelId, int usuarioId)
        {
            var usuario = await _context.Usuario
                .FirstOrDefaultAsync(u => u.Id == usuarioId);

            if (usuario == null)
            {
                return new ResultadoServiceAluguelDTO
                {
                    Sucesso = false,
                    StatusCode = 404,
                    Mensagem = "Usuário não encontrado."
                };
            }

            if (usuario.Bloqueado)
            {
                return new ResultadoServiceAluguelDTO
                {
                    Sucesso = false,
                    StatusCode = 400,
                    Mensagem = "Usuário bloqueado."
                };
            }

            var aluguel = await _context.Alugueis
                .Include(a => a.Ferramenta)
                .FirstOrDefaultAsync(a => a.Id == aluguelId);

            if (aluguel == null)
            {
                return new ResultadoServiceAluguelDTO
                {
                    Sucesso = false,
                    StatusCode = 404,
                    Mensagem = "Aluguel não encontrado."
                };
            }

            if (aluguel.Ferramenta.UsuarioId != usuarioId)
            {
                return new ResultadoServiceAluguelDTO
                {
                    Sucesso = false,
                    StatusCode = 403,
                    Mensagem = "Você não tem permissão para recusar a finalização deste aluguel."
                };
            }

            if (aluguel.Status != StatusAluguel.AguardandoConfirmacao)
            {
                return new ResultadoServiceAluguelDTO
                {
                    Sucesso = false,
                    StatusCode = 400,
                    Mensagem = "O aluguel não está aguardando confirmação."
                };
            }

            aluguel.Status = StatusAluguel.Atrasado;
            aluguel.CaucaoRetida = true;
            aluguel.ValorDevolvido = 0;

            await _context.SaveChangesAsync();

            return new ResultadoServiceAluguelDTO
            {
                Sucesso = true,
                StatusCode = 200,
                Mensagem = "Finalização recusada. O aluguel foi marcado como atrasado.",
                Dados = new
                {
                    aluguelId = aluguel.Id,
                    valorCaucao = aluguel.ValorCaucao,
                    valorDevolvido = aluguel.ValorDevolvido,
                    caucaoRetida = aluguel.CaucaoRetida,
                    status = aluguel.Status
                }
            };
        }

        // ================================
        // MÉTODO AUXILIAR - CALCULAR VALOR BASE
        // ================================
        private decimal CalcularValorAluguel(DateTime inicio, DateTime fim, decimal precoDia)
        {
            int dias = (fim - inicio).Days + 1;

            if (dias <= 0)
                dias = 1;

            return dias * precoDia;
        }

        // ================================
        // MÉTODO AUXILIAR - CALCULAR MULTA
        // ================================
        private decimal CalcularMulta(DateTime dataFim, decimal precoDia)
        {
            if (DateTime.UtcNow <= dataFim)
                return 0;

            int diasAtraso = (int)Math.Ceiling((DateTime.UtcNow - dataFim).TotalDays);

            decimal fatorMulta =
                diasAtraso <= 3 ? 1.2m :
                diasAtraso <= 7 ? 1.5m :
                                  2.0m;

            return diasAtraso * precoDia * fatorMulta;
        }
    }
}