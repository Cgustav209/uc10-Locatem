using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using uc10_Locatem.Data;
using uc10_Locatem.Enum;
using uc10_Locatem.Model;
using uc10_Locatem.Model.DTO;

namespace uc10_Locatem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FavoritosController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public FavoritosController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> ListarFavoritos()
        {
            var usuarioId = ObterUsuarioId();

            if (usuarioId == null)
                return Unauthorized("Usuário não autenticado");

            var favoritos = await _dbContext.Favoritos
                .AsNoTracking()
                .Where(f => f.UsuarioId == usuarioId.Value)
                .OrderByDescending(f => f.DataFavorito)
                .Select(f => new FavoritoResponseDTO
                {
                    Id = f.Id,
                    FerramentaId = f.FerramentaId,
                    DataFavorito = f.DataFavorito
                })
                .ToListAsync();

            return Ok(favoritos);
        }

        [HttpPost("{ferramentaId:int}")]
        public async Task<IActionResult> AdicionarFavorito(int ferramentaId)
        {
            var usuarioId = ObterUsuarioId();

            if (usuarioId == null)
                return Unauthorized("Usuário não autenticado");

            var ferramentaExiste = await _dbContext.Ferramenta
                .AsNoTracking()
                .AnyAsync(f =>
                    f.FerramentaId == ferramentaId &&
                    f.Status == StatusCadastro.Ativo);

            if (!ferramentaExiste)
                return NotFound("Ferramenta não encontrada ou indisponível para favoritar.");

            var favoritoExistente = await _dbContext.Favoritos
                .FirstOrDefaultAsync(f =>
                    f.UsuarioId == usuarioId.Value &&
                    f.FerramentaId == ferramentaId);

            if (favoritoExistente != null)
            {
                return Ok(MapearFavorito(favoritoExistente));
            }

            var favorito = new Favorito
            {
                UsuarioId = usuarioId.Value,
                FerramentaId = ferramentaId,
                DataFavorito = DateTime.UtcNow
            };

            _dbContext.Favoritos.Add(favorito);
            await _dbContext.SaveChangesAsync();

            return Ok(MapearFavorito(favorito));
        }

        [HttpDelete("{ferramentaId:int}")]
        public async Task<IActionResult> RemoverFavorito(int ferramentaId)
        {
            var usuarioId = ObterUsuarioId();

            if (usuarioId == null)
                return Unauthorized("Usuário não autenticado");

            var favorito = await _dbContext.Favoritos
                .FirstOrDefaultAsync(f =>
                    f.UsuarioId == usuarioId.Value &&
                    f.FerramentaId == ferramentaId);

            if (favorito == null)
                return NotFound("Essa ferramenta não está nos seus favoritos.");

            _dbContext.Favoritos.Remove(favorito);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

        private int? ObterUsuarioId()
        {
            var claim = User.FindFirst("id")?.Value;

            return int.TryParse(claim, out var usuarioId)
                ? usuarioId
                : null;
        }

        private static FavoritoResponseDTO MapearFavorito(Favorito favorito)
        {
            return new FavoritoResponseDTO
            {
                Id = favorito.Id,
                FerramentaId = favorito.FerramentaId,
                DataFavorito = favorito.DataFavorito
            };
        }
    }
}
