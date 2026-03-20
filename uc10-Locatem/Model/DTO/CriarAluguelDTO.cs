namespace uc10_Locatem.Model.DTO
{
    public class CriarAluguelDTO
    {
        public int FerramentaId { get; set; }
        public int LocadorId { get; set; }
        public int LocatarioId { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set;}
    }
}
