namespace uc10_Locatem.Services
{
    public class IProdutoBuscaService
    {

        public interface IProdutoBuscaService
        {
            Task<ProdutoBuscaResponseDto> SearchAsync(ProdutoBuscaRequestDto request, CancellationToken ct);
            Task<ProdutoDetalheDto?> GetBySlugAsync(string slug, CancellationToken ct);
        }

    }
}
