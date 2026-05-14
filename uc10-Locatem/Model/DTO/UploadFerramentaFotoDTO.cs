using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace uc10_Locatem.Model.DTO
{
    public class UploadFerramentaFotoDTO
    {
        [Required]
        public int FerramentaId { get; set; }
        //// Enviar múltiplos campos "Fotos" no form-data, por isso todos devem ser escritos como "Fotos" no primeiro campo.
        public List<IFormFile> Fotos { get; set; } = new();

        //public List<IFormFile>? Fotos { get; set; }
        // [Required]
        // public IFormFile? Foto { get; set; }
    }
}