using Microsoft.AspNetCore.Mvc;
using uc10_Locatem.Data;
using uc10_Locatem.Model;
using uc10_Locatem.Model.DTO;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace uc10_Locatem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UploadController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UploadController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("foto-perfil")]
        public async Task<IActionResult> UploadFoto([FromForm] UploadFotoDTO dto)
        {
            if (dto.Foto == null || dto.Foto.Length == 0)
                return BadRequest("Arquivo inválido");

            // verifica se usuário existe
            var usuarioExiste = _context.Usuario.Any(u => u.Id == dto.UsuarioId);
            if (!usuarioExiste)
                return BadRequest("Usuário não encontrado");

            // cria nome único
            var nomeArquivo = Guid.NewGuid().ToString() + Path.GetExtension(dto.Foto.FileName);

            var pastaUploads = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            if (!Directory.Exists(pastaUploads))
                Directory.CreateDirectory(pastaUploads);

            var caminhoCompleto = Path.Combine(pastaUploads, nomeArquivo);

            using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
            {
                await dto.Foto.CopyToAsync(stream);
            }

            var pathBanco = $"Uploads/{nomeArquivo}";

            // adiciona ou atualiza perfil
            var perfil = _context.UsuarioPerfis.FirstOrDefault(p => p.UsuarioId == dto.UsuarioId);
            if (perfil == null)
            {
                perfil = new UsuarioPerfil
                {
                    UsuarioId = dto.UsuarioId,
                    UrlFoto = pathBanco
                };
                _context.UsuarioPerfis.Add(perfil);
            }
            else
            {
                perfil.UrlFoto = pathBanco;
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                arquivo = nomeArquivo,
                path = pathBanco
            });
        }
    }
}