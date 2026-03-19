namespace uc10_Locatem.Model
{
    public class Categoria
    {
        public int Id { get; set; } // Identificador único guid evitará problemas de concorrência e merge
        public string Name { get; set; } = default!;   // Nome exibido
        public string Slug { get; set; } = default!;   // nome-url (sem acento/espaços)
        public int ParentId { get; set; }            // Pai (nulo = raiz)
        public Categoria? Parent { get; set; }
        public List<Categoria> Children { get; set; } = new(); // navegação Entidade FRamework
        public string FullPath { get; set; } = default!; // caminho "pai/filho"
        public int Level { get; set; }                   // nível na árvore (raiz=0)
        public int SortOrder { get; set; }               // ordenação no menu
        public bool IsActive { get; set; } = true;       // soft delete
    }

}

