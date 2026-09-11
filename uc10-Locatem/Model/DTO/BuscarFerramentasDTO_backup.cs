using System.ComponentModel.DataAnnotations;

namespace uc10_Locatem.Model.DTO; 

public class BuscarFerramentasDTO
{
    public double? LatitudeUsuario { get; set; }
    public double? LongitudeUsuario { get; set; }

    public string? Endereco { get; set; }
    [Range(
            0.1,
            500,
            ErrorMessage = "O raio deve estar entre 0,1 e 500 quilômetros.")]

    public double RaioKm { get; set; }
    public int? CategoriaId { get; set; }
}

public class ResultadoBuscaFerramentaDTO
{
    public int FerramentaId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public int UsuarioId { get; set; }
    public double DistanciaKm { get; set; }
    public double LatitudeFerramenta { get; set; }
    public double LongitudeFerramenta { get; set; }
    public int EnderecoId { get; set; }
    public string Logradouro { get; set; } = string.Empty;
    public string Numero { get; set; } = string.Empty;
    public string Complemento { get; set; } = string.Empty;
    public string Bairro { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string CEP { get; set; } = string.Empty;
    public bool EhPrioritario { get; set; }
}