using uc10_Locatem.Enum;

namespace uc10_Locatem.Model.DTO
{
    public class CriarUsuarioDTO
    {
        public string Nome { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Senha { get; set; } = string.Empty;

        public int TipoUsuario { get; set; }

        public string Telefone { get; set; } = string.Empty;

        public string Documeto { get; set; } = string.Empty;

        public string IdGoogle { get; set; } = string.Empty;

    }
}
