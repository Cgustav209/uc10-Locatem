using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UploadController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UploadController(AppDbContext context)
        {
            _context = context;
        }

        // FOTO PERFIL
        // ------
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

        // DELETAR FOTO PERFIL
        // ---------
        [HttpDelete("foto-perfil/{usuarioId} excluir")]
        public async Task<IActionResult> DeletarFotoPerfil(int usuarioId)
        {
            var perfil = await _context.UsuarioPerfis
                .FirstOrDefaultAsync(p => p.UsuarioId == usuarioId);

            if (perfil == null)
                return NotFound("Perfil não encontrado");

            var caminhoFisico = Path.Combine(
                Directory.GetCurrentDirectory(),
                perfil.UrlFoto.Replace("/", "\\")
                    .Replace($"{Request.Scheme}://{Request.Host}\\", "")
            );

            if (System.IO.File.Exists(caminhoFisico))
                System.IO.File.Delete(caminhoFisico);

            _context.UsuarioPerfis.Remove(perfil);

            await _context.SaveChangesAsync();

            return Ok("Foto de perfil deletada com sucesso");
        }

        // FOTO FERRAMENTA (MULTIPLAS)
        // -----------
        [HttpPost("foto-ferramenta")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadFotoFerramenta(
    [FromForm] UploadFerramentaFotoDTO dto)
        {
            // usuário logado
            var usuarioId = User.FindFirst("id")?.Value;

            if (usuarioId == null)
                return Unauthorized("Usuário não autenticado");

            int idUser = int.Parse(usuarioId);

            // validar arquivos
            if (dto.Fotos == null || dto.Fotos.Count == 0)
                return BadRequest("Nenhum arquivo enviado");

            // buscar ferramenta
            var ferramenta = await _context.Ferramenta
                .FirstOrDefaultAsync(f => f.FerramentaId == dto.FerramentaId);

            if (ferramenta == null)
                return NotFound("Ferramenta não encontrada");

            // validar dono
            if (ferramenta.UsuarioId != idUser)
                return Forbid();

            //  salvar imagens
            var urls = new List<string>();

            foreach (var foto in dto.Fotos)
            {
                var urlImagem = await SalvarArquivo(foto);

                var imagem = new FerramentaImagem
                {
                    FerramentaId = dto.FerramentaId,
                    UrlImagem = urlImagem
                };

                _context.FerramentaImagens.Add(imagem);
                urls.Add(urlImagem);
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensagem = "Imagem(ns) adicionada(s) com sucesso",
                imagem = urls
            });
        }
        // LISTAR FOTOS DA FERRAMENTA
        // ------------
        [HttpGet("ferramenta/{ferramentaId}/listarfotos")]
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

        // DELETAR FOTO
        //-----------
        [HttpDelete("fotoferramentaexcluir/{id}")]
        public async Task<IActionResult> DeletarFoto(int id)
        {
            //  verificar usuário logado
            var usuarioId = User.FindFirst("id")?.Value;

            if (usuarioId == null)
                return Unauthorized("Usuário não autenticado");

            int idUser = int.Parse(usuarioId);

            //  buscar imagem + ferramenta
            var imagem = await _context.FerramentaImagens
                .Include(i => i.Ferramenta)
                .FirstOrDefaultAsync(i => i.Id == id);

            //  verificar existência
            if (imagem == null)
                return NotFound("Imagem não encontrada");

            //  verificar dono
            if (imagem.Ferramenta.UsuarioId != idUser)
                return Forbid();

            //  deletar arquivo físico
            var caminhoFisico = Path.Combine(
                Directory.GetCurrentDirectory(),
                imagem.UrlImagem.Replace("/", "\\")
                    .Replace($"{Request.Scheme}://{Request.Host}\\", "")
            );

            if (System.IO.File.Exists(caminhoFisico))
                System.IO.File.Delete(caminhoFisico);

            //  remover do banco
            _context.FerramentaImagens.Remove(imagem);
            await _context.SaveChangesAsync();

            return Ok("Imagem deletada com sucesso");
        }

        // MÉTODO PRIVADO (REUTILIZÁVEL)
        // --------
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