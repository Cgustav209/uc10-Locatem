using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;
using uc10_Locatem.Data;
using uc10_Locatem.Services;
using uc10_Locatem.Services.Interfaces;

namespace uc10_Locatem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var chaveSecreta = builder.Configuration["Jwt:Key"];
            var issuer = builder.Configuration["Jwt:Issuer"];
            var audience = builder.Configuration["Jwt:Audience"];

            builder.Services.AddScoped<UsuarioService>();
            builder.Services.AddScoped<AuthService>();
            builder.Services.AddScoped<TokenService>();
            builder.Services.AddScoped<AluguelService>();
            builder.Services.AddScoped<ReservaService>();
            builder.Services.AddScoped<DisponibilidadeService>();
            builder.Services.AddScoped<IDisponibilidadeService, DisponibilidadeService>();


            // Add services to the container.
            builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
          
            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
         


            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(chaveSecreta!)
                )
                };
            });

            builder.Services.AddAuthorization();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();
            }

            app.UseHttpsRedirection();

            // Junta o caminho da pasta do projeto com a pasta "Uploads"
            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

            // Verifica se a pasta "Uploads" NÃO existe
            if (!Directory.Exists(uploadPath))
            {
                // Se não existir, cria a pasta automaticamente
                Directory.CreateDirectory(uploadPath);
            }

            // Configura o servidor para permitir acessar arquivos da pasta "Uploads"
            app.UseStaticFiles(new StaticFileOptions
            {
                // Define o caminho físico onde os arquivos estão no computador
                FileProvider = new PhysicalFileProvider(uploadPath),

                // Define a rota para acessar os arquivos pelo navegador
                // Ex: /Uploads/imagem.png
                RequestPath = "/Uploads"
            });

            // Usar apenas DESENVOLVIMENTO
            app.UseCors("PermitirTudo");

            // Usar em PRODUCAO
            //app.UseCors("PermitirFrontEnd");



            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
