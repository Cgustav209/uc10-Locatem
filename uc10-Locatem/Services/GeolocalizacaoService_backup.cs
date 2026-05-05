using Microsoft.EntityFrameworkCore;
using uc10_Locatem.Model;
using uc10_Locatem.Data;

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
            double lat1, double lon1,
            double lat2, double lon2)
        {
            var R = 6371; // raio da Terra em KM

            var dLat = (lat2 - lat1) * Math.PI / 180;
            var dLon = (lon2 - lon1) * Math.PI / 180;

            var a =
                Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(lat1 * Math.PI / 180) *
                Math.Cos(lat2 * Math.PI / 180) *
                Math.Sin(dLon / 2) *
                Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return R * c;
        }

        public async Task<List<Ferramenta>> BuscarPorRaio(
            double latUsuario,
            double lonUsuario,
            double raioKm)
        {
            var ferramentas = await _context.Ferramenta.ToListAsync();
            

            return ferramentas
                .Where(f =>
                    CalcularDistancia(
                        latUsuario,
                        lonUsuario,
                        f.Latitude,
                        f.Longitude
                    ) <= raioKm
                )
                .ToList();
        }
    }
}