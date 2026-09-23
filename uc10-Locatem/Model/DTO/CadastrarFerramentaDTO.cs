using System.ComponentModel.DataAnnotations;
using uc10_Locatem.API.Model.DTO;

namespace uc10_Locatem.Model.DTO
{
    public class CadastrarFerramentaDTO
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
        public int QuantidadeDisponivel { get; set; } = 1;

        public string EstadoConservacao { get; set; } = string.Empty;
        public string FonteAlimentacao { get; set; } = string.Empty;
        public List<EspecificacaoTecnicaDTO>? EspecificacoesTecnicas { get; set; } = new();
        public List<string>? DiasIndisponiveis { get; set; } = new();
        public string TipoAprovacao { get; set; } = "manual";

        // Opcional para manter compatibilidade com clientes antigos: quando omitido,
        // o backend reaproveita um endereço válido já cadastrado para o usuário.
        public CriarEnderecoDTO? EnderecoRetirada { get; set; }
    }
}
