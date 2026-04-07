namespace uc10_Locatem.Model
{
    public class AtributoDefinicao
    {

        public int Id { get; set; }
        public int CategoriaId { get; set; }
        public Categoria Categoria { get; set; } = default!; // navegação Entidade Fremework
        public string Volt { get; set; } = default!;      // "Voltagem"
        public string VoltKey { get; set; } = default!;       // "voltagem" (sem acento/esp)
        public string DataType { get; set; } = default!;  // "text" | "number" | "bool" | "enum"
        public bool IsFilterable { get; set; } = true;    // aparece como faceta
        public bool IsSearchable { get; set; } = false;   // entra na busca texto
        public string? UniM { get; set; }                 // Unidade de medida "V", "mm"
        public int SortOrder { get; set; }                // ordem que o atributo e apresentado  da faceta
    }

}

