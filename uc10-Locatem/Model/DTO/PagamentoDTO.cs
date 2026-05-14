namespace uc10_Locatem.Model.DTO
{
    public class PagamentoDTO
    {
        public int Id { get; set; }
        public int AluguelId { get; set; }
        public int UsuarioId { get; set; }
        public string Status { get; set; } = string.Empty;
        public string MetodoPagamento { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public DateTime DataPagamento { get; set; }
        public decimal ValorTotal { get; set; }
        public string FormadePagamento { get; set; } = string.Empty;
        public string QuantidadeParcelas { get; set; } = string.Empty;
    }
}
