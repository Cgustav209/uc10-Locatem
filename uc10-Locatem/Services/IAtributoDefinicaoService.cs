using uc10_Locatem.Model.DTO;

namespace uc10_Locatem.Services
{
    public class IAtributoDefinicaoService
    {

        public interface IAtributoDefinicaoService
        {
            Task<IReadOnlyList<AtributoDefinicaoDto>> ListByCategoriaAsync(int categoriaId, CancellationToken ct);
            Task<AtributoDefinicaoDto> CreateAsync(AtributoDefinicaoCreateUpdateDto dto, CancellationToken ct);
            Task<AtributoDefinicaoDto> UpdateAsync(int id, AtributoDefinicaoCreateUpdateDto dto, CancellationToken ct);
            Task DeleteAsync(int id, CancellationToken ct);
        }

    }
}
