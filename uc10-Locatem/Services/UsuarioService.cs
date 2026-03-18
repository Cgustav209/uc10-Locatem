using Microsoft.EntityFrameworkCore;
using uc10_Locatem.Data;
using uc10_Locatem.Model;

namespace uc10_Locatem.Services
{
    public class UsuarioService
    {
        private readonly AppDbContext _context;

        public UsuarioService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Usuario?> GetUserByEmail(string email)
        {
            return await _context.Usuario
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        }
    }
}
