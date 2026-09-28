using uc10_Locatem.Enum;

namespace uc10_Locatem.Model.DTO
{
    public class FerramentaResponseDTO
    {
        public int FerramentaId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public List<string> Acessorios { get; set; } = new();
        public decimal Diaria { get; set; }
        public decimal Caucao { get; set; }
        public DateTime DataCadastro { get; set; }
        public int CategoriaId { get; set; }
        public string CategoriaNome { get; set; } = string.Empty;
        public int UsuarioId { get; set; }
        public string UsuarioNome { get; set; } = string.Empty;
        public string? UsuarioFotoUrl { get; set; }
        public StatusCadastro Status { get; set; }
        public StatusDisponibilidade Disponibilidade { get; set; }
        public int QuantidadeDisponivel { get; set; }
        public string EstadoConservacao { get; set; } = string.Empty;
        public string FonteAlimentacao { get; set; } = string.Empty;
        public List<EspecificacaoTecnicaDTO> EspecificacoesTecnicas { get; set; } = new();
        public List<string> DiasIndisponiveis { get; set; } = new();
        public string TipoAprovacao { get; set; } = "manual";
        public int? EnderecoId { get; set; }
        public EnderecoFerramentaResponseDTO? EnderecoRetirada { get; set; }
        public string Localizacao { get; set; } = string.Empty;
        public List<FotoFerramentaBuscaDTO> Fotos { get; set; } = new();
        public double AvaliacaoMedia { get; set; }
        public int TotalAvaliacoes { get; set; }
    }
}
