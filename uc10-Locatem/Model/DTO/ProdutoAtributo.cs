namespace uc10_Locatem.Model.DTO
{
    public class ProdutoAtributo
    {
        public int ProductId { get; set; }               // FK -> Ferramenta.Id
        public Ferramenta Product { get; set; } = default!;
        public int AttributeId { get; set; }             // FK -> AtributoDefinicao.Id
        public AtributoDefinicao Attribute { get; set; } = default!;

        // Um campo por tipo (preencher só o correspondente ao DataType)
        public string? ValueText { get; set; } // tributo de texto livre ex: "Vermelho", "220V", "Metal"
        public decimal? ValueNumber { get; set; } // atributo numérico ex: 220, 0.5, 10
        public bool? ValueBool { get; set; } // atributo booleano ex: true (sim), false (não)
        public string? ValueEnum { get; set; } // atributo enum (lista pré-definida) ex: "Pequeno", "Médio", "Grande"

    }
}
