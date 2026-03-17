namespace uc10_Locatem.Model.DTO
{
    public class CadastrarFerramentaDTO
    {
        public string nome { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        public string Modelo {  get; set; } = string.Empty;
        public string Descricao { get; set;} = string.Empty;
        public string Acesssorios {  get; set; } = string.Empty;
        public int Diaria { get; set; }
        public int Caucao { get; set; } 
    }
}
