using uc10_Locatem.Model.DTO;

namespace uc10_Locatem.Services.Interfaces
{
    public interface IDisponibilidadeService
    {
        Task<DisponibilidadeResponseDTO> VerificarDisponibilidade(VerificarDisponibilidadeDTO dto);

        Task<AgendaDisponibilidadeResponseDTO> ObterAgenda(int ferramentaId);

        Task<ApiResponseDTO> CriarBloqueio(BloqueioDisponibilidadeDTO dto, int usuarioId);
    }
}