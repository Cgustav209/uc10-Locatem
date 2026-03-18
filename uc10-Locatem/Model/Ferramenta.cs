using System.ComponentModel.DataAnnotations;

namespace uc10_Locatem.Model

    //desenvolvida para teste 
{
    public class Ferramenta
    {
        [Key]
        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;
    }
}