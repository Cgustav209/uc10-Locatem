using uc10_Locatem.Model;

///para executar testes
namespace uc10_Locatem.Services.Interfaces
{
    public interface IUsuarioService
    {
        Task<Usuario?> GetUserByEmail(string email);

        Task<bool> UsuarioExiste(string email, string documento);

        Task<Usuario> CriarUsuario(Usuario usuario);
    }
}