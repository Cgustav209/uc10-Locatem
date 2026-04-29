using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace uc10_Locatem.Model
{
    //Endtidade categorias criada mas incompleta, só para conectar
    //com ferramenta
    public class Categoria
    {
        [Key]
        public int Id { get; set; }

        public string nome { get; set; } = string.Empty;

        //FK hierárquica
        public int? CategoriaPaiId { get; set; }

        [ForeignKey(nameof(CategoriaPaiId))] // Especifica a chave estrangeira para a categoria pai
        [JsonIgnore]
        public Categoria? CategoriaPai { get; set; } // Referência à categoria pai (pode ser nula para categorias de nível superior)

        // Subcategorias
        public ICollection<Categoria> Subcategorias { get; set; } = new List<Categoria>();

        // Relacionamento com ferramentas
        public ICollection<Ferramenta> Ferramentas { get; set; } = new List<Ferramenta>();

        // Categoria padrão
        public bool EhPadrao { get; set; } = false;
    }
}
