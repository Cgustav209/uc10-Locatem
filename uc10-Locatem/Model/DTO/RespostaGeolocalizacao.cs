using System.Text.Json.Serialization;

namespace uc10_Locatem.Services.DTOs
{
    public class RespostaGeolocalizacao
    {
        [JsonPropertyName("lat")]
        public string Latitude { get; set; }

        [JsonPropertyName("lon")]
        public string Longitude { get; set; }
    }
}