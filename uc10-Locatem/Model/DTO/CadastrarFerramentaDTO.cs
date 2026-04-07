namespace uc10_Locatem.Model.DTO
{
    public class CadastrarFerramentaDTO
    {
        public string Nome { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        public string Modelo {  get; set; } = string.Empty;
        public string Descricao { get; set;} = string.Empty;
        public List<string>? Acessorios { get; set; } = new();
        public int Diaria { get; set; } 
        //public int UsuarioId { get; set; }
        public int CategoriaId { get; set; }    
    }
}
