using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            var ferramentas = await _ferramentaDbContext.Ferramenta
                .Include(f => f.Categoria)
                .ToListAsync();

            return Ok(ferramentas);
        }

        // =====================================================
        // LISTAR APENAS DISPONÍVEIS
        // =====================================================

        [HttpGet("Disponiveis")]
        public async Task<IActionResult> GetFerramentasDisponiveis()
        {
            var ferramentas = await _ferramentaDbContext.Ferramenta
                .Where(f =>
                    f.Status == StatusCadastro.Ativo &&
                    f.Disponibilidade == StatusDisponibilidade.Disponivel)
                .ToListAsync();

            return Ok(ferramentas);
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

            var possuiEnderecoValido =
                await _ferramentaDbContext.Endereco
                    .AsNoTracking()
                    .AnyAsync(e =>
                        e.UsuarioId == id &&
                        e.Latitude.HasValue &&
                        e.Longitude.HasValue);

            if (!possuiEnderecoValido)
            {
                return BadRequest(
                    "É necessário cadastrar um endereço válido antes de cadastrar uma ferramenta.");
            }

            string acessorios = string.Join(", ",
                dto.Acessorios ?? new List<string>());

            Ferramenta novaFerramenta = new()
            {
                Nome = dto.Nome,
                Marca = dto.Marca,
                Modelo = dto.Modelo,
                Descricao = dto.Descricao,
                Acessorios = acessorios,
                Diaria = dto.Diaria,
                Caucao = dto.Caucao,

                //STATUS DO SISTEMA
                Status = StatusCadastro.Ativo,
                //DOSPONIBILIDADE PARA LOCAÇAO
                Disponibilidade = StatusDisponibilidade.Disponivel,

                CategoriaId = dto.CategoriaId,

                // CategoriaId = dadosFerramenta.CategoriaId,
                UsuarioId = id,
                //Status = true,
            };

            await _ferramentaDbContext.Ferramenta.AddAsync(novaFerramenta);

            int resultado = await _ferramentaDbContext.SaveChangesAsync();

            if (resultado > 0)
                return CreatedAtAction(
                    nameof(GetAllFerramentas),
                    new { id = novaFerramenta.FerramentaId },
                    novaFerramenta);

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

            var ferramenta = await _ferramentaDbContext.Ferramenta.FindAsync(id);

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

            // A busca pode receber: 1.Endereco      2.LatitudeUsuario e LongitudeUsuario
            
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
                    return BadRequest(new
                    {
                        mensagem =
                            "Não foi possível localizar o endereço informado.",
                        detalhe = ex.Message
                    });
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
                    "Informe um endereço ou as coordenadas do usuário.");
            }

            List<ResultadoBuscaFerramentaDTO> resultado;

            try
            {
                resultado =
                    await _geolocalizacaoService.BuscarPorRaioAsync(
                        latitude,
                        longitude,
                        dto.RaioKm,
                        dto.CategoriaId);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }

            if (resultado.Count == 0)
            {
                return NotFound(
                    "Nenhuma ferramenta encontrada dentro do raio informado.");
            }

            return Ok(resultado);
        }
    }
}
