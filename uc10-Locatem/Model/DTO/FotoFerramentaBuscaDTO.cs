namespace uc10_Locatem.Model.DTO;

public class FotoFerramentaBuscaDTO
{
    public int Id { get; set; }

    public string UrlImagem { get; set; } = string.Empty;
}
//este arquivo é usado para retornar a url da imagem da ferramenta na busca, para que o front-end possa exibir a imagem da ferramenta na tela de resultados da busca.