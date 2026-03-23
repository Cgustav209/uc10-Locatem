using uc10_Locatem.Enum;

namespace uc10_Locatem.Model.DTO
{
    public class CriarAluguelDTO
    {
        public int FerramentaId { get; set; }
        public int UsuarioId { get; set; } 
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set;}
    }
}
