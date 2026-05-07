using FluentAssertions;
using Moq;
using uc10_Locatem.Model;
using uc10_Locatem.Model.DTO;
using uc10_Locatem.Services;
using uc10_Locatem.Services.Interfaces;
using Xunit;

namespace uc10_Locatem.Testes.Servicos
{
    public class AuthServiceTests
    {
        [Fact]
        public async Task Deve_Fazer_Login_Com_Sucesso()
        {
            // Preparação

            var usuario = new Usuario
            {
                Email = "teste@gmail.com",
                Senha = BCrypt.Net.BCrypt.HashPassword("123456")
            };

            var loginDto = new LoginDTO
            {
                Email = "teste@gmail.com",
                Senha = "123456"
            };

            var usuarioServiceMock = new Mock<IUsuarioService>();

            usuarioServiceMock
                .Setup(x => x.GetUserByEmail(loginDto.Email))
                .ReturnsAsync(usuario);

            var authService = new AuthService(usuarioServiceMock.Object);

            // Execução

            var resultado = await authService.Login(loginDto);

            // Verificação

            resultado.Should().NotBeNull();

            resultado!.Email.Should().Be("teste@gmail.com");
        }
    }
}