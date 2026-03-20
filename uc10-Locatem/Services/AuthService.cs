using Microsoft.EntityFrameworkCore;
using uc10_Locatem.Data;
using uc10_Locatem.Model;
using uc10_Locatem.Model.DTO;

namespace uc10_Locatem.Services
{
    public class AuthService
    {
        private readonly UsuarioService _usuarioService;

        public AuthService(UsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        public async Task<Usuario?> Login(LoginDTO dto)
        {
            var usuario = await _usuarioService.GetUserByEmail(dto.Email);

            if (usuario == null)
                return null;

            // verifica senha com BCrypt
            if (!BCrypt.Net.BCrypt.Verify(dto.Senha, usuario.Senha))
                return null;

            return usuario;
        }
    }
}
