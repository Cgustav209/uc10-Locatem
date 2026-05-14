namespace uc10_Locatem.Model.DTO
{
    public class DisponibilidadeResponseDTO
    {
        public int FerramentaId { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public bool Disponivel { get; set; }
        public string Mensagem { get; set; } = string.Empty;
    }
}
