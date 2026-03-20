using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using uc10_Locatem.Enum;

namespace uc10_Locatem.Model
{
    public class Aluguel
    {
        [Key] // indica a chave primária
        public int Id { get; set; }

        public int FerramentaId { get; set; } // chave estrangeira = liga o aluguel com a ferramenta.

        public DateTime DataInicio { get; set; } // define o início do período de uso.
        public DateTime DataFim { get; set; } // define o final do período de uso.
        public DateTime DataCriacao { get; set; } // data da criação do aluguel

        public StatusAluguel Status { get; set; } = StatusAluguel.AguardandoPagamento; // define o estado do aluguel (puxa do enum "StatusAluguel.cs")

        public decimal ValorTotal { get; set; } // valor do aluguel da ferramenta

        // Caução
        public decimal ValorCaucao { get; set; }
        public bool CaucaoRetida { get; set; } = false;

        // Locador
        public int LocadorId { get; set; }

        [ForeignKey(nameof(LocadorId))]
        public Usuario Locador { get; set; } = null!;


        // Locatário
        public int LocatarioId { get; set; }

        [ForeignKey(nameof(LocatarioId))]
        public Usuario Locatario { get; set; } = null!;


        // só cria aluguel se reserva estiver aceita
        public int ReservaId { get; set; }
    }
}
