namespace uc10_Locatem.Model.DTO
{
    public class AlterarSenhaDTO
    {
        public string Email { get; set; } = string.Empty;
        public string SenhaAtual { get; set; } = string.Empty;
        public string NovaSenha { get; set; } = string.Empty;
        public string ConfirmarNovaSenha { get; set; } = string.Empty;
    }
}
