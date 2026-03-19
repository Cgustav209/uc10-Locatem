using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;


namespace uc10_Locatem.Model
{
    public class Ferramenta
    {
        [Key]
        public int FerramentaId { get; set; }

        [Required(ErrorMessage = "Campo Obrigatorio")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Campo Obrigatorio")]
        public string Marca { get; set; } = string.Empty;

        [Required(ErrorMessage = "Campo Obrigatorio")]
        public string Modelo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Campo Obrigatorio")]
        public string Descricao { get; set; } = string.Empty;

        [Required(ErrorMessage = "Campo Obrigatorio")]
        public string Acessorios { get; set; } = string.Empty;

        [Required(ErrorMessage = "Campo Obrigatorio")]
        public int Diaria { get; set; }

        [Required(ErrorMessage = "Campo Obrigatorio")]
        public int Caucao { get; set; }

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
  