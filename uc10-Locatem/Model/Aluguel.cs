using System.ComponentModel.DataAnnotations; 
using System.ComponentModel.DataAnnotations.Schema; 
using System.Text.Json.Serialization; 
using uc10_Locatem.Enum; 

namespace uc10_Locatem.Model
{
    // Representa a tabela "Aluguel" no banco de dados
    public class Aluguel
    {
        // ================= IDENTIFICAÇÃO =================

        [Key] // Define como chave primária (ID único de cada aluguel)
        public int Id { get; set; }


        // ================= RELACIONAMENTOS =================

        public int FerramentaId { get; set; } // FK → indica qual ferramenta está sendo alugada

        public int UsuarioId { get; set; } // FK → usuário que fez o aluguel (locatário)

        public int ReservaId { get; set; } // FK → ligação com a reserva que originou esse aluguel


        // ================= PERÍODO DO ALUGUEL =================

        public DateTime DataInicio { get; set; } // Data em que o aluguel começa

        public DateTime DataFim { get; set; } // Data em que o aluguel termina


        // ================= STATUS =================

        public StatusAluguel Status { get; set; } = StatusAluguel.AguardandoPagamento;
        // Estado atual do aluguel:
        // AguardandoPagamento → Ativo → AguardandoConfirmacao → Finalizado / Atrasado


        // ================= VALORES =================

        public decimal ValorTotal { get; set; } // Valor total do aluguel (diárias + multa, se houver)

        public decimal ValorDevolvido { get; set; } // Valor devolvido ao cliente (ex: devolução da caução)


        // ================= CAUÇÃO =================

        public decimal ValorCaucao { get; set; } // Valor da caução exigida pela ferramenta (garantia)

        public bool CaucaoRetida { get; set; }
        // Indica se a caução foi retida (true) ou devolvida (false)
        // true → houve problema (dano, atraso, etc)
        // false → tudo certo, cliente recebe de volta


        // ================= NAVEGAÇÃO =================

        [ForeignKey(nameof(FerramentaId))] // Diz que essa propriedade usa FerramentaId como chave estrangeira

        public Ferramenta Ferramenta { get; set; } = null!;
        // Objeto completo da ferramenta (permite acessar dados como Diaria, Nome, etc)
        // Ex: aluguel.Ferramenta.Diaria
    }
}