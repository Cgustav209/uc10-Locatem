using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using uc10_Locatem.Model;

namespace uc10_Locatem.API.Model
{
    public class Endereco
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Logradouro é um valor obrigatorio")]
        public string Logradouro { get; set; } = string.Empty;

        [Required(ErrorMessage = "Numero é um valor obrigatorio")]
        [StringLength(10)]
        public string Numero { get; set; } = string.Empty;

        [StringLength(150)]
        public string Complemente { get; set; } = string.Empty;

        [Required(ErrorMessage = "Bairro é um valor obrigatorio")]
        [StringLength(100)]
        public string Bairro { get; set; } = string.Empty;

        [Required(ErrorMessage = "Cidade é um valor obrigatorio")]
        [StringLength(100)]
        public string Cidade { get; set; } = string.Empty;

        [Required(ErrorMessage = "Estado é um valor obrigatorio")]
        [StringLength(50)]
        public string Estado { get; set; } = string.Empty;

        [Required(ErrorMessage = "CEP é um valor obrigatorio")]
        [StringLength(9)]
        public string CEP { get; set; } = string.Empty;


        public int UsuarioId { get; set; }

        [ForeignKey(nameof(UsuarioId))]
        [JsonIgnore]
        public Usuario Usuario { get; set; } = null!;

    }
    }
