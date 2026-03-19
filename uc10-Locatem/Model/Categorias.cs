using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace uc10_Locatem.Model
{
    //Endtidade categorias criada mas incompleta, só para conectar
    //com ferramenta
    public class Categorias
    {
        [Key]
        public int Id { get; set; }
        public string nome { get; set; } = string.Empty;
    }
}
