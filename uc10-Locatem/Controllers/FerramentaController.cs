using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
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


        [HttpGet("GetAllTools")]
        public async Task<IActionResult> GetAllFerrametas()
        {
            List<Ferramenta> listaFerramenta = await _ferramentaDbContext.Ferramenta.ToListAsync();

            return Ok(listaFerramenta);
        }

        //[HttpGet("categoria/{categoriaId}")]
        //public async Task<IActionResult> GetByCategoria(int categoriaId)
        //{
        //    var ferramentas = await _ferramentaDbContext.Ferramenta
        //        .Include(f => f.categoria)
        //        .Where(f => f.CategoriaId == categoriaId)
        //        .ToListAsync();

        //    if (!ferramentas.Any())
        //    {
        //        return NotFound(new
        //        {
        //            Erro = true,
        //            Mensagem = "Nenhuma ferramenta encontrada para essa categoria"
        //        });
        //    }

        //    return Ok(ferramentas);
        //}

        // n sei se ta correto

        [HttpPost("CadastrarFerramenta")]
        public async Task<ActionResult> CadastrarFerramenta([FromBody] CadastrarFerramentaDTO dadosFerramenta)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Pega ID do usuário logado (quando tiver JWT)
            var locadorId = User.FindFirst("id")?.Value;
            var UsuarioTipo = User.FindFirst("TipoUsuario")?.Value;

            if (locadorId == null)
            {
                return Unauthorized("Usuário não autenticado");
            }

            if (UsuarioTipo != TipoUsuario.Locador.ToString())
            {
                return Unauthorized("Somente locadores podem registrar ferramenta");
            }


            int id = int.Parse(locadorId);

            string resultado = string.Join(", ", dadosFerramenta.Acessorios ?? new List<string>());

           // buscar latitude e longitude pelo endereço
            var coordenadas = await
            _enderecoGeolocalizacaoService
            .ObterCoordenadasPorEndereco(dadosFerramenta.Endereco);

            Ferramenta novaFerramenta = new Ferramenta
            {
                Nome = dadosFerramenta.Nome,
                Marca = dadosFerramenta.Marca,
                Modelo = dadosFerramenta.Modelo,
                Descricao = dadosFerramenta.Descricao,
                Acessorios = resultado,
                Diaria = dadosFerramenta.Diaria,

                CategoriaId = dadosFerramenta.CategoriaId,
                UsuarioId = id,
                Status = true,

                Endereco = dadosFerramenta.Endereco,
                Latitude = coordenadas.latitude,
                Longitude = coordenadas.longitude
            };


            _ferramentaDbContext.Ferramenta.Add(novaFerramenta);
            int resultadoInsercao = await _ferramentaDbContext.SaveChangesAsync();

            if (resultadoInsercao > 0)
                return Ok(new
                {
                    mensagem = "Ferramenta criada com sucesso!",
                    id = novaFerramenta.FerramentaId
                });

            return BadRequest("Ferramenta não foi registrada!");
        }


        [HttpPut("EditarFerramenta/{id}")]
        public async Task<ActionResult> EditarFerramenta(int id, [FromBody] EditarFerramentaDTO dadosFerramenta)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }



            // Pega ID do usuário logado (quando tiver JWT)
            var usuarioId = User.FindFirst("id")?.Value;
            // pega o tipo do atual usuario
            var UsuarioTipo = User.FindFirst("TipoUsuario")?.Value;

            // checa se existe
            if (usuarioId == null)
            {
                return Unauthorized("Usuário não autenticado");
            }

            // checa se é do tipo desejado
            if (UsuarioTipo != TipoUsuario.Locador.ToString())
            {
                return Unauthorized("Somente locadores podem registrar ferramenta");
            }


            int idUser = int.Parse(usuarioId);

            // puxa a ferramenta desejada pelo id
            var ferramenta = await _ferramentaDbContext.Ferramenta.FindAsync(id);

            if (ferramenta == null)
            {
                return NotFound("Ferramenta não encontrada!");
            }
            // pega o id de quem cadastrou a ferramenta
            int idCreator = ferramenta.UsuarioId;

            //cheaca se é o mesmo user
            if (idUser != idCreator)
            {
                return Unauthorized("Você não tem permissão para editar esta ferramenta.");
            }


            string resultado = string.Join(", ", dadosFerramenta.Acessorios ?? new List<string>());

            ferramenta.Nome = dadosFerramenta.Nome;
            ferramenta.Marca = dadosFerramenta.Marca;
            ferramenta.Modelo = dadosFerramenta.Modelo;
            ferramenta.Descricao = dadosFerramenta.Descricao;
            ferramenta.Acessorios = resultado;
            ferramenta.Diaria = dadosFerramenta.Diaria;
            ferramenta.Status = dadosFerramenta.Status;


            int conclusao = await _ferramentaDbContext.SaveChangesAsync();

            if (conclusao > 0)
                return Ok("Ferramenta atualizada com sucesso!");

            return BadRequest("Erro ao atualizar ferramenta!");
        }

        [HttpDelete("DeletarFerramenta/{id}")]
        public async Task<IActionResult> DeletarFerramenta(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Pega ID do usuário logado (quando tiver JWT)
            var usuarioId = User.FindFirst("id")?.Value;
            // pega o tipo do atual usuario
            var UsuarioTipo = User.FindFirst("TipoUsuario")?.Value;

            // checa se existe
            if (usuarioId == null)
            {
                return Unauthorized("Usuário não autenticado");
            }

            // checa se é do tipo desejado
            if (UsuarioTipo != TipoUsuario.Locador.ToString())
            {
                return Unauthorized("Somente locadores podem registrar ferramenta");
            }


            int idUser = int.Parse(usuarioId);

            // puxa a ferramenta desejada pelo id
            var ferramenta = await _ferramentaDbContext.Ferramenta.FindAsync(id);

            if (ferramenta == null)
            {
                return NotFound("Ferramenta não encontrada!");
            }
            // pega o id de quem cadastrou a ferramenta
            int idCreator = ferramenta.UsuarioId;

            //cheaca se é o mesmo user
            if (idUser != idCreator)
            {
                return Unauthorized("Você não tem permissão para editar esta ferramenta.");
            }

            _ferramentaDbContext.Ferramenta.Remove(ferramenta);

            int conclusao = await _ferramentaDbContext.SaveChangesAsync();

            if (conclusao > 0)
                return Ok("Ferramenta deletada com sucesso!");

            return BadRequest("Erro ao deleta ferramenta!");
        }


        [HttpPost("BuscarFerramentasProximas")]
        public async Task<IActionResult> BuscarFerramentasProximas(
            [FromBody] BuscarFerramentasDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

           
            double latitude;
            double longitude;

            if (!string.IsNullOrWhiteSpace(dto.Endereco))
            {
                var coordenadas = await _enderecoGeolocalizacaoService
                    .ObterCoordenadasPorEndereco(dto.Endereco);

                latitude = coordenadas.latitude;
                longitude = coordenadas.longitude;
            }
            else if (
                dto.LatitudeUsuario.HasValue &&
                dto.LongitudeUsuario.HasValue &&
                dto.LatitudeUsuario != 0 &&
                dto.LongitudeUsuario != 0)
            {
                latitude = dto.LatitudeUsuario.Value;
                longitude = dto.LongitudeUsuario.Value;
            }
            else
            {
                return BadRequest("Informe um endereço válido ou coordenadas válidas.");
            }

            var query = _ferramentaDbContext.Ferramenta
            .Where(f => f.Status == true);
            
            if (dto.CategoriaId.HasValue)
            {
                query = query.Where(f => f.CategoriaId == dto.CategoriaId.Value);
            }

            var ferramentas = await query.ToListAsync();

            var resultado = ferramentas
                .Select(f => new
                {
                   // f.FerramentaId,
                    f.Nome,
                    f.Marca,
                    f.Modelo,
                    f.Descricao,
                    f.Diaria,
                    f.CategoriaId,

                    f.Endereco,

                    DistanciaKm = Math.Round(
                        _geolocalizacaoService.CalcularDistancia(
                            latitude,
                            longitude,
                            f.Latitude,
                            f.Longitude
                        ), 2)
                })
                .Where(f => f.DistanciaKm <= dto.RaioKm)
                .OrderBy(f => f.DistanciaKm)
                .ToList();

            if (!resultado.Any())
            {
                return NotFound(new
                {
                    mensagem = "Nenhuma ferramenta encontrada dentro do raio informado."
                });
            }

            return Ok(resultado);
        }     
    }
}
