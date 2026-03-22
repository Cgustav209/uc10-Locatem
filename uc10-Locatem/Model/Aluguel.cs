using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using uc10_Locatem.Enum;

namespace uc10_Locatem.Model
{
    public class Aluguel
    {
        // [ForeignKey(nameof(FerramentaId))]
        // [JsonIgnore]
        // public Usuario Usuario { get; set; } = null!;

        [Key] // indica a chave primária
        public int Id { get; set; }

        public int FerramentaId { get; set; } // chave estrangeira = liga o aluguel com a ferramenta.

        public DateTime DataInicio { get; set; } // define o período de uso.
        public DateTime DataFim { get; set; } // define o período de uso.

        public StatusAluguel Status { get; set; } = StatusAluguel.AguardandoPagamento; // define o estado do aluguel (puxa do enum "StatusAluguel.cs")

        public decimal ValorTotal { get; set; } // valor do aluguel da ferramenta

        //para o perfil
        public int UsuarioId { get; set;} 
    }
}
