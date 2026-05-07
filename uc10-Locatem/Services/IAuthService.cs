using uc10_Locatem.Model;
using uc10_Locatem.Model.DTO;

//para fazer testes unitários, é necessário criar uma interface para o serviço de autenticação
namespace uc10_Locatem.Services.Interfaces
{
    public interface IAuthService
    {
        Task<Usuario?> Login(LoginDTO dto);
    }
}