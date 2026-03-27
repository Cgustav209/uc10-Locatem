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

        internal async Task AtualizarUsuario(int id, AtualizarUsuarioDTO dto)
        {
            throw new NotImplementedException();
        }
        public async Task<Usuario?> BuscarPorId(int id)
        {
            return await _context.Usuario
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task SalvarAlteracoes()
        {
            await _context.SaveChangesAsync();
        }
    }
}
