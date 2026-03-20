using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace uc10_Locatem.Model.DTO
{
    public class UploadFotoDTO
    {
        [Required]
        public int UsuarioId { get; set; }

        [Required]
        public IFormFile? Foto { get; set; }
    }
}