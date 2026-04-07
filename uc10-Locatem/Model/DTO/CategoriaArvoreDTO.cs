namespace uc10_Locatem.Model.DTO
{
    public class CategoriaArvoreDTO
    {

        public int Id { get; set; }  // Identificador único guid evitará problemas de concorrência e merge
        public string Nome { get; set; } = null!; // Nome exibido
        public string Slug { get; set; } = null!; // nome-url (sem acento/espaços)
        public string FullPath { get; set; } = null!; // caminho "pai/filho"
        public List<CategoriaArvoreDTO> Children { get; set; } = new(); // navegação Entidade FRamework
    }

}

