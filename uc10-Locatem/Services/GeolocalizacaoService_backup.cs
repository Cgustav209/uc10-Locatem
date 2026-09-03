using Microsoft.EntityFrameworkCore;
using uc10_Locatem.Data;
using uc10_Locatem.Enum;
using uc10_Locatem.Model;
using uc10_Locatem.Model.DTO;

namespace uc10_Locatem.Services
{
    public class GeolocalizacaoService
    {
        private readonly AppDbContext _context;

        public GeolocalizacaoService(AppDbContext context)
        {
            _context = context;
        }

        public double CalcularDistancia(
            double latitude1,
            double longitude1,
            double latitude2,
            double longitude2)
        {
            const double raioTerraKm = 6371.0;

            var diferencaLatitude =
                (latitude2 - latitude1) * Math.PI / 180.0;

            var diferencaLongitude =
                (longitude2 - longitude1) * Math.PI / 180.0;

            var latitude1Rad = latitude1 * Math.PI / 180.0;
            var latitude2Rad = latitude2 * Math.PI / 180.0;

            var a =
                Math.Sin(diferencaLatitude / 2) *
                Math.Sin(diferencaLatitude / 2)
                +
                Math.Cos(latitude1Rad) *
                Math.Cos(latitude2Rad) *
                Math.Sin(diferencaLongitude / 2) *
                Math.Sin(diferencaLongitude / 2);

            // Evita pequenos erros numéricos no cálculo
            a = Math.Clamp(a, 0.0, 1.0);

            var c = 2 * Math.Atan2(
                Math.Sqrt(a),
                Math.Sqrt(1 - a));

            return raioTerraKm * c;
        }

        public async Task<List<ResultadoBuscaFerramentaDTO>>
            BuscarPorRaioAsync(
                double latitudeUsuario,
                double longitudeUsuario,
                double raioKm,
                int? categoriaId = null)
        {
            if (latitudeUsuario < -90 || latitudeUsuario > 90)
            {
                throw new ArgumentException(
                    "A latitude deve estar entre -90 e 90.");
            }

            if (longitudeUsuario < -180 || longitudeUsuario > 180)
            {
                throw new ArgumentException(
                    "A longitude deve estar entre -180 e 180.");
            }

            if (raioKm <= 0)
            {
                throw new ArgumentException(
                    "O raio deve ser maior que zero.");
            }

            var query = _context.Ferramenta
                .AsNoTracking()
                .Include(f => f.Usuario)
                .ThenInclude(u => u.Enderecos)
                .Where(f =>
                    f.Status == StatusCadastro.Ativo &&
                    f.Disponibilidade ==
                        StatusDisponibilidade.Disponivel);

            if (categoriaId.HasValue)
            {
                query = query.Where(f =>
                    f.CategoriaId == categoriaId.Value);
            }

            var ferramentas = await query.ToListAsync();

            var resultado =
                new List<ResultadoBuscaFerramentaDTO>();

            foreach (var ferramenta in ferramentas)
            {
                // A ferramenta não possui EnderecoId. Por isso, usamos o endereço do proprietário. O endereço prioritário será escolhido primeiro.
                 
                var endereco = ferramenta.Usuario?.Enderecos?
                    .Where(e =>
                        e.Latitude.HasValue &&
                        e.Longitude.HasValue)
                    .OrderByDescending(e => e.EhPrioritario)
                    .FirstOrDefault();

                
                 // Se o proprietário não possui endereço com latitude e longitude, a ferramenta não pode participar da busca por proximidade.
                 
                if (endereco == null)
                {
                    continue;
                }

                var distancia = CalcularDistancia(
                    latitudeUsuario,
                    longitudeUsuario,
                    endereco.Latitude!.Value,
                    endereco.Longitude!.Value);

                if (distancia > raioKm)
                {
                    continue;
                }
                //novooo
                resultado.Add(new ResultadoBuscaFerramentaDTO
                {
                    FerramentaId = ferramenta.FerramentaId,
                    Nome = ferramenta.Nome,
                    UsuarioId = ferramenta.UsuarioId,
                    DistanciaKm = Math.Round(distancia, 2),
                    EnderecoId = endereco.Id,
                    Logradouro = endereco.Logradouro,
                    Numero = endereco.Numero,
                    Complemento = endereco.Complemento,
                    Bairro = endereco.Bairro,
                    Cidade = endereco.Cidade,
                    Estado = endereco.Estado,
                    CEP = endereco.CEP,
                    EhPrioritario = endereco.EhPrioritario,
                    LatitudeFerramenta = endereco.Latitude!.Value,
                    LongitudeFerramenta = endereco.Longitude!.Value
                });
            }

            return resultado
                .OrderBy(r => r.DistanciaKm)
                .ToList();
        }
    }
}