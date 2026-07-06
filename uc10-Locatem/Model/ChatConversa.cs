namespace uc10_Locatem.Model
{
    public class ChatConversa
    {
        public int Id { get; set; }

        public int Usuario1Id { get; set; }
        public int Usuario2Id { get; set; }

        public int? ReservaId { get; set; }
        public int? FerramentaId { get; set; }

        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
        public DateTime DataAtualizacao { get; set; } = DateTime.UtcNow;

        public List<ChatMensagem> Mensagens { get; set; }
    }
}