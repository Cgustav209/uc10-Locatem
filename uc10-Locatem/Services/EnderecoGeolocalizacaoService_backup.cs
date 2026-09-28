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

        public async Task<(double latitude, double longitude)> ObterCoordenadasPorEndereco(
            string endereco,
            string? cep = null)
        {
            var consultas = new List<string>();

            // 1. Endereço completo
            if (!string.IsNullOrWhiteSpace(endereco))
            {
                consultas.Add(endereco);
            }

            // 2. Tenta retirar o bairro/conjunto e manter rua + número + cidade + estado
            var partes = endereco
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Trim())
                .ToList();

            if (partes.Count >= 4)
            {
                var rua = partes[0];
                var cidade = partes[^2];
                var estado = partes[^1];

                consultas.Add($"{rua}, {cidade}, {estado}, Brasil");

                // 3. Somente rua + cidade + estado
                var ruaSemNumero = RemoverNumeroInicial(rua);

                if (!string.IsNullOrWhiteSpace(ruaSemNumero))
                {
                    consultas.Add($"{ruaSemNumero}, {cidade}, {estado}, Brasil");
                }
            }

            // 4. Tenta pelo CEP
            if (!string.IsNullOrWhiteSpace(cep))
            {
                consultas.Add($"{cep}, São Paulo, Brasil");
            }

            foreach (var consulta in consultas.Distinct())
            {
                var url =
                    $"https://nominatim.openstreetmap.org/search" +
                    $"?q={Uri.EscapeDataString(consulta)}" +
                    $"&format=jsonv2" +
                    $"&limit=1" +
                    $"&countrycodes=br";

                _httpClient.DefaultRequestHeaders.UserAgent.Clear();
                _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("LOCATEM-App");

                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                    continue;

                var json = await response.Content.ReadAsStringAsync();

                var resultado =
                    JsonSerializer.Deserialize<List<RespostaGeolocalizacao>>(
                        json,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                if (resultado == null || resultado.Count == 0)
                    continue;

                if (!double.TryParse(
                        resultado[0].Latitude,
                        NumberStyles.Float,
                        CultureInfo.InvariantCulture,
                        out var latitude))
                {
                    continue;
                }

                if (!double.TryParse(
                        resultado[0].Longitude,
                        NumberStyles.Float,
                        CultureInfo.InvariantCulture,
                        out var longitude))
                {
                    continue;
                }

                return (latitude, longitude);
            }

            throw new Exception(
                $"Não foi possível localizar o endereço: '{endereco}'");
        }

        private static string RemoverNumeroInicial(string valor)
        {
            var partes = valor.Split(
                ' ',
                StringSplitOptions.RemoveEmptyEntries);

            if (partes.Length <= 1)
                return valor;

            if (int.TryParse(partes[0], out _))
            {
                return string.Join(' ', partes.Skip(1));
            }

            return valor;
        }
    }
}