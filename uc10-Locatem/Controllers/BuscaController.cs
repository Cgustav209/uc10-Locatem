using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using uc10_Locatem.Data;
using uc10_Locatem.Enum;
using uc10_Locatem.Model;
using uc10_Locatem.Model.DTO;

//Se somente usuários logados puderem pesquisar, adicione: using Microsoft.AspNetCore.Authorization;
//e acima da classe: [Authorize]


namespace uc10_Locatem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BuscaController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BuscaController(AppDbContext context)
        {
            _context = context;
        }

        /*
          GET: /api/Busca
         
          Busca geral com filtros combinados.
         */
        [HttpGet]
        public async Task<IActionResult> Buscar(
            [FromQuery] BuscarFerramentasNoSistema filtros)
        {
            return await ExecutarBusca(filtros);
        }

        /*
          GET: /api/Busca/categoria/1         
          Busca por categoria.
          Outros filtros também podem ser enviados junto.
         */
        [HttpGet("categoria/{idCategoria:int}")]
        public async Task<IActionResult> BuscarPorCategoria(
            int idCategoria,
            [FromQuery] BuscarFerramentasNoSistema filtros)
        {
            if (idCategoria <= 0)
            {
                return BadRequest(
                    "O ID da categoria deve ser maior que zero.");
            }

            return await ExecutarBusca(
                filtros,
                idCategoria);
        }

        /*
          GET: /api/Busca/localizacao       
          Exemplos:
          /api/Busca/localizacao?cidade=São Paulo
          /api/Busca/localizacao?cidade=São Paulo&estado=SP
         */
        [HttpGet("localizacao")]
        public async Task<IActionResult> BuscarPorLocalizacao(
            [FromQuery] BuscarFerramentasNoSistema filtros)
        {
            if (string.IsNullOrWhiteSpace(filtros.Cidade) &&
                string.IsNullOrWhiteSpace(filtros.Estado))
            {
                return BadRequest(
                    "Informe a cidade ou o estado.");
            }

            return await ExecutarBusca(filtros);
        }

        /*
          GET: /api/Busca/disponibilidade         
          Exemplo:         
          /api/Busca/disponibilidade
         /api/Busca/disponibilidade?disponibilidade=Disponivel
         */
        [HttpGet("disponibilidade")]
        public async Task<IActionResult> BuscarPorDisponibilidade(
            [FromQuery] BuscarFerramentasNoSistema filtros)
        {
            return await ExecutarBusca(filtros);
        }

        /*
          Método central da busca.
          Todos os endpoints passam por ele para que os filtros
          funcionem individualmente ou combinados.
         */
        private async Task<IActionResult> ExecutarBusca(
            BuscarFerramentasNoSistema filtros,
            int? categoriaDaRota = null)
        {
            if (filtros.PrecoMin.HasValue &&
                filtros.PrecoMin.Value < 0)
            {
                return BadRequest(
                    "O preço mínimo não pode ser negativo.");
            }

            if (filtros.PrecoMax.HasValue &&
                filtros.PrecoMax.Value < 0)
            {
                return BadRequest(
                    "O preço máximo não pode ser negativo.");
            }

            if (filtros.PrecoMin.HasValue &&
                filtros.PrecoMax.HasValue &&
                filtros.PrecoMin.Value > filtros.PrecoMax.Value)
            {
                return BadRequest(
                    "O preço mínimo não pode ser maior que o preço máximo.");
            }

            var categoriaId =
                categoriaDaRota ?? filtros.CategoriaId;

            IQueryable<Ferramenta> query =
                _context.Ferramenta
                    .AsNoTracking()
                    .AsSplitQuery()
                    .Include(f => f.Usuario)
                    .ThenInclude(u => u.Enderecos)
                    .Include(f => f.Imagens)
                    .Where(f =>
                        f.Status == StatusCadastro.Ativo);

            /*
              Regra padrão:
              sem filtro de disponibilidade, mostrar apenas
              ferramentas disponíveis para aluguel.
              Se o usuário informar uma disponibilidade específica,
              ela será respeitada.
             */
            if (filtros.Disponibilidade.HasValue)
            {
                query = query.Where(f =>
                    f.Disponibilidade ==
                    filtros.Disponibilidade.Value);
            }
            else
            {
                query = query.Where(f =>
                    f.Disponibilidade ==
                    StatusDisponibilidade.Disponivel);
            }

           // Filtro por categoria.

            if (categoriaId.HasValue)
            {
                query = query.Where(f =>
                    f.CategoriaId == categoriaId.Value);
            }

            /*
              Busca textual em:
              -Nome
              - Marca
              - Modelo
              - Descrição
              - Acessórios
             */
            if (!string.IsNullOrWhiteSpace(filtros.Texto))
            {
                var texto = filtros.Texto.Trim();
                var padrao = $"%{texto}%";

                query = query.Where(f =>
                    EF.Functions.Like(f.Nome, padrao) ||
                    EF.Functions.Like(f.Marca, padrao) ||
                    EF.Functions.Like(f.Modelo, padrao) ||
                    EF.Functions.Like(f.Descricao, padrao) ||
                    EF.Functions.Like(
                        f.Acessorios ?? string.Empty,
                        padrao));
            }

            //filtro por faixa de preço

            if (filtros.PrecoMin.HasValue)
            {
                query = query.Where(f =>
                    f.Diaria >= filtros.PrecoMin.Value);
            }

            if (filtros.PrecoMax.HasValue)
            {
                query = query.Where(f =>
                    f.Diaria <= filtros.PrecoMax.Value);
            }

            /*
              Filtro por cidade usando os endereços
              do proprietário da ferramenta.
             */
            if (!string.IsNullOrWhiteSpace(filtros.Cidade))
            {
                var cidade = filtros.Cidade.Trim();
                var padraoCidade = $"%{cidade}%";

                query = query.Where(f =>
                    f.Usuario.Enderecos.Any(e =>
                        EF.Functions.Like(
                            e.Cidade,
                            padraoCidade)));
            }

            /*
              Filtro por estado usando os endereços
              do proprietário da ferramenta.
             */
            if (!string.IsNullOrWhiteSpace(filtros.Estado))
            {
                var estado = filtros.Estado.Trim();
                var padraoEstado = $"%{estado}%";

                query = query.Where(f =>
                    f.Usuario.Enderecos.Any(e =>
                        EF.Functions.Like(
                            e.Estado,
                            padraoEstado)));
            }

            //ordenação dos resultados

            var ordenacao =
                filtros.OrdenarPor?
                    .Trim()
                    .ToLowerInvariant();

            query = ordenacao switch
            {
                "menor-preco" =>
                    query.OrderBy(f => f.Diaria),

                "menorpreco" =>
                    query.OrderBy(f => f.Diaria),

                "maior-preco" =>
                    query.OrderByDescending(f => f.Diaria),

                "maiorpreco" =>
                    query.OrderByDescending(f => f.Diaria),

                "alfabetica" =>
                    query.OrderBy(f => f.Nome),

                "ordem-alfabetica" =>
                    query.OrderBy(f => f.Nome),

                "recentes" =>
                    query.OrderByDescending(f => f.DataCadastro),

                _ =>
                    query.OrderByDescending(f => f.DataCadastro)
            };

            var ferramentas = await query.ToListAsync();

            /*
              Monta uma resposta própria, evitando retornar
              diretamente as entidades do Entity Framework.
             */
            var resultado = ferramentas
                .Select(f =>
                {
                    var endereco = f.Usuario?.Enderecos?
                        .Where(e =>
                            e.Latitude.HasValue &&
                            e.Longitude.HasValue)
                        .OrderByDescending(e => e.EhPrioritario)
                        .FirstOrDefault();

                    return new ResultadoBuscaDTO
                    {
                        FerramentaId = f.FerramentaId,
                        Nome = f.Nome,
                        Marca = f.Marca,
                        Modelo = f.Modelo,
                        Descricao = f.Descricao,
                        Acessorios = f.Acessorios,
                        Diaria = f.Diaria,
                        Caucao = f.Caucao,
                        CategoriaId = f.CategoriaId,
                        UsuarioId = f.UsuarioId,
                        Status = f.Status,
                        Disponibilidade = f.Disponibilidade,
                        // DataCadastro = f.DataCadastro,

                        //EnderecoId = endereco?.Id,
                        Logradouro = endereco?.Logradouro,
                        Numero = endereco?.Numero,
                        Bairro = endereco?.Bairro,
                        Cidade = endereco?.Cidade,
                        Estado = endereco?.Estado,
                        CEP = endereco?.CEP,

                        Fotos = f.Imagens
                        .OrderBy(i => i.Id)
                        .Select(i => new FotoFerramentaBuscaDTO
                        {
                            Id = i.Id,
                            UrlImagem = i.UrlImagem
                        })
                        .ToList()
                    };
                })
                .ToList();
                     

            /*
              Busca sem resultado retorna 200 com array vazio.
              Isso facilita o tratamento no frontend.
             */
            return Ok(resultado);
        }
    }
}