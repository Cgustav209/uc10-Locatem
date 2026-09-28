using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text.Json;
using uc10_Locatem.API.Model;
using uc10_Locatem.Data;
using uc10_Locatem.Enum;
using uc10_Locatem.Model;
using uc10_Locatem.Model.DTO;
using uc10_Locatem.Services;

namespace uc10_Locatem.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class FerramentaController : ControllerBase
    {
        private readonly AppDbContext _ferramentaDbContext;
        private readonly GeolocalizacaoService _geolocalizacaoService;
        private readonly EnderecoGeolocalizacaoService _enderecoGeolocalizacaoService;

        public FerramentaController(
            AppDbContext context,
            GeolocalizacaoService geolocalizacaoService,
            EnderecoGeolocalizacaoService enderecoGeolocalizacaoService)
        {
            _ferramentaDbContext = context;
            _geolocalizacaoService = geolocalizacaoService;
            _enderecoGeolocalizacaoService = enderecoGeolocalizacaoService;
        }

        // =====================================================
        // LISTAR TODAS AS FERRAMENTAS
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> GetAllFerramentas()
        {
            var ferramentas = await ConsultarFerramentasComDados()
                .AsNoTracking()
                .ToListAsync();

            return Ok(await MontarRespostasFerramentas(ferramentas));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetFerramentaPorId(int id)
        {
            var ferramenta = await ConsultarFerramentasComDados()
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.FerramentaId == id);

            if (ferramenta == null)
                return NotFound("Ferramenta não encontrada");

            return Ok((await MontarRespostasFerramentas(new List<Ferramenta> { ferramenta })).Single());
        }

        // =====================================================
        // LISTAR APENAS DISPONÍVEIS
        // =====================================================

        [HttpGet("Disponiveis")]
        public async Task<IActionResult> GetFerramentasDisponiveis()
        {
            var ferramentas = await ConsultarFerramentasComDados()
                .Where(f =>
                    f.Status == StatusCadastro.Ativo &&
                    f.Disponibilidade == StatusDisponibilidade.Disponivel)
                .AsNoTracking()
                .ToListAsync();

            return Ok(await MontarRespostasFerramentas(ferramentas));
        }

        // =====================================================
        // LISTAR AS FERRAMENTAS DO LOCADOR LOGADO
        // =====================================================

        [AllowAnonymous]
        [HttpGet("Locador/{usuarioId:int}")]
        public async Task<IActionResult> GetFerramentasDoLocador(int usuarioId)
        {
            var locadorExiste = await _ferramentaDbContext.Usuario
                .AnyAsync(u => u.Id == usuarioId && u.TipoUsuario == TipoUsuario.Locador);

            if (!locadorExiste)
                return NotFound("Locador não encontrado");

            var ferramentas = await ConsultarFerramentasComDados()
                .Where(f =>
                    f.UsuarioId == usuarioId &&
                    f.Status == StatusCadastro.Ativo &&
                    f.Disponibilidade == StatusDisponibilidade.Disponivel)
                .AsNoTracking()
                .ToListAsync();

            return Ok(await MontarRespostasFerramentas(ferramentas));
        }

        [HttpGet("Minhas")]
        public async Task<IActionResult> GetMinhasFerramentas()
        {
            var usuarioId = User.FindFirst("id")?.Value;
            var tipoUsuario = User.FindFirst("TipoUsuario")?.Value;

            if (usuarioId == null || !int.TryParse(usuarioId, out int id))
                return Unauthorized("Usuário não autenticado");

            if (tipoUsuario != TipoUsuario.Locador.ToString())
                return Unauthorized("Somente locadores podem consultar suas ferramentas");

            var ferramentas = await ConsultarFerramentasComDados()
                .Where(f => f.UsuarioId == id)
                .AsNoTracking()
                .ToListAsync();

            return Ok(await MontarRespostasFerramentas(ferramentas));
        }

        // =====================================================
        // CADASTRAR FERRAMENTA
        // =====================================================

        [HttpPost]
        public async Task<IActionResult> CadastrarFerramenta(
            [FromBody] CadastrarFerramentaDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var usuarioId = User.FindFirst("id")?.Value;
            var tipoUsuario = User.FindFirst("TipoUsuario")?.Value;

            if (usuarioId == null)
                return Unauthorized("Usuário não autenticado");

            if (tipoUsuario != TipoUsuario.Locador.ToString())
                return Unauthorized("Somente locadores podem cadastrar ferramentas");

           // int id = int.Parse(usuarioId);

            //nova validação
            if (!int.TryParse(usuarioId, out int id))
            {
                return Unauthorized(
                    "O ID do usuário autenticado é inválido.");
            }

            var enderecoRetirada = await ObterOuCriarEnderecoRetirada(id, dto.EnderecoRetirada);

            if (enderecoRetirada == null)
            {
                return BadRequest(
                    "É necessário cadastrar um endereço válido antes de cadastrar uma ferramenta.");
            }

            string acessorios = string.Join(", ",
                dto.Acessorios ?? new List<string>());

            var especificacoes = (dto.EspecificacoesTecnicas ?? new List<EspecificacaoTecnicaDTO>())
                .Where(e => !string.IsNullOrWhiteSpace(e.Label) || !string.IsNullOrWhiteSpace(e.Valor))
                .Select(e => new EspecificacaoTecnicaDTO
                {
                    Label = e.Label.Trim(),
                    Valor = e.Valor.Trim()
                })
                .ToList();

            Ferramenta novaFerramenta = new()
            {
                Nome = dto.Nome,
                Marca = dto.Marca,
                Modelo = dto.Modelo,
                Descricao = dto.Descricao,
                Acessorios = acessorios,
                Diaria = dto.Diaria,
                Caucao = dto.Caucao,
                QuantidadeDisponivel = dto.QuantidadeDisponivel,
                EstadoConservacao = dto.EstadoConservacao ?? string.Empty,
                FonteAlimentacao = dto.FonteAlimentacao ?? string.Empty,
                EspecificacoesTecnicasJson = JsonSerializer.Serialize(especificacoes),
                TipoAprovacao = NormalizarTipoAprovacao(dto.TipoAprovacao),
                Status = StatusCadastro.Ativo,
                Disponibilidade = StatusDisponibilidade.Disponivel,
                CategoriaId = dto.CategoriaId,
                UsuarioId = id,
                Endereco = enderecoRetirada
            };

            List<DateTime> diasIndisponiveis;

            try
            {
                diasIndisponiveis = ParsearDiasIndisponiveis(dto.DiasIndisponiveis);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }

            foreach (var data in diasIndisponiveis)
            {
                novaFerramenta.BloqueiosDisponibilidade.Add(new BloqueioDisponibilidade
                {
                    DataInicio = data,
                    DataFim = data.AddDays(1),
                    Motivo = "Indisponibilidade cadastrada no formulário",
                    Ativo = true
                });
            }

            await _ferramentaDbContext.Ferramenta.AddAsync(novaFerramenta);

            int resultado = await _ferramentaDbContext.SaveChangesAsync();

            if (resultado > 0)
            {
                var ferramentaCriada = await ConsultarFerramentasComDados()
                    .AsNoTracking()
                    .FirstAsync(f => f.FerramentaId == novaFerramenta.FerramentaId);

                var resposta = (await MontarRespostasFerramentas(
                    new List<Ferramenta> { ferramentaCriada }))
                    .Single();

                return CreatedAtAction(
                    nameof(GetFerramentaPorId),
                    new { id = novaFerramenta.FerramentaId },
                    resposta);
            }

            return BadRequest("Erro ao cadastrar ferramenta");
        }

        // =====================================================
        // EDITAR FERRAMENTA
        // =====================================================

        [HttpPut("{id}")]
        public async Task<IActionResult> EditarFerramenta(
            int id,
            [FromBody] EditarFerramentaDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var usuarioId = User.FindFirst("id")?.Value;
            var tipoUsuario = User.FindFirst("TipoUsuario")?.Value;

            if (usuarioId == null)
                return Unauthorized("Usuário não autenticado");

            if (tipoUsuario != TipoUsuario.Locador.ToString())
                return Unauthorized("Somente locadores podem editar ferramentas");

            int idUser = int.Parse(usuarioId);

            var ferramenta = await ConsultarFerramentasComDados()
                .FirstOrDefaultAsync(f => f.FerramentaId == id);

            if (ferramenta == null)
                return NotFound("Ferramenta não encontrada");

            // VERIFICA SE É O DONO
            if (ferramenta.UsuarioId != idUser)
            {
                return Unauthorized(
                    "Você não tem permissão para editar esta ferramenta");
            }

            string acessorios = string.Join(", ",
                dto.Acessorios ?? new List<string>());

            ferramenta.Nome = dto.Nome;
            ferramenta.Marca = dto.Marca;
            ferramenta.Modelo = dto.Modelo;
            ferramenta.Descricao = dto.Descricao;
            ferramenta.Acessorios = acessorios;
            ferramenta.Diaria = dto.Diaria;
            ferramenta.Caucao = dto.Caucao;
            ferramenta.CategoriaId = dto.CategoriaId;

            if (dto.QuantidadeDisponivel.HasValue)
                ferramenta.QuantidadeDisponivel = dto.QuantidadeDisponivel.Value;

            if (dto.EstadoConservacao != null)
                ferramenta.EstadoConservacao = dto.EstadoConservacao;

            if (dto.FonteAlimentacao != null)
                ferramenta.FonteAlimentacao = dto.FonteAlimentacao;

            if (dto.EspecificacoesTecnicas != null)
            {
                ferramenta.EspecificacoesTecnicasJson = JsonSerializer.Serialize(
                    dto.EspecificacoesTecnicas);
            }

            if (!string.IsNullOrWhiteSpace(dto.TipoAprovacao))
                ferramenta.TipoAprovacao = NormalizarTipoAprovacao(dto.TipoAprovacao);

            if (dto.EnderecoRetirada != null)
                ferramenta.Endereco = await ObterOuCriarEnderecoRetirada(idUser, dto.EnderecoRetirada);

            if (dto.DiasIndisponiveis != null)
            {
                ferramenta.BloqueiosDisponibilidade
                    .ToList()
                    .ForEach(b => b.Ativo = false);

                List<DateTime> diasIndisponiveis;

                try
                {
                    diasIndisponiveis = ParsearDiasIndisponiveis(dto.DiasIndisponiveis);
                }
                catch (ArgumentException ex)
                {
                    return BadRequest(ex.Message);
                }

                foreach (var data in diasIndisponiveis)
                {
                    ferramenta.BloqueiosDisponibilidade.Add(new BloqueioDisponibilidade
                    {
                        DataInicio = data,
                        DataFim = data.AddDays(1),
                        Motivo = "Indisponibilidade cadastrada no formulário",
                        Ativo = true
                    });
                }
            }

            await _ferramentaDbContext.SaveChangesAsync();

            return Ok("Ferramenta atualizada com sucesso");
        }

        // =====================================================
        // ALTERAR DISPONIBILIDADE
        // =====================================================

        [HttpPatch("{id}/Disponibilidade")]
        public async Task<IActionResult> AlterarDisponibilidade(
            int id,
            [FromQuery] StatusDisponibilidade disponibilidade)
        {
            var ferramenta = await _ferramentaDbContext.Ferramenta.FindAsync(id);

            if (ferramenta == null)
                return NotFound("Ferramenta não encontrada");

            ferramenta.Disponibilidade = disponibilidade;

            await _ferramentaDbContext.SaveChangesAsync();

            return Ok(new
            {
                mensagem = "Disponibilidade atualizada",
                ferramentaId = ferramenta.FerramentaId,
                disponibilidade = ferramenta.Disponibilidade
            });
        }

        // =====================================================
        // DESATIVAR FERRAMENTA
        // =====================================================

        [HttpPatch("{id}/Desativar")]
        public async Task<IActionResult> DesativarFerramenta(int id)
        {
            var ferramenta = await _ferramentaDbContext.Ferramenta.FindAsync(id);

            if (ferramenta == null)
                return NotFound("Ferramenta não encontrada");

            ferramenta.Status = StatusCadastro.Inativo;

            await _ferramentaDbContext.SaveChangesAsync();

            return Ok("Ferramenta desativada com sucesso");
        }

        [HttpPatch("{id}/Ativar")]
        public async Task<IActionResult> AtivarFerramenta(int id)
        {
            var ferramenta = await _ferramentaDbContext.Ferramenta.FindAsync(id);

            if (ferramenta == null)
                return NotFound("Ferramenta não encontrada");

            ferramenta.Status = StatusCadastro.Ativo;

            await _ferramentaDbContext.SaveChangesAsync();

            return Ok("Ferramenta ativada com sucesso");
        }

        //BUSCAR FERRAMENTAS
        //===============

        //        [HttpPost("BuscarFerramentasProximas")]
        //        public async Task<IActionResult> BuscarFerramentasProximas(
        //        [FromBody] BuscarFerramentasDTO dto)
        //        {
        //            if (!ModelState.IsValid)
        //            {
        //                return BadRequest(ModelState);
        //            }

        //            double latitude;
        //            double longitude;

        //            // endereço OU coordenadas
        //            if (!string.IsNullOrWhiteSpace(dto.Endereco))
        //            {
        //                var coordenadas = await _enderecoGeolocalizacaoService
        //                    .ObterCoordenadasPorEndereco(dto.Endereco);

        //                latitude = coordenadas.latitude;
        //                longitude = coordenadas.longitude;
        //            }
        //            else if (dto.LatitudeUsuario.HasValue && dto.LongitudeUsuario.HasValue)
        //            {
        //                latitude = dto.LatitudeUsuario.Value;
        //                longitude = dto.LongitudeUsuario.Value;
        //            }
        //            else
        //            {
        //                return BadRequest("Informe endereço ou coordenadas.");
        //            }

        //            var query = _ferramentaDbContext.Ferramenta
        //               .Include(f => f.Usuario)
        //               .ThenInclude (u => u.Enderecos)
        //               .Where(f =>f.Status == StatusCadastro.Ativo &&
        //               f.Disponibilidade == StatusDisponibilidade.Disponivel);

        //            //.Where(f => f.Status == StatusCadastro.Ativo);

        //            // filtro categoria
        //            if (dto.CategoriaId.HasValue)
        //            {
        //                query = query.Where(f => f.CategoriaId == dto.CategoriaId.Value);
        //            }

        //            var ferramentas = await query.ToListAsync();
        //            //logs temporarios
        //            Console.WriteLine($"Ferramentas encontradas no banco: {ferramentas.Count}");

        //            var resultado = ferramentas
        //           .Where(f => f.Usuario.Enderecos.Any(e => e.EhPrioritario))
        //           .Select(f =>
        //    {
        //              var endereco = f.Usuario.Enderecos
        //             .First(e => e.EhPrioritario);

        //             return new
        //           {
        //            f.FerramentaId,
        //            f.Nome,

        //            DistanciaKm = Math.Round(
        //                _geolocalizacaoService.CalcularDistancia(
        //                    latitude,
        //                    longitude,
        //                    endereco.Latitude ?? 0,
        //                    endereco.Longitude ?? 0
        //                ), 2)
        //                };
        //                })
        //            .Where(f => f.DistanciaKm <= dto.RaioKm)
        //            .OrderBy(f => f.DistanciaKm)
        //            .ToList();

        //            if (!resultado.Any())
        //            {
        //                return NotFound("Nenhuma ferramenta encontrada.");
        //            }

        //            return Ok(resultado);
        //        }
        //    }
        //}



        [HttpPost("BuscarFerramentasProximas")]
        public async Task<IActionResult> BuscarFerramentasProximas(
    [FromBody] BuscarFerramentasDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (dto.RaioKm <= 0)
            {
                return BadRequest(
                    "O raio deve ser maior que zero.");
            }

            double latitude;
            double longitude;

            // Obtém localização do usuário
            if (!string.IsNullOrWhiteSpace(dto.Endereco))
            {
                try
                {
                    var coordenadas =
                        await _enderecoGeolocalizacaoService
                            .ObterCoordenadasPorEndereco(dto.Endereco);

                    latitude = coordenadas.latitude;
                    longitude = coordenadas.longitude;
                }
                catch (Exception ex)
                {
                    return BadRequest(
                        $"Erro ao localizar endereço informado: {ex.Message}");
                }
            }
            else if (
                dto.LatitudeUsuario.HasValue &&
                dto.LongitudeUsuario.HasValue)
            {
                latitude = dto.LatitudeUsuario.Value;
                longitude = dto.LongitudeUsuario.Value;
            }
            else
            {
                return BadRequest(
                    "Informe endereço ou coordenadas.");
            }

            var query = _ferramentaDbContext.Ferramenta
                .Include(f => f.Usuario)
                .ThenInclude(u => u.Enderecos)
                .Where(f =>
                    f.Status == StatusCadastro.Ativo &&
                    f.Disponibilidade == StatusDisponibilidade.Disponivel);

            if (dto.CategoriaId.HasValue)
            {
                query = query.Where(f =>
                    f.CategoriaId == dto.CategoriaId.Value);
            }

            var ferramentas = await query.ToListAsync();


    //        return Ok(
    //ferramentas.Select(f => new
    //{
    //    FerramentaId = f.FerramentaId,
    //    Nome = f.Nome,
    //    UsuarioId = f.UsuarioId,

    //    Enderecos = f.Usuario.Enderecos.Select(e => new
    //    {
    //        e.Id,
    //        e.UsuarioId,
    //        e.Latitude,
    //        e.Longitude,
    //        e.EhPrioritario
    //               })
    //            })
    //          );

            if (!ferramentas.Any())
            {
                return NotFound(
                    "Nenhuma ferramenta cadastrada.");
            }

            var resultado = new List<object>();

            foreach (var ferramenta in ferramentas)
            {
                var endereco = ferramenta.Usuario?.Enderecos?
                    .FirstOrDefault(e =>
                        e.Latitude.HasValue &&
                        e.Longitude.HasValue);

                if (endereco == null)
                {
                    continue;
                }

                var distancia =
                    _geolocalizacaoService.CalcularDistancia(
                        latitude,
                        longitude,
                        endereco.Latitude.Value,
                        endereco.Longitude.Value);

                if (distancia <= dto.RaioKm)
                {
                    resultado.Add(new
                    {
                        ferramenta.FerramentaId,
                        ferramenta.Nome,
                        DistanciaKm = Math.Round(distancia, 2),

                        LatitudeFerramenta = endereco.Latitude,
                        LongitudeFerramenta = endereco.Longitude
                    });
                }
            }

            if (!resultado.Any())
            {
                return NotFound(
                    "Nenhuma ferramenta encontrada dentro do raio informado.");
            }

            //return Ok(resultado);
            return Ok(
    ferramentas.Select(f => new
    {
        FerramentaId = f.FerramentaId,
        UsuarioId = f.UsuarioId,
        QuantidadeEnderecos = f.Usuario.Enderecos.Count
    })
);
        }
        private IQueryable<Ferramenta> ConsultarFerramentasComDados()
        {
            return _ferramentaDbContext.Ferramenta
                .Include(f => f.Categoria)
                .Include(f => f.Usuario)
                .Include(f => f.Imagens)
                .Include(f => f.Endereco)
                .Include(f => f.BloqueiosDisponibilidade)
                .AsSplitQuery();
        }

        private async Task<List<FerramentaResponseDTO>> MontarRespostasFerramentas(List<Ferramenta> ferramentas)
        {
            var ids = ferramentas.Select(f => f.FerramentaId).ToList();

            var avaliacoes = await _ferramentaDbContext.Avaliacoes
                .Where(a => a.FerramentaId.HasValue && ids.Contains(a.FerramentaId.Value))
                .GroupBy(a => a.FerramentaId!.Value)
                .Select(g => new
                {
                    FerramentaId = g.Key,
                    Media = g.Average(a => (double)a.Nota),
                    Total = g.Count()
                })
                .ToDictionaryAsync(x => x.FerramentaId);

            return ferramentas.Select(f =>
            {
                var resposta = MontarRespostaFerramenta(f);

                if (avaliacoes.TryGetValue(f.FerramentaId, out var avaliacao))
                {
                    resposta.AvaliacaoMedia = avaliacao.Media;
                    resposta.TotalAvaliacoes = avaliacao.Total;
                }

                return resposta;
            }).ToList();
        }

        private FerramentaResponseDTO MontarRespostaFerramenta(Ferramenta ferramenta)
        {
            var especificacoes = new List<EspecificacaoTecnicaDTO>();

            if (!string.IsNullOrWhiteSpace(ferramenta.EspecificacoesTecnicasJson))
            {
                try
                {
                    especificacoes = JsonSerializer.Deserialize<List<EspecificacaoTecnicaDTO>>(
                        ferramenta.EspecificacoesTecnicasJson) ?? new List<EspecificacaoTecnicaDTO>();
                }
                catch (JsonException)
                {
                    especificacoes = new List<EspecificacaoTecnicaDTO>();
                }
            }

            var diasIndisponiveis = new List<string>();

            foreach (var bloqueio in ferramenta.BloqueiosDisponibilidade.Where(b => b.Ativo))
            {
                for (var data = bloqueio.DataInicio.Date; data < bloqueio.DataFim.Date; data = data.AddDays(1))
                {
                    diasIndisponiveis.Add(data.ToString("yyyy-MM-dd"));
                }
            }

            diasIndisponiveis = diasIndisponiveis
                .Distinct()
                .OrderBy(d => d)
                .ToList();

            var acessorios = string.IsNullOrWhiteSpace(ferramenta.Acessorios)
                ? new List<string>()
                : ferramenta.Acessorios
                    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .ToList();

            var endereco = ferramenta.Endereco;

            return new FerramentaResponseDTO
            {
                FerramentaId = ferramenta.FerramentaId,
                Nome = ferramenta.Nome,
                Marca = ferramenta.Marca,
                Modelo = ferramenta.Modelo,
                Descricao = ferramenta.Descricao,
                Acessorios = acessorios,
                Diaria = ferramenta.Diaria,
                Caucao = ferramenta.Caucao,
                DataCadastro = ferramenta.DataCadastro,
                CategoriaId = ferramenta.CategoriaId,
                CategoriaNome = ferramenta.Categoria?.nome ?? string.Empty,
                UsuarioId = ferramenta.UsuarioId,
                UsuarioNome = ferramenta.Usuario?.Nome ?? string.Empty,
                UsuarioFotoUrl = ferramenta.Usuario?.UrlFoto,
                Status = ferramenta.Status,
                Disponibilidade = ferramenta.Disponibilidade,
                QuantidadeDisponivel = ferramenta.QuantidadeDisponivel,
                EstadoConservacao = ferramenta.EstadoConservacao,
                FonteAlimentacao = ferramenta.FonteAlimentacao,
                EspecificacoesTecnicas = especificacoes,
                DiasIndisponiveis = diasIndisponiveis,
                TipoAprovacao = NormalizarTipoAprovacao(ferramenta.TipoAprovacao),
                EnderecoId = ferramenta.EnderecoId,
                EnderecoRetirada = endereco == null ? null : new EnderecoFerramentaResponseDTO
                {
                    Id = endereco.Id,
                    Logradouro = endereco.Logradouro,
                    Numero = endereco.Numero,
                    Complemento = endereco.Complemento,
                    Bairro = endereco.Bairro,
                    Cidade = endereco.Cidade,
                    Estado = endereco.Estado,
                    CEP = endereco.CEP,
                    Latitude = endereco.Latitude,
                    Longitude = endereco.Longitude
                },
                Localizacao = endereco == null
                    ? string.Empty
                    : $"{endereco.Cidade} - {endereco.Estado}",
                Fotos = ferramenta.Imagens
                    .OrderBy(i => i.Id)
                    .Select(i => new FotoFerramentaBuscaDTO
                    {
                        Id = i.Id,
                        UrlImagem = i.UrlImagem
                    })
                    .ToList()
            };
        }

        private async Task<Endereco?> ObterOuCriarEnderecoRetirada(
            int usuarioId,
            uc10_Locatem.API.Model.DTO.CriarEnderecoDTO? dto)
        {
            if (dto == null)
            {
                return await _ferramentaDbContext.Endereco
                    .Where(e =>
                        e.UsuarioId == usuarioId &&
                        e.Latitude.HasValue &&
                        e.Longitude.HasValue)
                    .OrderByDescending(e => e.EhPrioritario)
                    .FirstOrDefaultAsync();
            }

            var existente = await _ferramentaDbContext.Endereco
                .FirstOrDefaultAsync(e =>
                    e.UsuarioId == usuarioId &&
                    e.CEP == dto.CEP &&
                    e.Logradouro == dto.Logradouro &&
                    e.Numero == dto.Numero);

            if (existente != null)
                return existente;

            var enderecoCompleto =
                $"{dto.Logradouro}, {dto.Numero}, {dto.Bairro}, {dto.Cidade}, {dto.Estado}";

            var coordenadas = await _enderecoGeolocalizacaoService
                .ObterCoordenadasPorEndereco(enderecoCompleto);

            return new Endereco
            {
                Logradouro = dto.Logradouro,
                Numero = dto.Numero,
                Complemento = dto.Complemento,
                Bairro = dto.Bairro,
                Cidade = dto.Cidade,
                Estado = dto.Estado,
                CEP = dto.CEP,
                Latitude = coordenadas.latitude,
                Longitude = coordenadas.longitude,
                TipoEndereco = dto.TipoEndereco is uc10_Locatem.Enum.TipoEndereco.Residencial or uc10_Locatem.Enum.TipoEndereco.Comercial
                    ? dto.TipoEndereco
                    : uc10_Locatem.Enum.TipoEndereco.Residencial,
                EhPrioritario = false,
                UsuarioId = usuarioId
            };
        }

        private static string NormalizarTipoAprovacao(string? tipoAprovacao)
        {
            return tipoAprovacao?.Trim().ToLowerInvariant() switch
            {
                "automatica" => "automatica",
                "manual" => "manual",
                _ => "manual"
            };
        }

        private static List<DateTime> ParsearDiasIndisponiveis(IEnumerable<string>? dias)
        {
            var resultado = new List<DateTime>();

            foreach (var dia in dias ?? Enumerable.Empty<string>())
            {
                if (!DateTime.TryParseExact(
                    dia,
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var data))
                {
                    throw new ArgumentException($"Data indisponível inválida: {dia}");
                }

                if (data.Date < DateTime.UtcNow.Date)
                    throw new ArgumentException($"A data indisponível não pode estar no passado: {dia}");

                resultado.Add(data.Date);
            }

            return resultado.Distinct().OrderBy(d => d).ToList();
        }

    }
}
