namespace uc10_Locatem.Model.DTO
{
    public class ResponseAlterarSenha
    {
        public bool Erro { get; set; }
        public string Message { get; set; } = string.Empty;
        public Usuario? Usuario { get; set; }
    }
}
