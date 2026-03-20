using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;


namespace uc10_Locatem.Model
{
    public class Ferramenta
    {
        [Key]
        public int FerramentaId { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Campo Obrigatório")]
        public string Marca { get; set; } = string.Empty;

        [Required(ErrorMessage = "Campo Obrigatório")]
        public string Modelo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Campo Obrigatório")]
        public string Descricao { get; set; } = string.Empty;

        [Required(ErrorMessage = "Campo Obrigatório")]
        public List<string> Acessorios { get; set; } = ["broca 8mm", "mandril para troca de de broca", "carregador da bateria"];

        [Required(ErrorMessage = "Campo Obrigatório")]
        public int PrecoDiaria { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório")]
        public int PrecoCaucao { get; set; }

        public DateTime DataCadastro { get; set; }

        //categorias da entidade categorias
        public int CategoriaId { get; set; }

        public int UsuarioId { get; set; }

        // FK do id_locador e id_locatario, ainda n finalizado,
        // puxa id do usuario mas ainda nao por tipo
        [ForeignKey(nameof(UsuarioId))]
        [JsonIgnore]
        public Usuario Usuario { get; set; } = null!;

        [ForeignKey(nameof(CategoriaId))]
        [JsonIgnore]
        public Categorias categoria { get; set; } = null!;



    }
}
  
