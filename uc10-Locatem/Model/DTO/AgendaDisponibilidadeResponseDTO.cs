namespace uc10_Locatem.Model.DTO
{
    public class AgendaDisponibilidadeResponseDTO
    {
        public int FerramentaId { get; set; }
        public List<PeriodoOcupadoDTO> PeriodosOcupados { get; set; } = [];
    }
}
