using System.Globalization;
using System.Text.Json;
using uc10_Locatem.Services.DTOs;

namespace uc10_Locatem.Services
{
    public class EnderecoGeolocalizacaoService
    {
        private readonly HttpClient _httpClient;

        public EnderecoGeolocalizacaoService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<(double latitude, double longitude)>
            ObterCoordenadasPorEndereco(string endereco)
        {
            var url =
                $"https://nominatim.openstreetmap.org/search?q={Uri.EscapeDataString(endereco)}&format=json&limit=1";

            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("LOCATEM-App");

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                throw new Exception("Erro ao consultar localização.");

            var json = await response.Content.ReadAsStringAsync();

            var resultado = JsonSerializer.Deserialize<List<RespostaGeolocalizacao>>(json);

            if (resultado == null || !resultado.Any())
                throw new Exception("Endereço não encontrado.");

            var latitude = double.Parse(
                resultado[0].Latitude,
                CultureInfo.InvariantCulture);

            var longitude = double.Parse(
                resultado[0].Longitude,
                CultureInfo.InvariantCulture);

            return (latitude, longitude);
        }
    }
}