using uc10_Locatem.Enum;

namespace uc10_Locatem.Model.DTO;

public class BuscarFerramentasNoSistema
{
    public string? Texto { get; set; }
    public int? CategoriaId { get; set; }
    public string? Cidade { get; set; }
    public string? Estado { get; set; }
    public decimal? PrecoMin { get; set; }
    public decimal? PrecoMax { get; set; }
    public StatusDisponibilidade? Disponibilidade { get; set; }
    /*
      Valores aceitos:         
      menor-preco
      maior-preco
      recentes
      alfabetica
     */
    public string? OrdenarPor { get; set; } = "recentes";
}