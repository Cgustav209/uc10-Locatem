namespace uc10_Locatem.Model.DTO
{
    public class PerfilPublicoLocadorResponseDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? UrlFoto { get; set; }
        public int Desde { get; set; }
        public string Localizacao { get; set; } = string.Empty;
        public double AvaliacaoMedia { get; set; }
        public int TotalAvaliacoes { get; set; }
        public int FerramentasAnunciadas { get; set; }
        public int LocacoesConcluidas { get; set; }
    }
}
