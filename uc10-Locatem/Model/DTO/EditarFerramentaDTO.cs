using System.ComponentModel.DataAnnotations;
using uc10_Locatem.API.Model.DTO;

namespace uc10_Locatem.Model.DTO
{
    public class EditarFerramentaDTO
    {
        public string Nome { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public List<string>? Acessorios { get; set; } = new();
        public decimal Diaria { get; set; }
        public decimal Caucao { get; set; }
        public int CategoriaId { get; set; }

        [Range(1, 999, ErrorMessage = "A quantidade disponível deve estar entre 1 e 999 unidades.")]
        public int? QuantidadeDisponivel { get; set; }
        public string? EstadoConservacao { get; set; }
        public string? FonteAlimentacao { get; set; }
        public List<EspecificacaoTecnicaDTO>? EspecificacoesTecnicas { get; set; }
        public List<string>? DiasIndisponiveis { get; set; }
        public string? TipoAprovacao { get; set; }
        public CriarEnderecoDTO? EnderecoRetirada { get; set; }
    }
}
