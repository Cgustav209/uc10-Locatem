using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using uc10_Locatem.Data;
using uc10_Locatem.Enum;
using uc10_Locatem.Model;
using uc10_Locatem.Model.DTO;

namespace uc10_Locatem.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class FerramentaController : ControllerBase
    {
        private readonly AppDbContext _context;

        public FerramentaController(AppDbContext context)
        {
            _context = context;
        }

        // =====================================================
        // LISTAR TODAS AS FERRAMENTAS
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> GetAllFerramentas()
        {
            var ferramentas = await _context.Ferramenta
                .Include(f => f.categoria)
                .ToListAsync();

            return Ok(ferramentas);
        }

        // =====================================================
        // LISTAR APENAS DISPONÍVEIS
        // =====================================================

        [HttpGet("Disponiveis")]
        public async Task<IActionResult> GetFerramentasDisponiveis()
        {
            var ferramentas = await _context.Ferramenta
                .Where(f =>
                    f.Status == StatusCadastro.Ativo &&
                    f.Disponibilidade == StatusDisponibilidade.Disponivel)
                .ToListAsync();

            return Ok(ferramentas);
        }

        // =====================================================
        // CADASTRAR FERRAMENTA
        // =====================================================

        [HttpPost]
        public async Task<IActionResult> CadastrarFerramenta(
            [FromBody] CadastrarFerramentaDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var usuarioId = User.FindFirst("id")?.Value;
            var tipoUsuario = User.FindFirst("TipoUsuario")?.Value;

            if (usuarioId == null)
                return Unauthorized("Usuário não autenticado");

            if (tipoUsuario != TipoUsuario.Locador.ToString())
                return Unauthorized("Somente locadores podem cadastrar ferramentas");

            int id = int.Parse(usuarioId);

            string acessorios = string.Join(", ",
                dto.Acessorios ?? new List<string>());

            Ferramenta novaFerramenta = new()
            {
                Nome = dto.Nome,
                Marca = dto.Marca,
                Modelo = dto.Modelo,
                Descricao = dto.Descricao,
                Acessorios = acessorios,
                Diaria = dto.Diaria,
                Caucao = dto.Caucao,

                // STATUS DO SISTEMA
                Status = StatusCadastro.Ativo,

                // DISPONIBILIDADE PARA LOCAÇÃO
                Disponibilidade = StatusDisponibilidade.Disponivel,

                CategoriaId = dto.CategoriaId,
                UsuarioId = id
            };

            await _context.Ferramenta.AddAsync(novaFerramenta);

            int resultado = await _context.SaveChangesAsync();

            if (resultado > 0)
                return CreatedAtAction(
                    nameof(GetAllFerramentas),
                    new { id = novaFerramenta.FerramentaId },
                    novaFerramenta);

            return BadRequest("Erro ao cadastrar ferramenta");
        }

        // =====================================================
        // EDITAR FERRAMENTA
        // =====================================================

        [HttpPut("{id}")]
        public async Task<IActionResult> EditarFerramenta(
            int id,
            [FromBody] EditarFerramentaDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var usuarioId = User.FindFirst("id")?.Value;
            var tipoUsuario = User.FindFirst("TipoUsuario")?.Value;

            if (usuarioId == null)
                return Unauthorized("Usuário não autenticado");

            if (tipoUsuario != TipoUsuario.Locador.ToString())
                return Unauthorized("Somente locadores podem editar ferramentas");

            int idUser = int.Parse(usuarioId);

            var ferramenta = await _context.Ferramenta.FindAsync(id);

            if (ferramenta == null)
                return NotFound("Ferramenta não encontrada");

            // VERIFICA SE É O DONO
            if (ferramenta.UsuarioId != idUser)
            {
                return Unauthorized(
                    "Você não tem permissão para editar esta ferramenta");
            }

            string acessorios = string.Join(", ",
                dto.Acessorios ?? new List<string>());

            ferramenta.Nome = dto.Nome;
            ferramenta.Marca = dto.Marca;
            ferramenta.Modelo = dto.Modelo;
            ferramenta.Descricao = dto.Descricao;
            ferramenta.Acessorios = acessorios;
            ferramenta.Diaria = dto.Diaria;
            ferramenta.Caucao = dto.Caucao;
            ferramenta.CategoriaId = dto.CategoriaId;

            await _context.SaveChangesAsync();

            return Ok("Ferramenta atualizada com sucesso");
        }

        // =====================================================
        // ALTERAR DISPONIBILIDADE
        // =====================================================

        [HttpPatch("{id}/Disponibilidade")]
        public async Task<IActionResult> AlterarDisponibilidade(
            int id,
            [FromQuery] StatusDisponibilidade disponibilidade)
        {
            var ferramenta = await _context.Ferramenta.FindAsync(id);

            if (ferramenta == null)
                return NotFound("Ferramenta não encontrada");

            ferramenta.Disponibilidade = disponibilidade;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensagem = "Disponibilidade atualizada",
                ferramentaId = ferramenta.FerramentaId,
                disponibilidade = ferramenta.Disponibilidade
            });
        }

        // =====================================================
        // DESATIVAR FERRAMENTA
        // =====================================================

        [HttpPatch("{id}/Desativar")]
        public async Task<IActionResult> DesativarFerramenta(int id)
        {
            var ferramenta = await _context.Ferramenta.FindAsync(id);

            if (ferramenta == null)
                return NotFound("Ferramenta não encontrada");

            ferramenta.Status = StatusCadastro.Inativo;

            await _context.SaveChangesAsync();

            return Ok("Ferramenta desativada com sucesso");
        }
    }
}