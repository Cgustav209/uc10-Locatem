using Microsoft.EntityFrameworkCore;
using uc10_Locatem.Data;
using uc10_Locatem.Model;
using uc10_Locatem.Model.DTO;

namespace uc10_Locatem.Services
{
    public class UsuarioService
    {
        private readonly AppDbContext _context;

        public UsuarioService(AppDbContext context)
        {
            _context = context;
        }

        // Buscar por email
        public async Task<Usuario?> GetUserByEmail(string email)
        {
            return await _context.Usuario
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        // Verificar se já existe usuário
        public async Task<bool> UsuarioExiste(string email, string documento)
        {
            return await _context.Usuario
                .AnyAsync(u => u.Email == email || u.Documento == documento);
        }

        // Criar usuário
        public async Task<Usuario> CriarUsuario(Usuario usuario)
        {
            _context.Usuario.Add(usuario);
            await _context.SaveChangesAsync();
            return usuario;
        }

        //atualizar usuario

        public async Task<Usuario> AtualizarUsuario(int id, UpdateUsuarioDTO dto)
        {
            var usuario = await _context.Usuario.FindAsync(id);

            if (usuario == null)
                throw new Exception("Usuário não encontrado");

            if (!string.IsNullOrWhiteSpace(dto.Nome))
                usuario.Nome = dto.Nome;

            if (!string.IsNullOrWhiteSpace(dto.Email))
            {
                bool emailJaExiste = await _context.Usuario
                    .AnyAsync(u => u.Email == dto.Email && u.Id != id);

                if (emailJaExiste)
                    throw new Exception("Este email já está em uso");

                usuario.Email = dto.Email;
            }

            if (!string.IsNullOrWhiteSpace(dto.Telefone))
                usuario.Telefone = dto.Telefone;

            await _context.SaveChangesAsync();

            return usuario;
        }
    }
    
}
