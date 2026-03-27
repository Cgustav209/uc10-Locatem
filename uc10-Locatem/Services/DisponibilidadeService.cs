using Microsoft.EntityFrameworkCore;
using uc10_Locatem.Data;
using uc10_Locatem.Enum;
using uc10_Locatem.Model;
using uc10_Locatem.Model.DTO;

namespace uc10_Locatem.Services
{
    public class DisponibilidadeService
    {
        private readonly AppDbContext _context;

        public DisponibilidadeService(AppDbContext context)
        {
            _context = context;
        }

        // =========================================================
        // MÉTODO PRINCIPAL: VERIFICAR DISPONIBILIDADE
        // =========================================================
        public async Task<DisponibilidadeResponseDTO> VerificarDisponibilidade(VerificarDisponibilidadeDTO dto)
        {
            DisponibilidadeResponseDTO response = new DisponibilidadeResponseDTO
            {
                FerramentaId = dto.FerramentaId,
                DataInicio = dto.DataInicio,
                DataFim = dto.DataFim,
                Disponivel = false,
                Mensagem = string.Empty
            };

            // =========================================================
            // VALIDAÇÃO 1: FERRAMENTA EXISTE?
            // =========================================================
            Ferramenta? ferramenta = await _context.Ferramenta.FirstOrDefaultAsync(f => f.FerramentaId == dto.FerramentaId);

            if (ferramenta == null)
            {
                response.Mensagem = "Ferramenta não encontrada.";
                return response;
            }

            // =========================================================
            // VALIDAÇÃO 2: FERRAMENTA ESTÁ ATIVA?
            // =========================================================
            if (!ferramenta.Ativo)
            {
                response.Mensagem = "A ferramenta está inativa e não pode ser reservada.";
                return response;
            }

            // =========================================================
            // VALIDAÇÃO 3: DATA INÍCIO MENOR QUE DATA FIM
            // =========================================================
            if (dto.DataInicio >= dto.DataFim)
            {
                response.Mensagem = "A data de início deve ser menor que a data de fim.";
                return response;
            }

            // =========================================================
            // VALIDAÇÃO 4: BLOQUEAR DATAS NO PASSADO
            // =========================================================
            if (dto.DataInicio.Date < DateTime.UtcNow.Date || dto.DataFim.Date < DateTime.UtcNow.Date)
            {
                response.Mensagem = "Não é permitido reservar datas no passado.";
                return response;
            }

            // =========================================================
            // VALIDAÇÃO 5: LIMITAR DURAÇÃO MÁXIMA
            // =========================================================
            int duracaoMaximaDias = 30;
            int quantidadeDias = (dto.DataFim.Date - dto.DataInicio.Date).Days;

            if (quantidadeDias > duracaoMaximaDias)
            {
                response.Mensagem = $"A duração máxima permitida é de {duracaoMaximaDias} dias.";
                return response;
            }

            // =========================================================
            // VALIDAÇÃO 5: CONFLITO COM ALUGUÉIS
            // Considera: Ativo e AguardandoPagamento
            // =========================================================
            bool conflitoComAluguel = await _context.Alugueis
                .AnyAsync(a =>
                    a.FerramentaId == dto.FerramentaId &&
                    (a.Status == StatusAluguel.Ativo || a.Status == StatusAluguel.AguardandoPagamento) &&
                    HaSobreposicao(dto.DataInicio, dto.DataFim, a.DataInicio, a.DataFim)
                );

            if (conflitoComAluguel)
            {
                response.Mensagem = "A ferramenta está indisponível nesse período por conflito com aluguel.";
                return response;
            }

            // =========================================================
            // VALIDAÇÃO 6: CONFLITO COM RESERVAS ACEITAS
            // =========================================================
            bool conflitoComReserva = await _context.Reserva
                .AnyAsync(r =>
                    r.FerramentaId == dto.FerramentaId &&
                    r.Status == StatusReserva.Aceita &&
                    HaSobreposicao(dto.DataInicio, dto.DataFim, r.DataInicio, r.DataFim)
                );

            if (conflitoComReserva)
            {
                response.Mensagem = "A ferramenta está indisponível nesse período por conflito com reserva aceita.";
                return response;
            }

            // =========================================================
            // VALIDAÇÃO 7: CONFLITO COM BLOQUEIOS MANUAIS
            // =========================================================
            bool conflitoComBloqueio = await _context.BloqueioDisponibilidade
                .AnyAsync(b =>
                    b.FerramentaId == dto.FerramentaId &&
                    b.Ativo &&
                    HaSobreposicao(dto.DataInicio, dto.DataFim, b.DataInicio, b.DataFim)
                );

            if (conflitoComBloqueio)
            {
                response.Mensagem = "A ferramenta está indisponível nesse período por bloqueio manual.";
                return response;
            }

            // =========================================================
            // SE PASSOU POR TODAS AS VALIDAÇÕES
            // =========================================================
            response.Disponivel = true;
            response.Mensagem = "A ferramenta está disponível nesse período.";

            return response;
        }

