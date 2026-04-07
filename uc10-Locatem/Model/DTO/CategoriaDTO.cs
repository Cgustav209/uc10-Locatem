namespace uc10_Locatem.Model.DTO
{
    public class CategoriaDTO
    {
        public int Id { get; set; } // Identificador único guid evitará problemas de concorrência e merge
        public string Nome { get; set; } = default!;   // Nome exibido
        public string Slug { get; set; } = default!;   // nome-url (sem acento/espaços)
        public int ParentId { get; set; }            // Pai (nulo = raiz)
        public Categoria? Parent { get; set; }
        public List<Categoria> Children { get; set; } = new(); // navegação Entidade FRamework
        public string FullPath { get; set; } = default!; // caminho "pai/filho"
        public int Level { get; set; }                   // nível na árvore (raiz=0)
        public int SortOrder { get; set; }               // ordenação no menu
        public bool Ativo { get; set; } = true;          // soft delete serve para não excluir categorias, apenas marcar como inativas e não exibir mais
    }
}
