using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace uc10_Locatem.Model.DTO
{
    public class UploadFerramentaFotoDTO
    {
        [Required]
        public int FerramentaId { get; set; }

        [Required]
        public IFormFile? Foto { get; set; }
    }
}