        // =========================================================
        // MÉTODO: RETORNAR AGENDA DA FERRAMENTA
        // =========================================================
        public async Task<AgendaDisponibilidadeResponseDTO> ObterAgenda(int ferramentaId)
        {
            AgendaDisponibilidadeResponseDTO agenda = new AgendaDisponibilidadeResponseDTO
            {
                FerramentaId = ferramentaId
            };

            // =========================================================
            // BUSCAR ALUGUÉIS QUE BLOQUEIAM A AGENDA
            // =========================================================
            List<PeriodoOcupadoDTO> alugueis = await _context.Alugueis
                .Where(a =>
                    a.FerramentaId == ferramentaId &&
                    (a.Status == StatusAluguel.Ativo || a.Status == StatusAluguel.AguardandoPagamento)
                )
                .Select(a => new PeriodoOcupadoDTO
                {
                    DataInicio = a.DataInicio,
                    DataFim = a.DataFim,
                    Origem = "Aluguel"
                })
                .ToListAsync();

            // =========================================================
            // BUSCAR RESERVAS ACEITAS
            // =========================================================
            List<PeriodoOcupadoDTO> reservas = await _context.Reserva
                .Where(r =>
                    r.FerramentaId == ferramentaId &&
                    r.Status == StatusReserva.Aceita
                )
                .Select(r => new PeriodoOcupadoDTO
                {
                    DataInicio = r.DataInicio,
                    DataFim = r.DataFim,
                    Origem = "Reserva"
                })
                .ToListAsync();

            // =========================================================
            // BUSCAR BLOQUEIOS MANUAIS ATIVOS
            // =========================================================
            List<PeriodoOcupadoDTO> bloqueios = await _context.BloqueioDisponibilidade
                .Where(b =>
                    b.FerramentaId == ferramentaId &&
                    b.Ativo
                )
                .Select(b => new PeriodoOcupadoDTO
                {
                    DataInicio = b.DataInicio,
                    DataFim = b.DataFim,
                    Origem = "BloqueioManual"
                })
                .ToListAsync();

            // =========================================================
            // UNIFICAR E ORDENAR
            // =========================================================
            agenda.PeriodosOcupados = alugueis
                .Concat(reservas)
                .Concat(bloqueios)
                .OrderBy(p => p.DataInicio)
                .ToList();

            return agenda;
        }

        // =========================================================
        // MÉTODO: CRIAR BLOQUEIO MANUAL
        // =========================================================
        public async Task<BloqueioResponseDTO> CriarBloqueio(BloqueioDisponibilidadeDTO dto, int usuarioId)
        {
            BloqueioResponseDTO response = new BloqueioResponseDTO
            {
                Sucesso = false,
                Mensagem = string.Empty
            };

            // =========================================================
            // VALIDAR SE O USUÁRIO EXISTE
            // =========================================================
            Usuario? usuario = await _context.Usuario
                .FirstOrDefaultAsync(u => u.Id == usuarioId);

            if (usuario == null)
            {
                response.Mensagem = "Usuário não encontrado.";
                return response;
            }

            // =========================================================
            // VALIDAR SE O USUÁRIO É LOCADOR
            // =========================================================
            if (usuario.TipoUsuario != TipoUsuario.Locador)
            {
                response.Mensagem = "Apenas locadores podem bloquear datas.";
                return response;
            }

            // =========================================================
            // VALIDAR SE A FERRAMENTA EXISTE
            // =========================================================
            Ferramenta? ferramenta = await _context.Ferramenta
                .FirstOrDefaultAsync(f => f.FerramentaId == dto.FerramentaId);

            if (ferramenta == null)
            {
                response.Mensagem = "Ferramenta não encontrada.";
                return response;
            }

            // =========================================================
            // VALIDAR SE A FERRAMENTA ESTÁ ATIVA
            // =========================================================
            if (!ferramenta.Ativo)
            {
                response.Mensagem = "A ferramenta está inativa e não pode ser bloqueada.";
                return response;
            }

            // =========================================================
            // VALIDAR SE O USUÁRIO LOGADO É O DONO DA FERRAMENTA
            // =========================================================
            if (ferramenta.UsuarioId != usuarioId)
            {
                response.Mensagem = "Você não pode bloquear datas de uma ferramenta que não é sua.";
                return response;
            }

            // =========================================================
            // VALIDAR PERÍODO
            // =========================================================
            if (dto.DataInicio >= dto.DataFim)
            {
                response.Mensagem = "A data de início deve ser menor que a data de fim.";
                return response;
            }

            if (dto.DataInicio.Date < DateTime.UtcNow.Date || dto.DataFim.Date < DateTime.UtcNow.Date)
            {
                response.Mensagem = "Não é permitido bloquear datas no passado.";
                return response;
            }

            // =========================================================
            // IMPEDIR BLOQUEIO SOBREPOSTO COM OUTRO BLOQUEIO ATIVO
            // =========================================================
            bool conflitoComBloqueio = await _context.BloqueioDisponibilidade
                .AnyAsync(b =>
                    b.FerramentaId == dto.FerramentaId &&
                    b.Ativo &&
                    HaSobreposicao(dto.DataInicio, dto.DataFim, b.DataInicio, b.DataFim)
                );

            if (conflitoComBloqueio)
            {
                response.Mensagem = "Já existe um bloqueio ativo para esse período.";
                return response;
            }

            // =========================================================
            // CRIAR BLOQUEIO
            // =========================================================
            BloqueioDisponibilidade bloqueio = new BloqueioDisponibilidade
            {
                FerramentaId = dto.FerramentaId,
                DataInicio = dto.DataInicio,
                DataFim = dto.DataFim,
                Motivo = dto.Motivo,
                Ativo = true
            };

            _context.BloqueioDisponibilidade.Add(bloqueio);
            await _context.SaveChangesAsync();

            response.Sucesso = true;
            response.Mensagem = "Bloqueio manual criado com sucesso.";

            return response;
        }

        // =========================================================
        // MÉTODO AUXILIAR: VERIFICAR SOBREPOSIÇÃO DE DATAS
        // =========================================================
        private static bool HaSobreposicao(DateTime inicioSolicitado, DateTime fimSolicitado, DateTime inicioExistente, DateTime fimExistente)
        {
            return inicioSolicitado < fimExistente && fimSolicitado > inicioExistente;
        }
    }
}