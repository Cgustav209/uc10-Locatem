using uc10_Locatem.Enum;

namespace uc10_Locatem.API.Model.DTO
{
    public class CriarEnderecoDTO
    {
        public string Logradouro { get; set; } = string.Empty;
        public string Numero {  get; set; } = string.Empty;
        public string Complemento {  get; set; } = string.Empty;
        public string Bairro { get; set; } = string.Empty;
        public string Cidade {  get; set; } = string.Empty;
        public string Estado {  get; set; } = string.Empty;
        public string CEP {  get; set; } = string.Empty;
        public TipoEndereco TipoEndereco { get; set; }
        public int UsuarioId { get; set; }
        public bool EhPrioritario { get; internal set; }
    }
}
