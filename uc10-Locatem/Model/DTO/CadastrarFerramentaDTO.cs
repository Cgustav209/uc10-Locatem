namespace uc10_Locatem.Model.DTO
{
    public class CadastrarFerramentaDTO
    {
        public string Nome { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        public string Modelo {  get; set; } = string.Empty;
        public string Descricao { get; set;} = string.Empty;
        public string Acessorios {  get; set; } = string.Empty;
        public decimal PrecoDiaria { get; set; }
        public decimal PrecoCaucao { get; set; } 
        public int UsuarioId { get; set; }
        public int CategoriaId { get; set; }    
    }
}
