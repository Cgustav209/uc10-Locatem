using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using uc10_Locatem.Data;
using uc10_Locatem.Model;
using uc10_Locatem.Model.DTO;

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

        // =====================================================
        // FOTO PERFIL
        // =====================================================
        [HttpPost("foto-perfil")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadFotoPerfil([FromForm] UploadFotoDTO dto)
        {
            if (dto.Foto == null || dto.Foto.Length == 0)
                return BadRequest("Arquivo inválido");

            var usuarioExiste = await _context.Usuario
                .AnyAsync(u => u.Id == dto.UsuarioId);

            if (!usuarioExiste)
                return NotFound("Usuário não encontrado");

            var urlFoto = await SalvarArquivo(dto.Foto);

            var perfil = await _context.UsuarioPerfis
                .FirstOrDefaultAsync(p => p.UsuarioId == dto.UsuarioId);

            if (perfil == null)
            {
                perfil = new UsuarioPerfil
                {
                    UsuarioId = dto.UsuarioId,
                    UrlFoto = urlFoto
                };

                _context.UsuarioPerfis.Add(perfil);
            }
            else
            {
                perfil.UrlFoto = urlFoto;
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                urlFoto
            });
        }

        // =====================================================
        // FOTO FERRAMENTA (MULTIPLAS)
        // =====================================================
        [HttpPost("foto-ferramenta")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadFotoFerramenta(
            [FromForm] UploadFerramentaFotoDTO dto)
        {
            if (dto.Foto == null || dto.Foto.Length == 0)
                return BadRequest("Arquivo inválido");

            var ferramenta = await _context.Ferramenta
                .FirstOrDefaultAsync(f => f.FerramentaId == dto.FerramentaId);

            if (ferramenta == null)
                return NotFound("Ferramenta não encontrada");

            var urlImagem = await SalvarArquivo(dto.Foto);

            var imagem = new FerramentaImagem
            {
                FerramentaId = dto.FerramentaId,
                UrlImagem = urlImagem
            };

            _context.FerramentaImagens.Add(imagem);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensagem = "Imagem adicionada com sucesso",
                urlImagem
            });
        }

        // =====================================================
        // LISTAR FOTOS DA FERRAMENTA
        // =====================================================
        [HttpGet("ferramenta/{ferramentaId}/fotos")]
        public async Task<IActionResult> ListarFotos(int ferramentaId)
        {
            var existe = await _context.Ferramenta
                .AnyAsync(f => f.FerramentaId == ferramentaId);

            if (!existe)
                return NotFound("Ferramenta não encontrada");

            var fotos = await _context.FerramentaImagens
                .Where(i => i.FerramentaId == ferramentaId)
                .Select(i => new
                {
                    i.Id,
                    i.UrlImagem
                })
                .ToListAsync();

            return Ok(fotos);
        }

        // =====================================================
        // DELETAR FOTO
        // =====================================================
        [HttpDelete("foto/{id}")]
        public async Task<IActionResult> DeletarFoto(int id)
        {
            var imagem = await _context.FerramentaImagens.FindAsync(id);

            if (imagem == null)
                return NotFound("Imagem não encontrada");

            var caminhoFisico = Path.Combine(
                Directory.GetCurrentDirectory(),
                imagem.UrlImagem.Replace("/", "\\")
                    .Replace($"{Request.Scheme}://{Request.Host}\\", "")
            );

            if (System.IO.File.Exists(caminhoFisico))
                System.IO.File.Delete(caminhoFisico);

            _context.FerramentaImagens.Remove(imagem);

            await _context.SaveChangesAsync();

            return Ok("Imagem deletada com sucesso");
        }

        // =====================================================
        // MÉTODO PRIVADO (REUTILIZÁVEL)
        // =====================================================
        private async Task<string> SalvarArquivo(IFormFile arquivo)
        {
            var nomeArquivo = Guid.NewGuid() +
                Path.GetExtension(arquivo.FileName);

            var pastaUploads = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Uploads"
            );

            if (!Directory.Exists(pastaUploads))
                Directory.CreateDirectory(pastaUploads);

            var caminhoCompleto = Path.Combine(pastaUploads, nomeArquivo);

            using var stream = new FileStream(caminhoCompleto, FileMode.Create);
            await arquivo.CopyToAsync(stream);

            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            return $"{baseUrl}/Uploads/{nomeArquivo}";
        }
    }
}