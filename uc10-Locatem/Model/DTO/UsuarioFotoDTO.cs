using Microsoft.AspNetCore.Http;

namespace uc10_Locatem.Model.DTO
{
    public class UsuarioFotoDTO
    {
        public int UsuarioId { get; set; }

        public IFormFile Foto { get; set; } = null!;
    }
}