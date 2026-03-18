
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using uc10_Locatem.Data;
using uc10_Locatem.Services;

namespace uc10_Locatem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddScoped<UsuarioService>();
            builder.Services.AddScoped<TokenService>();


            // Add services to the container.

            builder.Services.AddDbContext<AppDbContext>(options => 
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
          
            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();                            

           //coloquei para a foto de perfil ser salva na pasta wwwroot
           app.UseStaticFiles();

            app.MapControllers();

            app.Run();
        }
    }
}
