using uc10_Locatem.Enum;
namespace uc10_Locatem.Model.DTO;

public class ResultadoBuscaDTO
{
    public int FerramentaId { get; set; } 
    public string Nome { get; set; } = string.Empty;
    public string Marca { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public string? Acessorios { get; set; }
    public decimal Diaria { get; set; }
    public decimal Caucao { get; set; }
    public int CategoriaId { get; set; }
    public int UsuarioId { get; set; }
    public StatusCadastro Status { get; set; }
    public StatusDisponibilidade Disponibilidade { get; set; }
  //public DateTime DataCadastro { get; set; }
  //public int? EnderecoId { get; set; }
    public string? Logradouro { get; set; }
    public string? Numero { get; set; }
    public string? Bairro { get; set; }
    public string? Cidade { get; set; }
    public string? Estado { get; set; }
    public string? CEP { get; set; }

    public List<FotoFerramentaBuscaDTO> Fotos { get; set; } = new();
}