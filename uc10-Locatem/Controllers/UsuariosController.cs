using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using uc10_Locatem.Data;
using uc10_Locatem.Model;
using uc10_Locatem.Model.DTO;

namespace uc10_Locatem.Controllers
{
    [ApiController]

    [Route("api/[controller]")]

    public class UsuariosController : ControllerBase
    {
        private readonly AppDbContext _usuarioDbContext;


        public UsuariosController(AppDbContext context)
        {
            _usuarioDbContext = context;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllClientes()
        {
            List<Usuario> listaUsuario = await _usuarioDbContext.Usuario.
                Include(usuario => usuario.Enderecos).
                ToListAsync();


            return Ok(listaUsuario);
        }


        [HttpGet("{tipo}/{id}")]

        public async Task<IActionResult> GetByTipoAndId([FromRoute] string tipo, [FromRoute] int id)
        {
            List<Usuario> listaUsuario = await _usuarioDbContext.Usuario.Include(usuario => usuario.Enderecos).
                ToListAsync();

            var usuario = listaUsuario.FirstOrDefault(usuario => usuario.Id == id && usuario.Tipo.ToLower() == tipo.ToLower());

            if (usuario == null)
            {
                return BadRequest(
                    new
                    {
                        Erro = true,
                        Mensagem = $"O cliente com o id {id} não foi encontrado"
                    }
                    );
            }

            return Ok(usuario);
        }

        [HttpPost("CriarUsuario")]
        public async Task<ActionResult> CriarUsuario([FromBody] CriarUsuarioDTO dadosUsuario)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }



            string Senhacriptografada = BCrypt.Net.BCrypt.HashPassword(dadosUsuario.Senha);

            Usuario usuario = new Usuario
            {
                Nome = dadosUsuario.Nome,
                Email = dadosUsuario.Email,
                Senha = Senhacriptografada,
                TipoUsuario = (Enum.TipoUsuario)dadosUsuario.TipoUsuario,
                Telefone = dadosUsuario.Telefone,
                Documento = dadosUsuario.Documeto,

            };

            _usuarioDbContext.Usuario.Add(usuario);

            int resultadoGravacao = await _usuarioDbContext.SaveChangesAsync();

            if (resultadoGravacao > 0)
            {
                return Ok(
                    new
                    {
                        Erro = false,
                        Mensagem = "Usuário criado com sucesso"
                    }
                );

            }
            else
            {
                return BadRequest(
                    new
                    {
                        Erro = true,
                        Mensagem = "Ocorreu um erro ao criar o usuário"
                    }
                );
            }
        }

        [HttpDelete("DeleteUser{id}")]

        public async Task<IActionResult> DeletarUsuario([FromRoute] int id)
        {
            Usuario? usuarioEncontrado = await _usuarioDbContext.Usuario.FirstOrDefaultAsync(usuario => usuario.Id == id);
            if (usuarioEncontrado == null)
            {
                return BadRequest(
                    new
                    {
                        Erro = true,
                        Mensagem = $"O cliente com o id {id} não foi encontrado"
                    }
                );
            }
            _usuarioDbContext.Usuario.Remove(usuarioEncontrado);
            int resultadoRemocao = await _usuarioDbContext.SaveChangesAsync();
            if (resultadoRemocao > 0)
            {
                return Ok(
                    new
                    {
                        Erro = false,
                        Mensagem = "Usuário deletado com sucesso"
                    }
                );
            }
            else
            {
                return BadRequest(
                    new
                    {
                        Erro = true,
                        Mensagem = "Ocorreu um erro ao deletar o usuário"
                    }
                );
            }

        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] CriarUsuarioDTO dadosLogin)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Usuario? usuarioEncontrado = await _usuarioDbContext.Usuario.FirstOrDefaultAsync(usuario => usuario.Email == dadosLogin.Email);
            if (usuarioEncontrado == null || !BCrypt.Net.BCrypt.Verify(dadosLogin.Senha, usuarioEncontrado.Senha))
            {
                return BadRequest(
                    new
                    {
                        Erro = true,
                        Mensagem = "Email ou senha inválidos"
                    }
                );
            }
            return Ok(
                new
                {
                    Erro = false,
                    Mensagem = "Login realizado com sucesso",
                    UsuarioId = usuarioEncontrado.Id,
                    TipoUsuario = usuarioEncontrado.TipoUsuario
                }
            );
        }

        [HttpPut("AtualizarUsuario{id}")]
        public async Task<IActionResult> AtualizarUsuario([FromRoute] int id, [FromBody] CriarUsuarioDTO dadosAtualizados)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Usuario? usuarioEncontrado = await _usuarioDbContext.Usuario.FirstOrDefaultAsync(usuario => usuario.Id == id);
            if (usuarioEncontrado == null)
            {
                return BadRequest(
                    new
                    {
                        Erro = true,
                        Mensagem = $"O cliente com o id {id} não foi encontrado"
                    }
                );
            }
            usuarioEncontrado.Nome = dadosAtualizados.Nome;
            usuarioEncontrado.Email = dadosAtualizados.Email;
            usuarioEncontrado.Senha = BCrypt.Net.BCrypt.HashPassword(dadosAtualizados.Senha);
            usuarioEncontrado.TipoUsuario = (Enum.TipoUsuario)dadosAtualizados.TipoUsuario;
            usuarioEncontrado.Telefone = dadosAtualizados.Telefone;
            usuarioEncontrado.Documento = dadosAtualizados.Documeto;
            int resultadoAtualizacao = await _usuarioDbContext.SaveChangesAsync();
            if (resultadoAtualizacao > 0)
            {
                return Ok(
                    new
                    {
                        Erro = false,
                        Mensagem = "Usuário atualizado com sucesso"
                    }
                );
            }
            else
            {
                return BadRequest(
                    new
                    {
                        Erro = true,
                        Mensagem = "Ocorreu um erro ao atualizar o usuário"
                    }
                );
            }





        }

        [HttpPut("RecuperarSenha")]
        public async Task<IActionResult> RecuperarSenha([FromBody] string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest(
                    new
                    {
                        Erro = true,
                        Mensagem = "Email é obrigatório"
                    }
                );
            }
            Usuario? usuarioEncontrado = await _usuarioDbContext.Usuario.FirstOrDefaultAsync(usuario => usuario.Email == email);
            if (usuarioEncontrado == null)
            {
                return BadRequest(
                    new
                    {
                        Erro = true,
                        Mensagem = "Email não encontrado"
                    }
                );
            }
            // Aqui você pode implementar a lógica para enviar um email de recuperação de senha ou gerar um token de recuperação
            return Ok(
                new
                {
                    Erro = false,
                    Mensagem = "Instruções para recuperação de senha foram enviadas para o email fornecido"
                }
            );


        }

    }
}