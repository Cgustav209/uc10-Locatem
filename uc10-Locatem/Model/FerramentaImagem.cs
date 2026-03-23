namespace uc10_Locatem.Model
{
    public class FerramentaImagem
    {
        public int Id { get; set; }

        public int FerramentaId { get; set; }
        public Ferramenta Ferramenta { get; set; }

        public string UrlImagem { get; set; }
    }
}