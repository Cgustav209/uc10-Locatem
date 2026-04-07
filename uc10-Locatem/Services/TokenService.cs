using uc10_Locatem.Model;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace uc10_Locatem.Services
{
    // Classe responsável por gerar o toker JWT
    // JWT = JSON Web Token
    public class TokenService
    {
        // Interface que permite acessar o appsettings.json
        private readonly IConfiguration _configuration;

        // ================= CONSTRUTOR =================
        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // MÉTODO: GERAR TOKEN JWT
        // Cria e retorna um token com base nas configurações do sistema
        public string GerarToken(Usuario usuario)
        {
            // CONFIGURAÇÕES DO JWT (vindas do appsettings.json)

            // Chave secreta usada para assinar o token (segurança)
            string chaveSecreta = _configuration["Jwt:Key"]!; // a exclamaçã(!) diz que não vai ser nulo

            // Quem está emitindo o token (sua API)
            string issuer = _configuration["Jwt:Issuer"]!;

            // Para quem o token é destinado (clientes que vão consumir)
            string audience = _configuration["Jwt:Audience"]!;

            // Tempo de expiração do token (em horas)
            int expiracaoHoras = int.Parse(_configuration["Jwt:ExpireHours"]!);

            // Convertemos a chave secreta para bytes, pois o algoritimo de assinatura do token espera uma chave em formato de bytes
            var chaveBytes = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(chaveSecreta));

            // Definimos as credenciais de assinatura do token, utilizando a chave secreta e o algoritimo de assinatura HMAC SHA256
            var credenciais = new SigningCredentials(chaveBytes, SecurityAlgorithms.HmacSha256);

            // Definimos as claims do token, que são as informações que queremos incluir no token. As claims são pares de chave-valor que representam informações sobre o usuário autenticado. No exemplo, estamos incluindo o email, nome e id do usuário, além de Jti (JWT ID) que é um identificador único para o token.
            var claims = new[]
            {
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Name, usuario.Nome),
                new Claim("id", usuario.Id.ToString()),
                new Claim("TipoUsuario", usuario.TipoUsuario.ToString()),


                // Id único do token
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            // Montamos o token com todas as informações necessárias, como o emissor, audiência, claims, data de expiração e credenciais de assinatura


            JwtSecurityToken token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(expiracaoHoras),
                signingCredentials: credenciais
            );

            string tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return tokenString;
        }
    }
}
