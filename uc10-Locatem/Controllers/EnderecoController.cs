using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using uc10_Locatem.API.Model;
using uc10_Locatem.API.Model.DTO;
using uc10_Locatem.Data;
using uc10_Locatem.Model;

namespace uc10_Locatem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // 🔐 protege TODOS os endpoints
    public class EnderecoController : ControllerBase
    {
        private readonly AppDbContext _enderecoDbContext;

        public EnderecoController(AppDbContext context)
        {
            _enderecoDbContext = context;
        }

        // 🔥 MÉTODO CENTRALIZADO (evita repetição)
        private int? ObterUsuarioId()
        {
            var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst("sub")?.Value
                ?? User.FindFirst("id")?.Value;

            if (string.IsNullOrEmpty(usuarioIdClaim))
                return null;

            return int.Parse(usuarioIdClaim);
        }

        [HttpPost("CadastrarEnderecos")]
        public async Task<IActionResult> CadastrarEnderecos([FromBody] List<CriarEnderecoDTO> enderecosDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var usuarioId = ObterUsuarioId();
            if (usuarioId == null)
                return Unauthorized("Usuário não autenticado");

            if (enderecosDto == null || !enderecosDto.Any())
                return BadRequest("Nenhum endereço informado");

            var enderecos = enderecosDto.Select(dto => new Endereco
            {
                Logradouro = dto.Logradouro,
                Numero = dto.Numero,
                Complemento = dto.Complemento,
                Bairro = dto.Bairro,
                Cidade = dto.Cidade,
                Estado = dto.Estado,
                CEP = dto.CEP,
                TipoEndereco = dto.TipoEndereco,
                UsuarioId = usuarioId.Value
            }).ToList();

            await _enderecoDbContext.Endereco.AddRangeAsync(enderecos);
            await _enderecoDbContext.SaveChangesAsync();

            return Ok(new
            {
                mensagem = "Endereços cadastrados com sucesso",
                quantidade = enderecos.Count,
                ids = enderecos.Select(e => e.Id)
            });
        }

        [HttpDelete("DeletarEndereco/{id}")]
        public async Task<IActionResult> DeletarEndereco(int id)
        {
            var usuarioId = ObterUsuarioId();
            if (usuarioId == null)
                return Unauthorized("Usuário não autenticado");

            var endereco = await _enderecoDbContext.Endereco
                .FirstOrDefaultAsync(e => e.Id == id && e.UsuarioId == usuarioId);

            if (endereco == null)
                return NotFound($"Endereço com ID {id} não encontrado para este usuário");

            _enderecoDbContext.Endereco.Remove(endereco);
            await _enderecoDbContext.SaveChangesAsync();

            return Ok($"Endereço com ID {id} deletado com sucesso");
        }

        [HttpPut("AtualizarEndereco/{id}")]
        public async Task<IActionResult> AtualizarEndereco(int id, [FromBody] CriarEnderecoDTO dadosEndereco)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var usuarioId = ObterUsuarioId();
            if (usuarioId == null)
                return Unauthorized("Usuário não autenticado");

            var enderecoExistente = await _enderecoDbContext.Endereco
                .FirstOrDefaultAsync(e => e.Id == id && e.UsuarioId == usuarioId);

            if (enderecoExistente == null)
                return NotFound($"Endereço com ID {id} não encontrado para este usuário");

            enderecoExistente.Logradouro = dadosEndereco.Logradouro;
            enderecoExistente.Numero = dadosEndereco.Numero;
            enderecoExistente.Complemento = dadosEndereco.Complemento;
            enderecoExistente.Bairro = dadosEndereco.Bairro;
            enderecoExistente.Cidade = dadosEndereco.Cidade;
            enderecoExistente.Estado = dadosEndereco.Estado;
            enderecoExistente.CEP = dadosEndereco.CEP;
            enderecoExistente.TipoEndereco = dadosEndereco.TipoEndereco;

            await _enderecoDbContext.SaveChangesAsync();

            return Ok(new
            {
                mensagem = $"Endereço com ID {id} atualizado com sucesso",
                enderecoExistente
            });
        }

        [HttpGet("MeusEnderecos")]
        public async Task<IActionResult> MeusEnderecos()
        {
            var usuarioId = ObterUsuarioId();
            if (usuarioId == null)
                return Unauthorized("Usuário não autenticado");

            var enderecos = await _enderecoDbContext.Endereco
                .Where(e => e.UsuarioId == usuarioId)
                .ToListAsync();

            return Ok(enderecos);
        }
    }
}