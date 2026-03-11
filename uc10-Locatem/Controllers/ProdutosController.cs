using Microsoft.AspNetCore.Mvc;

namespace uc10_Locatem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProdutosController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAllProdutos()
        {
            var Produtos = new[]
            {
                    new {Id = 1 , Nome ="Furadeira Impacto", Tipo="Elétrica", Marca ="Bosch", Descricao ="Furadeira para uso geral", Voltagem ="220V",Acessorios="Maleta e brocas", PrecoDiaria ="35", Disponivel ="Sim", Categoria ="Furadeiras", DataCadastro= "01/01/2026", Caucao ="150" },
                    new {Id = 2 , Nome ="Martelete Perfurador", Tipo="Elétrica", Marca ="Makita", Descricao ="Ideal para concreto", Voltagem ="220V",Acessorios="Maleta e ponteiros", PrecoDiaria ="60", Disponivel ="Sim", Categoria ="Marteletes", DataCadastro= "02/01/2026", Caucao ="300" },
                    new {Id = 3 , Nome ="Serra Circular", Tipo="Elétrica", Marca ="DeWalt", Descricao ="Serra para cortes em madeira", Voltagem ="110V",Acessorios="Disco de corte", PrecoDiaria ="55", Disponivel ="Sim", Categoria ="Serras", DataCadastro= "03/01/2026", Caucao ="250" },
                    new {Id = 4 , Nome ="Parafusadeira", Tipo="Elétrica", Marca ="Black & Decker", Descricao ="Parafusadeira compacta", Voltagem ="Bivolt",Acessorios="Bits variados", PrecoDiaria ="25", Disponivel ="Sim", Categoria ="Parafusadeiras", DataCadastro= "04/01/2026", Caucao ="120" },
                    new {Id = 5 , Nome ="Lixadeira Orbital", Tipo="Elétrica", Marca ="Makita", Descricao ="Lixamento fino em madeira", Voltagem ="220V",Acessorios="Lixas extras", PrecoDiaria ="30", Disponivel ="Sim", Categoria ="Lixadeiras", DataCadastro= "05/01/2026", Caucao ="140" },
                    new {Id = 6 , Nome ="Esmerilhadeira", Tipo="Elétrica", Marca ="Bosch", Descricao ="Corte e desbaste em metal", Voltagem ="110V",Acessorios="Disco de corte", PrecoDiaria ="40", Disponivel ="Sim", Categoria ="Esmerilhadeiras", DataCadastro= "06/01/2026", Caucao ="180" },
                    new {Id = 7 , Nome ="Compressor de Ar", Tipo="Elétrico", Marca ="Schulz", Descricao ="Compressor portátil", Voltagem ="220V",Acessorios="Mangueira", PrecoDiaria ="70", Disponivel ="Sim", Categoria ="Compressores", DataCadastro= "07/01/2026", Caucao ="400" },
                    new {Id = 8 , Nome ="Betoneira", Tipo="Elétrica", Marca ="Csm", Descricao ="Mistura de concreto", Voltagem ="220V",Acessorios="Tambor", PrecoDiaria ="120", Disponivel ="Sim", Categoria ="Construção", DataCadastro= "08/01/2026", Caucao ="800" },
                    new {Id = 9 , Nome ="Escada Extensiva", Tipo="Manual", Marca ="Mor", Descricao ="Escada de alumínio", Voltagem ="N/A",Acessorios="Trava de segurança", PrecoDiaria ="20", Disponivel ="Sim", Categoria ="Escadas", DataCadastro= "09/01/2026", Caucao ="100" },
                    new {Id = 10 , Nome ="Lavadora Alta Pressão", Tipo="Elétrica", Marca ="Karcher", Descricao ="Lavadora para limpeza pesada", Voltagem ="220V",Acessorios="Mangueira e bico", PrecoDiaria ="50", Disponivel ="Sim", Categoria ="Lavadoras", DataCadastro= "10/01/2026", Caucao ="200" },
                    new {Id = 11 , Nome ="Cortador de Grama", Tipo="Elétrico", Marca ="Tramontina", Descricao ="Corte de grama residencial", Voltagem ="220V",Acessorios="Coletor", PrecoDiaria ="45", Disponivel ="Sim", Categoria ="Jardinagem", DataCadastro= "11/01/2026", Caucao ="180" },
                    new {Id = 12 , Nome ="Soprador de Folhas", Tipo="Elétrico", Marca ="Vonder", Descricao ="Limpeza de folhas", Voltagem ="110V",Acessorios="Bico direcionador", PrecoDiaria ="35", Disponivel ="Sim", Categoria ="Jardinagem", DataCadastro= "12/01/2026", Caucao ="120" },
                    new {Id = 13 , Nome ="Gerador Portátil", Tipo="Combustão", Marca ="Toyama", Descricao ="Gerador para emergências", Voltagem ="Bivolt",Acessorios="Manual", PrecoDiaria ="150", Disponivel ="Sim", Categoria ="Geradores", DataCadastro= "13/01/2026", Caucao ="1000" },
                    new {Id = 14 , Nome ="Serra Tico-Tico", Tipo="Elétrica", Marca ="Bosch", Descricao ="Cortes detalhados", Voltagem ="220V",Acessorios="Lâminas", PrecoDiaria ="28", Disponivel ="Sim", Categoria ="Serras", DataCadastro= "14/01/2026", Caucao ="130" },
                    new {Id = 15 , Nome ="Nivel a Laser", Tipo="Eletrônico", Marca ="Stanley", Descricao ="Alinhamento preciso", Voltagem ="Bateria",Acessorios="Tripé", PrecoDiaria ="50", Disponivel ="Sim", Categoria ="Medição", DataCadastro= "15/01/2026", Caucao ="250" },
                    new {Id = 16 , Nome ="Compactador de Solo", Tipo="Combustão", Marca ="Buffalo", Descricao ="Compactação de terra", Voltagem ="N/A",Acessorios="Base compactadora", PrecoDiaria ="140", Disponivel ="Sim", Categoria ="Construção", DataCadastro= "16/01/2026", Caucao ="900" },
                    new {Id = 17 , Nome ="Cortador de Piso", Tipo="Elétrico", Marca ="Cortag", Descricao ="Corte de porcelanato", Voltagem ="110V",Acessorios="Disco diamantado", PrecoDiaria ="45", Disponivel ="Sim", Categoria ="Construção", DataCadastro= "17/01/2026", Caucao ="220" },
                    new {Id = 18 , Nome ="Escada Tesoura", Tipo="Manual", Marca ="Mor", Descricao ="Escada dobrável", Voltagem ="N/A",Acessorios="Trava", PrecoDiaria ="18", Disponivel ="Sim", Categoria ="Escadas", DataCadastro= "18/01/2026", Caucao ="90" },
                    new {Id = 19 , Nome ="Aspirador Industrial", Tipo="Elétrico", Marca ="Wap", Descricao ="Limpeza pesada", Voltagem ="220V",Acessorios="Bicos diversos", PrecoDiaria ="55", Disponivel ="Sim", Categoria ="Limpeza", DataCadastro= "19/01/2026", Caucao ="200" },
                    new {Id = 20 , Nome ="Serra Sabre", Tipo="Elétrica", Marca ="Makita", Descricao ="Corte em madeira e metal", Voltagem ="220V",Acessorios="Lâminas extras", PrecoDiaria ="48", Disponivel ="Sim", Categoria ="Serras", DataCadastro= "20/01/2026", Caucao ="210" },
            };
            return Ok(Produtos);
        }

        [HttpGet("{id}")]
        public IActionResult GetById([FromRoute] int id)
        {
            var Produtos = new[]
           {
                new { Id = 1, Nome = "Furadeira Impacto", Tipo = "Elétrica", Marca = "Bosch", Descricao = "Furadeira para uso geral", Voltagem = "220V", Acessorios = "Maleta e brocas", PrecoDiaria = "35", Disponivel = "Sim", Categoria = "Furadeiras", DataCadastro = "01/01/2026", Caucao = "150" },
                new { Id = 2, Nome = "Martelete Perfurador", Tipo = "Elétrica", Marca = "Makita", Descricao = "Ideal para concreto", Voltagem = "220V", Acessorios = "Maleta e ponteiros", PrecoDiaria = "60", Disponivel = "Sim", Categoria = "Marteletes", DataCadastro = "02/01/2026", Caucao = "300" },
                new { Id = 3, Nome = "Serra Circular", Tipo = "Elétrica", Marca = "DeWalt", Descricao = "Serra para cortes em madeira", Voltagem = "110V", Acessorios = "Disco de corte", PrecoDiaria = "55", Disponivel = "Sim", Categoria = "Serras", DataCadastro = "03/01/2026", Caucao = "250" },
                new { Id = 4, Nome = "Parafusadeira", Tipo = "Elétrica", Marca = "Black & Decker", Descricao = "Parafusadeira compacta", Voltagem = "Bivolt", Acessorios = "Bits variados", PrecoDiaria = "25", Disponivel = "Sim", Categoria = "Parafusadeiras", DataCadastro = "04/01/2026", Caucao = "120" },
                new { Id = 5, Nome = "Lixadeira Orbital", Tipo = "Elétrica", Marca = "Makita", Descricao = "Lixamento fino em madeira", Voltagem = "220V", Acessorios = "Lixas extras", PrecoDiaria = "30", Disponivel = "Sim", Categoria = "Lixadeiras", DataCadastro = "05/01/2026", Caucao = "140" },
                new { Id = 6, Nome = "Esmerilhadeira", Tipo = "Elétrica", Marca = "Bosch", Descricao = "Corte e desbaste em metal", Voltagem = "110V", Acessorios = "Disco de corte", PrecoDiaria = "40", Disponivel = "Sim", Categoria = "Esmerilhadeiras", DataCadastro = "06/01/2026", Caucao = "180" },
                new { Id = 7, Nome = "Compressor de Ar", Tipo = "Elétrico", Marca = "Schulz", Descricao = "Compressor portátil", Voltagem = "220V", Acessorios = "Mangueira", PrecoDiaria = "70", Disponivel = "Sim", Categoria = "Compressores", DataCadastro = "07/01/2026", Caucao = "400" },
                new { Id = 8, Nome = "Betoneira", Tipo = "Elétrica", Marca = "Csm", Descricao = "Mistura de concreto", Voltagem = "220V", Acessorios = "Tambor", PrecoDiaria = "120", Disponivel = "Sim", Categoria = "Construção", DataCadastro = "08/01/2026", Caucao = "800" },
                new { Id = 9, Nome = "Escada Extensiva", Tipo = "Manual", Marca = "Mor", Descricao = "Escada de alumínio", Voltagem = "N/A", Acessorios = "Trava de segurança", PrecoDiaria = "20", Disponivel = "Sim", Categoria = "Escadas", DataCadastro = "09/01/2026", Caucao = "100" },
                new { Id = 10, Nome = "Lavadora Alta Pressão", Tipo = "Elétrica", Marca = "Karcher", Descricao = "Lavadora para limpeza pesada", Voltagem = "220V", Acessorios = "Mangueira e bico", PrecoDiaria = "50", Disponivel = "Sim", Categoria = "Lavadoras", DataCadastro = "10/01/2026", Caucao = "200" },
        
            };

            var produtoId = Produtos.FirstOrDefault(produto => produto.Id == id);

            if(produtoId == null)
            {
                return BadRequest(
                  //Retorno em formato JSON
                  new
                  {
                      Erro = true,
                      Mensagem = $"O Produto da com id {id} não foi encontrado"
                  }
                  );
            }
            return Ok(produtoId);
        }

        [HttpGet("marca/{marca}")]
        public IActionResult GetByMarca([FromRoute] string marca)
        {
            var Produtos = new[]
           {
                new { Id = 1, Nome = "Furadeira Impacto", Tipo = "Elétrica", Marca = "Bosch", Descricao = "Furadeira para uso geral", Voltagem = "220V", Acessorios = "Maleta e brocas", PrecoDiaria = "35", Disponivel = "Sim", Categoria = "Furadeiras", DataCadastro = "01/01/2026", Caucao = "150" },
                new { Id = 2, Nome = "Martelete Perfurador", Tipo = "Elétrica", Marca = "Makita", Descricao = "Ideal para concreto", Voltagem = "220V", Acessorios = "Maleta e ponteiros", PrecoDiaria = "60", Disponivel = "Sim", Categoria = "Marteletes", DataCadastro = "02/01/2026", Caucao = "300" },
                new { Id = 3, Nome = "Serra Circular", Tipo = "Elétrica", Marca = "DeWalt", Descricao = "Serra para cortes em madeira", Voltagem = "110V", Acessorios = "Disco de corte", PrecoDiaria = "55", Disponivel = "Sim", Categoria = "Serras", DataCadastro = "03/01/2026", Caucao = "250" },
                new { Id = 4, Nome = "Parafusadeira", Tipo = "Elétrica", Marca = "Black & Decker", Descricao = "Parafusadeira compacta", Voltagem = "Bivolt", Acessorios = "Bits variados", PrecoDiaria = "25", Disponivel = "Sim", Categoria = "Parafusadeiras", DataCadastro = "04/01/2026", Caucao = "120" },
                new { Id = 5, Nome = "Lixadeira Orbital", Tipo = "Elétrica", Marca = "Makita", Descricao = "Lixamento fino em madeira", Voltagem = "220V", Acessorios = "Lixas extras", PrecoDiaria = "30", Disponivel = "Sim", Categoria = "Lixadeiras", DataCadastro = "05/01/2026", Caucao = "140" },
                new { Id = 6, Nome = "Esmerilhadeira", Tipo = "Elétrica", Marca = "Bosch", Descricao = "Corte e desbaste em metal", Voltagem = "110V", Acessorios = "Disco de corte", PrecoDiaria = "40", Disponivel = "Sim", Categoria = "Esmerilhadeiras", DataCadastro = "06/01/2026", Caucao = "180" },
                new { Id = 7, Nome = "Compressor de Ar", Tipo = "Elétrico", Marca = "Schulz", Descricao = "Compressor portátil", Voltagem = "220V", Acessorios = "Mangueira", PrecoDiaria = "70", Disponivel = "Sim", Categoria = "Compressores", DataCadastro = "07/01/2026", Caucao = "400" },
                new { Id = 8, Nome = "Betoneira", Tipo = "Elétrica", Marca = "Csm", Descricao = "Mistura de concreto", Voltagem = "220V", Acessorios = "Tambor", PrecoDiaria = "120", Disponivel = "Sim", Categoria = "Construção", DataCadastro = "08/01/2026", Caucao = "800" },
                new { Id = 9, Nome = "Escada Extensiva", Tipo = "Manual", Marca = "Mor", Descricao = "Escada de alumínio", Voltagem = "N/A", Acessorios = "Trava de segurança", PrecoDiaria = "20", Disponivel = "Sim", Categoria = "Escadas", DataCadastro = "09/01/2026", Caucao = "100" },
                new { Id = 10, Nome = "Lavadora Alta Pressão", Tipo = "Elétrica", Marca = "Karcher", Descricao = "Lavadora para limpeza pesada", Voltagem = "220V", Acessorios = "Mangueira e bico", PrecoDiaria = "50", Disponivel = "Sim", Categoria = "Lavadoras", DataCadastro = "10/01/2026", Caucao = "200" },

            };

            var produtoMarca = Produtos.FirstOrDefault(produto => produto.Marca.Equals(marca, StringComparison.OrdinalIgnoreCase));   
            // Verifica se a Marca do produto é igual à marca recebida na rota
            // Equals compara as duas strings
            // StringComparison.OrdinalIgnoreCase faz a comparação ignorando maiúsculas e minúsculas

            if (produtoMarca == null)
            {
                return BadRequest(
                  //Retorno em formato JSON
                  new
                  {
                      Erro = true,
                      Mensagem = $"O Produto da marca {marca} não foi encontrado"
                  }
                  );
            }
            return Ok(produtoMarca);
        }
    }
}