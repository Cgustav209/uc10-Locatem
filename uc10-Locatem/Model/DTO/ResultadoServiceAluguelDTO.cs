namespace uc10_Locatem.Model.DTO
{
    public class ResultadoServiceAluguelDTO
    {
        // Indica se a operação deu certo ou não
        public bool Sucesso { get; set; }

        // Código HTTP que o controller vai usar na resposta
        public int StatusCode { get; set; }

        // Mensagem explicando o resultado
        public string Mensagem { get; set; } = string.Empty;

        // Dados retornados pela operação (objeto criado, lista, etc.)
        public object? Dados { get; set; }
    }
}