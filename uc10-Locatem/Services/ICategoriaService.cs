using uc10_Locatem.Model.DTO;

namespace uc10_Locatem.Services
{
    public class ICategoriaService
    {

        public interface ICategoriaService
        {
            Task<IReadOnlyList<CategoriaArvoreDto>> GetTreeAsync(CancellationToken ct);
            Task<CategoriaDto?> GetByFullPathAsync(string fullPath, CancellationToken ct);
            Task<CategoriaDto> CreateAsync(CategoriaCreateUpdateDto dto, CancellationToken ct);
            Task<CategoriaDto> UpdateAsync(int id, CategoriaCreateUpdateDto dto, CancellationToken ct);
            Task DeleteAsync(int id, bool softDelete, CancellationToken ct);
        }

    }
}
